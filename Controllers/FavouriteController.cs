
using System.Security.Claims;
using System.Threading.Tasks;
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
    public async Task<IActionResult> Favourite([FromBody] FavouriteRequestDto request)
    {

        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found.");
            }

            int user_id = int.Parse(userIdClaim.Value);

            bool response = await _favouritesRepository.FavouriteHandlerAsync(request.MalId, user_id, request.Type);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [Authorize]
    [HttpPost("CheckFavourite")]
    public async Task<IActionResult> CheckFavourite([FromBody] FavouriteRequestDto request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found.");
            }
            int user_id = int.Parse(userIdClaim.Value);
            bool isFavourited = await _favouritesRepository.IsFavouriteAsync(request.MalId, user_id);
            return Ok(new { isFavourited });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

    }
    [Authorize]
    [HttpGet("GetFavourites")]
    public async Task<IActionResult> GetFavourites()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found.");
            }
            int user_id = int.Parse(userIdClaim.Value);
            List<Favourite> favourites = await _favouritesRepository.GetFavouritesAsync(user_id);
            return Ok(favourites);
        }
        catch (Exception ex)
        {

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}