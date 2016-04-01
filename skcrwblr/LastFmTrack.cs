namespace Skcrwblr
{
    /// <summary>
    /// Represents a track on Last.fm.
    /// </summary>
    public class LastFmTrack
    {
        public int listeners = 0;
        public int playcount = 0;
        public string artist = "";
        public string album = "";
        public string title = "";
        public string url = "";
        public string imageUrl = "";
        public string albumArtist = "";
        public bool userLoved = false;
    }

    /// <summary>
    /// Represents an album on Last.fm.
    /// </summary>
    public class LastFmAlbum
    {
        public string artist = "";
        public string album = "";
        public string url = "";
        public string imageUrl = "";
        public string albumArtist = "";
    }

    /// <summary>
    /// Represents an artist on Last.fm.
    /// </summary>
    public class LastFmArtist
    {
        public string artist = "";
        public string url = "";
        public string imageUrl = "";
    }
}
