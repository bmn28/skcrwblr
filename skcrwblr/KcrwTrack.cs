using System;

namespace Skcrwblr
{
    /// <summary>
    /// Represents a track fetched from the KCRW API.
    /// </summary>
    class KcrwTrack
    {
        public string Artist { get; }
        public string Track { get; }
        public string Album { get; }
        public DateTime Timestamp { get; }
        public KcrwResponse Response { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KcrwTrack"/> class by fetching the first
        /// track in the tracklist returned by accessing the API at the specified URL.
        /// </summary>
        /// <param name="url">The API URL to access.</param>
        public KcrwTrack(string url) : this(KcrwApi.RequestNowPlaying(url)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KcrwTrack"/> class using the specified
        /// <see cref="KcrwResponse"/>.
        /// </summary>
        /// <param name="response">The KcrwResponse to initialize this instance with.</param>
        public KcrwTrack(KcrwResponse response)
        {
            Album = response.Album;
            Track = response.Title;
            Artist = response.Artist;
            Timestamp = DateTime.Parse(response.Datetime);
            Response = response;
        }

        /// <summary>
        /// Indicates whether both the artist and track of this instance are empty.
        /// </summary>
        /// <returns>true if both the artist and track are empty; otherwise, false.</returns>
        public bool IsEmpty()
        {
            return String.IsNullOrWhiteSpace(Artist) && String.IsNullOrWhiteSpace(Track);
        }

        /// <summary>
        /// Determines whether this instance and another specified <see cref="KcrwTrack"/> object
        /// have the same artist and track.
        /// </summary>
        /// <param name="obj">The <see cref="KcrwTrack"/> to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as this instance;
        /// otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            KcrwTrack p = obj as KcrwTrack;
            if ((System.Object)p == null)
            {
                return false;
            }
            return Artist.Equals(p.Artist) && Track.Equals(p.Track);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="KcrwTrack"/>.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Artist.GetHashCode();
                hash = hash * 23 + Track.GetHashCode();
                return hash;
            }
        }
    }
}
