using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/[controller]")]

public class SpotifyController : ControllerBase
{
    private readonly SpotifyService _spotifyService;
    public SpotifyController(SpotifyService spotifyService)
    {
        _spotifyService = spotifyService;
    }

    [HttpGet("access-token")]
    public async Task<IActionResult> GetAccessToken()
    {
        try
        {
            var accessToken = await _spotifyService.GetAccessTokenAsync();
            return Ok(new { AccessToken = accessToken });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}