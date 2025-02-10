using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

public class SpotifyService
{
    private readonly HttpClient _httpClient;
    private string _accessToken;

    public SpotifyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            string clientId = Environment.GetEnvironmentVariable("Spotify_ClientID");
            string clientSecret = Environment.GetEnvironmentVariable("Spotify_ClientSecret");
            string clientCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://accounts.spotify.com/api/token"),
                Headers = {
                    {"Authorization",$"Basic {clientCredentials}"}
                },
                Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);

            if (!response.IsSuccessStatusCode)
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get access token: {errorResponse}");
            }

            string responseData = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseData);
            return tokenResponse?.access_token;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public class AlbumDTO
    {
        public string AlbumName { get; set; }
        public string ReleaseDate { get; set; }
        public List<string> ArtistNames { get; set; }
    }

    public async Task<string> GetAlbumByName(string AlbumName)
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            _accessToken = await GetAccessTokenAsync();
        }

        string searchUrl = $"https://api.spotify.com/v1/search?q={AlbumName}&type=album";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, searchUrl);
        request.Headers.Add("Authorization", $"Bearer {_accessToken}");
        HttpResponseMessage response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            string errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to search album: {errorResponse}");
        }

        string responseData = await response.Content.ReadAsStringAsync();

        return responseData;

    }

}