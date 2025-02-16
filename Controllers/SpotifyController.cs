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

    [HttpGet("get-categories")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _spotifyService.GetCategories();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


    [HttpGet("get-playlists")]
    public async Task<IActionResult> GetPopAlbums()
    {
        try
        {
            var albums = await _spotifyService.GetPlaylists();
            return Ok(albums);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}