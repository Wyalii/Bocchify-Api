using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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

    public async Task<dynamic> GetCategories()
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            _accessToken = await GetAccessTokenAsync();
        }

        string categoriesUrl = "https://api.spotify.com/v1/browse/categories";

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, categoriesUrl);
        request.Headers.Add("Authorization", $"Bearer {_accessToken}");
        HttpResponseMessage response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            string errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get categories: {errorResponse}");
        }

        string jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<dynamic>(jsonString);
    }


    public async Task<dynamic> GetPlaylists()
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            _accessToken = await GetAccessTokenAsync();
        }


        string searchUrl = "https://api.spotify.com/v1/featured-playlists?country=US&locale=en_US";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, searchUrl);
        request.Headers.Add("Authorization", $"Bearer {_accessToken}");
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            string errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get categories: {errorResponse}");
        }

        string jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<dynamic>(jsonString);
    }


}