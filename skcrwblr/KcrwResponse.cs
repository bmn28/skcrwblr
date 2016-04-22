using System;
using System.Runtime.Serialization;

namespace Skcrwblr
{
    /// <summary>
    /// Represents a track object fetched from the KCRW API.
    /// </summary>
    [DataContract]
    public class KcrwResponse
    {
        [DataMember(Name = "affiliateLinkiPhone")]
        public string AffiliateLinkIPhone { get; set; }
        [DataMember(Name = "program_start")]
        public string ProgramStart { get; set; }
        [DataMember(Name = "affiliateLinkSpotify")]
        public string AffiliateLinkSpotify { get; set; }
        [DataMember(Name = "performance_start")]
        public string PerformanceStart { get; set; }
        [DataMember(Name = "artist_link")]
        public string ArtistLink { get; set; }
        [DataMember(Name = "play_id")]
        public int PlayId { get; set; }
        [DataMember(Name = "offset")]
        public int Offset { get; set; }
        [DataMember(Name = "program_id")]
        public string ProgramId { get; set; }
        [DataMember(Name = "datetime")]
        public string Datetime { get; set; }
        [DataMember(Name = "program_end")]
        public string ProgramEnd { get; set; }
        [DataMember(Name = "affiliateLinkRdio")]
        public string AffiliateLinkRdio { get; set; }
        [DataMember(Name = "albumImage")]
        public string AlbumImage { get; set; }
        [DataMember(Name = "year")]
        public int Year { get; set; }
        [DataMember(Name = "date")]
        public string Date { get; set; }
        [DataMember(Name = "feature_url")]
        public string FeatureUrl { get; set; }
        [DataMember(Name = "program_title")]
        public string ProgramTitle { get; set; }
        [DataMember(Name = "listen_link")]
        public string ListenLink { get; set; }
        [DataMember(Name = "feature_title")]
        public string FeatureTitle { get; set; }
        [DataMember(Name = "albumImageLarge")]
        public string AlbumImageLarge { get; set; }
        [DataMember(Name = "album")]
        public string Album { get; set; }
        [DataMember(Name = "guest")]
        public string Guest { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "artist")]
        public string Artist { get; set; }
        [DataMember(Name = "host")]
        public string Host { get; set; }
        [DataMember(Name = "comments")]
        public string Comments { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "affiliateLinkiTunes")]
        public string AffiliateLinkITunes { get; set; }
        [DataMember(Name = "live")]
        public string Live { get; set; }
        [DataMember(Name = "artist_url")]
        public string ArtistUrl { get; set; }
        [DataMember(Name = "affiliateLinkAmazon")]
        public string AffiliateLinkAmazon { get; set; }
        [DataMember(Name = "time")]
        public string Time { get; set; }
        [DataMember(Name = "action")]
        public string Action { get; set; }
        [DataMember(Name = "credits")]
        public string Credits { get; set; }
        [DataMember(Name = "channel")]
        public string Channel { get; set; }
        [DataMember(Name = "location")]
        public string Location { get; set; }

        private string userArtist = null;
        public string UserArtist
        {
            get
            {
                return userArtist == null ? Artist : userArtist;
            }
            set
            {
                userArtist = value;
            }
        }

        private string userTitle = null;
        public string UserTitle
        {
            get
            {
                return userTitle == null ? Title : userTitle;
            }
            set
            {
                userTitle = value;
            }
        }

        private string userAlbum = null;
        public string UserAlbum
        {
            get
            {
                return userAlbum == null ? Album : userAlbum;
            }
            set
            {
                userAlbum = value;
            }
        }

        public string LastFmArtist { get; set; }
        public string LastFmTitle { get; set; }
        public string LastFmAlbum { get; set; }
        public string LastFmImageUrl { get; set; }
        public bool LastFmFound { get; set; } = false;
        public bool UserScrobbled { get; set; } = false;
        public bool UserLoved { get; set; } = false;

        private DateTime? parsedDateTime = null;
        public DateTime ParsedDateTime
        {
            get
            {
                if (parsedDateTime == null)
                {
                    parsedDateTime = DateTime.Parse(Datetime, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
                }
                return (DateTime)parsedDateTime;
            }
        }
    }
}
