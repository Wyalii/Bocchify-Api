
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class FavouriteController : ControllerBase
{
    private readonly FavouritesRepository _favouritesRepository;
    private readonly TokenService _tokenService;
    public FavouriteController(FavouritesRepository favouritesRepository, TokenService tokenService)
    {
        _favouritesRepository = favouritesRepository;
        _tokenService = tokenService;
    }
    [HttpPost("Favourite")]
    public IActionResult Favourite([FromBody] FavouriteRequestDto request)
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Missing or invalid Authorization header.");
        }
        var token = authHeader.Substring("Bearer ".Length).Trim();
        var claims = _tokenService.GetClaimsFromToken(token);
        if (claims == null)
        {
            return Unauthorized("Invalid or expired token.");
        }

        var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }

        int user_id = int.Parse(userIdClaim.Value);

        bool response = _favouritesRepository.FavouriteHandler(request.MalId, user_id);
        return Ok(response);
    }
    [HttpPost("CheckFavourite")]
    public IActionResult CheckFavourite([FromBody] FavouriteRequestDto request)
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Missing or invalid Authorization header.");
        }
        var token = authHeader.Substring("Bearer ".Length).Trim();
        var claims = _tokenService.GetClaimsFromToken(token);
        if (claims == null)
        {
            return Unauthorized("Invalid or expired token.");
        }

        var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }

        int user_id = int.Parse(userIdClaim.Value);
        bool isFavourited = _favouritesRepository.IsFavourite(request.MalId, user_id);
        return Ok(new { isFavourited });

    }
}