public class ExternalUrls
{
    public string spotify { get; set; }
}

public class PlaylistImage
{
    public string url { get; set; }
}

public class Owner
{
    public string display_name { get; set; }
    public string id { get; set; }
}

public class Playlist
{
    public string id { get; set; }
    public string name { get; set; }
    public List<PlaylistImage> images { get; set; }
    public string description { get; set; }
    public ExternalUrls external_urls { get; set; }
    public Owner owner { get; set; }
    public string type { get; set; }
    public string uri { get; set; }
    public bool collaborative { get; set; }
}

public class Playlists
{
    public List<Playlist> items { get; set; }
    public string href { get; set; }
    public int limit { get; set; }
    public string next { get; set; }
    public string previous { get; set; }
    public int total { get; set; }
}

public class SpotifySearchResponse
{
    public Playlists playlists { get; set; }
}
