
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [HttpPost("Favourite")]
    public IActionResult Favourite([FromBody] FavouriteRequestDto request)
    {

        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found.");
            }

            int user_id = int.Parse(userIdClaim.Value);

            bool response = _favouritesRepository.FavouriteHandler(request.MalId, user_id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [Authorize]
    [HttpPost("CheckFavourite")]
    public IActionResult CheckFavourite([FromBody] FavouriteRequestDto request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found.");
            }
            int user_id = int.Parse(userIdClaim.Value);
            bool isFavourited = _favouritesRepository.IsFavourite(request.MalId, user_id);
            return Ok(new { isFavourited });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

    }
    // [HttpGet("GetFavourites")]
    // public IActionResult GetFavourites()
    // {

    // }
}