using System.Net;
using System.Text;
using System.Text.Json;

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

}