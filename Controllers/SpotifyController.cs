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

    [HttpPost("get-album")]
    public async Task<IActionResult> GetAccessToken([FromBody] string albumRequest)
    {
        try
        {
            var FoundAlbum = await _spotifyService.GetAlbumByName(albumRequest);
            return Ok(FoundAlbum);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}