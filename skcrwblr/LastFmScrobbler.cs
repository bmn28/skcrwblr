using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Skcrwblr
{
    /// <summary>
    /// Makes requests to the Last.fm API.
    /// </summary>
    public class LastFmScrobbler : IDisposable
    {
        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const string ApiKey = ApiCredentials.ApiKey;
        private const string Secret = ApiCredentials.Secret;

        private HttpClient client;
        private MD5 md5Hash;
        private string username;
        private string sessionKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastFmScrobbler"/> class.
        /// </summary>
        public LastFmScrobbler()
        {
            md5Hash = MD5.Create();
            client = new HttpClient();
            client.DefaultRequestHeaders.ExpectContinue = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                client.Dispose();
                md5Hash.Dispose();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The name of the currently logged in user, or null if not logged in.
        /// </summary>
        public string Username
        {
            get
            {
                return (sessionKey != null) ? username : null;
            }
        }

        /// <summary>
        /// Calculates the MD5 hash of the given string.
        /// </summary>
        /// <param name="input">A string.</param>
        /// <returns>The MD5 hash of the input represented as a string of hexadecimal digits.
        /// </returns>
        private string GetMd5Hash(string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// Generates an API method signature.
        /// </summary>
        /// <param name="input">A dictionary of the parameters for the API request.</param>
        /// <returns>The API method signature.</returns>
        private string GetApiSig(Dictionary<string, string> input)
        {
            var list = input.Keys.ToList();
            list.Sort();

            StringBuilder sBuilder = new StringBuilder();
            foreach (var key in list)
            {
                sBuilder.Append(key);
                sBuilder.Append(input[key]);
            }
            sBuilder.Append(Secret);

            return GetMd5Hash(sBuilder.ToString());
        }

        /// <summary>
        /// Calls the Last.fm API and returns the response as a string.
        /// </summary>
        /// <param name="input">A dictionary of the parameters for the API request.</param>
        /// <returns>A string containing the API response.</returns>
        private async Task<string> CallApi(Dictionary<string, string> input)
        {
            var values = new Dictionary<string, string>(input);
            string apiSig = GetApiSig(values);
            values.Add("api_sig", apiSig);

            var content = new FormUrlEncodedContent(values);
            var responseMsg = await client.PostAsync("http://ws.audioscrobbler.com/2.0/", content);
            return await responseMsg.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Calls the Last.fm API and returns the response as an XmlReader.
        /// </summary>
        /// <param name="input">A dictionary of the parameters for the API request.</param>
        /// <returns>An <see cref="XmlReader"/> containing the API response.</returns>
        private async Task<XmlReader> ReadApi(Dictionary<string, string> input)
        {
            var reader = XmlReader.Create(new StringReader(await CallApi(input)));
            reader.ReadToFollowing("lfm");
            string status = reader.GetAttribute("status");
            if (status.Equals("ok"))
            {
                return reader;
            }
            else
            {
                reader.ReadToFollowing("error");
                string msg = reader.ReadElementContentAsString();
                reader.Dispose();
                throw new System.Runtime.Remoting.ServerException(msg);
            }
        }

        /// <summary>
        /// Authorizes a Last.fm web service session.
        /// </summary>
        /// <returns>true if the authorization was successful; otherwise, false.</returns>
        public async Task<bool> Authorize()
        {
            var token = await GetToken();

            if (token == null)
            {
                return false;
            }

            AuthWindow authWindow = new AuthWindow();
            authWindow.Navigate("http://www.last.fm/api/auth/?api_key=" + ApiKey + "&token=" + token);
            authWindow.ShowDialog();

            await GetSession(token);

            if (sessionKey != null)
            {
                WriteSession();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Fetches an unauthorized request token for the API account.
        /// </summary>
        /// <returns>The unauthorized request token.</returns>
        private async Task<string> GetToken()
        {
            var values = new Dictionary<string, string>
                {
                    {"method", "auth.gettoken"},
                    {"api_key", ApiKey},
                };

            try
            {
                using (var reader = await ReadApi(values))
                {
                    reader.ReadToFollowing("token");
                    return reader.ReadElementContentAsString();
                }
            }
            catch (XmlException xEx)
            {
                throw new InvalidOperationException(xEx.Message);
            }
        }

        /// <summary>
        /// Initiates a web service session using the specified token.
        /// </summary>
        /// <param name="token">true if the session was initiated successfully; otherwise, false.
        /// </param>
        private async Task<bool> GetSession(string token)
        {
            if (token == null)
            {
                throw new InvalidOperationException("No token.");
            }

            var values = new Dictionary<string, string>
            {
                {"method", "auth.getSession"},
                {"api_key", ApiKey},
                {"token", token},
            };

            try
            {
                using (var reader = await ReadApi(values))
                {
                    reader.ReadToFollowing("name");
                    username = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("key");
                    sessionKey = reader.ReadElementContentAsString();

                    return true;
                }
            }
            catch (XmlException xEx)
            {
                throw new InvalidOperationException(xEx.Message);
            }
        }

        /// <summary>
        /// Writes the username and session key to the registry.
        /// </summary>
        public void WriteSession()
        {
            if (sessionKey != null)
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software", true);
                rk = rk.CreateSubKey("skcrwblr");
                rk = rk.CreateSubKey("Credentials");
                rk.SetValue("sk", sessionKey);
                rk.SetValue("username", username);
            }
        }

        /// <summary>
        /// Attempts to read the username and session key from the registry.
        /// </summary>
        /// <returns>true if successful; otherwise, false.</returns>
        public bool ReadSession()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("skcrwblr");
            if (rk == null)
            {
                return false;
            }
            rk = rk.OpenSubKey("Credentials");
            if (rk == null)
            {
                return false;
            }
            sessionKey = rk.GetValue("sk") as string;
            username = rk.GetValue("username") as string;

            return sessionKey != null;
        }

        /// <summary>
        /// Clears the username and session key, and deletes them from the registry.
        /// </summary>
        public void EndSession()
        {
            username = null;
            sessionKey = null;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("skcrwblr", true);
            rk.DeleteSubKeyTree("Credentials");
        }

        /// <summary>
        /// Updates the now playing track on Last.fm.
        /// </summary>
        /// <param name="artist">The artist of the track to update with.</param>
        /// <param name="track">The track to update with.</param>
        /// <param name="album">The album of the track to update with.</param>
        public async Task UpdateNowPlaying(string artist, string track, string album)
        {
            if (sessionKey == null)
            {
                throw new InvalidOperationException("Not logged in.");
            }

            var values = new Dictionary<string, string>
            {
                {"method", "track.updateNowPlaying"},
                {"artist", artist},
                {"track", track},
                {"album", album},
                {"api_key", ApiKey},
                {"sk", sessionKey},
            };

            using (XmlReader reader = await ReadApi(values))
            {
                reader.ReadToFollowing("ignoredMessage");
                string msg = reader.ReadElementContentAsString();
                if (!string.IsNullOrEmpty(msg))
                {
                    throw new ServerException(msg);
                }
            }
        }

        /// <summary>
        /// Scrobbles a track on Last.fm.
        /// </summary>
        /// <param name="artist">The artist of the track to scrobble.</param>
        /// <param name="track">The track to scrobble.</param>
        /// <param name="album">The album of the track to scrobble.</param>
        /// <param name="timestamp">The time at which the track began playing.</param>
        public async Task Scrobble(string artist, string track, string album, DateTime timestamp)
        {
            if (sessionKey == null)
            {
                throw new InvalidOperationException("Not logged in.");
            }

            var values = new Dictionary<string, string>
            {
                {"method", "track.scrobble"},
                {"artist", artist},
                {"track", track},
                {"timestamp", (timestamp - epoch).TotalSeconds.ToString()},
                {"album", album},
                {"chosenByUser", "0"},
                {"api_key", ApiKey},
                {"sk", sessionKey},
            };

            using (XmlReader reader = await ReadApi(values))
            {
                reader.ReadToFollowing("ignoredMessage");
                string msg = reader.ReadElementContentAsString();
                if (!string.IsNullOrEmpty(msg))
                {
                    throw new ServerException(msg);
                }
            }
        }

        public async Task Scrobble(KcrwResponse value, bool preferLastFm = false)
        {
            string artist;
            string title;
            string album;

            if (preferLastFm)
            {
                artist = value.LastFmArtist != null ? value.LastFmArtist : value.Artist;
                title = value.LastFmTitle != null ? value.LastFmTitle : value.Title;
                album = value.LastFmAlbum != null ? value.LastFmAlbum : value.Album;
            }
            else
            {
                artist = value.UserArtist != null ? value.UserArtist : value.Artist;
                title = value.UserTitle != null ? value.UserTitle : value.Title;
                album = value.UserAlbum != null ? value.UserAlbum : value.Album;
            }
            await Scrobble(artist, title, album, value.ParsedDateTime);
        }

        public async Task Search(KcrwResponse value, bool useUserData)
        {
            try
            {
                string artist = (useUserData && value.UserArtist != null) ? value.UserArtist : value.Artist;
                string title = (useUserData && value.UserTitle != null) ? value.UserTitle : value.Title;
                LastFmTrack track = await GetInfo(artist, title);
                value.LastFmArtist = track.artist;
                value.LastFmTitle = track.title;
                value.LastFmAlbum = track.album;
                value.LastFmImageUrl = track.imageUrl;
                value.UserLoved = track.userLoved;
                value.LastFmFound = true;
            }
            catch (ServerException ex)
            {
                if (ex.Message.Equals("Track not found"))
                {
                    value.LastFmFound = false;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Loves a track on Last.fm.
        /// </summary>
        /// <param name="artist">The artist of the track to love.</param>
        /// <param name="track">The track to love.</param>
        public async Task Love(string artist, string track)
        {
            if (sessionKey == null)
            {
                throw new InvalidOperationException("Not logged in.");
            }

            var values = new Dictionary<string, string>
            {
                {"method", "track.love"},
                {"artist", artist},
                {"track", track},
                {"api_key", ApiKey},
                {"sk", sessionKey},
            };

            await ReadApi(values);
        }

        /// <summary>
        /// Unloves a track on Last.fm.
        /// </summary>
        /// <param name="artist">The artist of the track to unlove.</param>
        /// <param name="track">The track to unlove.</param>
        public async Task Unlove(string artist, string track)
        {
            if (sessionKey == null)
            {
                throw new InvalidOperationException("Not logged in.");
            }

            var values = new Dictionary<string, string>
            {
                {"method", "track.unlove"},
                {"artist", artist},
                {"track", track},
                {"api_key", ApiKey},
                {"sk", sessionKey},
            };

            await ReadApi(values);
        }

        /// <summary>
        /// Gets track info from Last.fm.
        /// </summary>
        /// <param name="inputArtist">The artist of the track to search for.</param>
        /// <param name="inputTrack">The track to search for.</param>
        /// <returns>A <see cref="LastFmTrack"/> instance representing the returned track.</returns>
        public async Task<LastFmTrack> GetInfo(string inputArtist, string inputTrack)
        {
            var values = new Dictionary<string, string>
                {
                    { "method", "track.getInfo" },
                    { "artist", inputArtist },
                    { "track", inputTrack },
                    { "autocorrect", "1" },
                    { "api_key", ApiKey },
                };

            if (sessionKey != null)
            {
                values.Add("username", username);
            }

            using (XmlReader reader = await ReadApi(values))
            {
                var track = new LastFmTrack();

                reader.ReadToFollowing("name");
                track.title = reader.ReadElementContentAsString();
                reader.ReadToFollowing("url");
                track.url = reader.ReadElementContentAsString();

                if (reader.ReadToFollowing("listeners"))
                {
                    track.listeners = reader.ReadElementContentAsInt();
                }
                    
                if (reader.ReadToFollowing("playcount"))
                {
                    track.playcount = reader.ReadElementContentAsInt();
                }
                reader.ReadToFollowing("artist");
                reader.ReadToDescendant("name");
                track.artist = reader.ReadElementContentAsString();
                if (reader.ReadToFollowing("album"))
                {
                    if (reader.ReadToDescendant("artist"))
                    {
                        track.albumArtist = reader.ReadElementContentAsString();
                        reader.ReadToNextSibling("title");
                        track.album = reader.ReadElementContentAsString();
                    }
                    else
                    {
                        reader.ReadToDescendant("title");
                        track.album = reader.ReadElementContentAsString();
                    }
                    if (reader.ReadToNextSibling("image"))
                    {
                        track.imageUrl = reader.ReadElementContentAsString();
                        if (reader.ReadToNextSibling("image"))
                        {
                            track.imageUrl = reader.ReadElementContentAsString();
                            if (reader.ReadToNextSibling("image"))
                            {
                                track.imageUrl = reader.ReadElementContentAsString();
                            }
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(track.imageUrl))
                {
                    track.imageUrl = await GetArtistImage(inputArtist);
                }
                if (sessionKey != null)
                {
                    if (reader.ReadToFollowing("userloved") && reader.ReadElementContentAsInt() == 1)
                    {
                        track.userLoved = true;
                    }
                }

                return track;
            }
        }

        /// <summary>
        /// Gets artist info from Last.fm.
        /// </summary>
        /// <param name="inputArtist">The artist to search for.</param>
        /// <returns>A <see cref="LastFmArtist"/> instance representing the returned artist.</returns>
        public async Task<LastFmArtist> GetArtist(string inputArtist)
        {
            var values = new Dictionary<string, string>
                {
                    { "method", "artist.getInfo" },
                    { "artist", inputArtist },
                    { "autocorrect", "1" },
                    { "api_key", ApiKey },
                };

            using (XmlReader reader = await ReadApi(values))
            {
                var artist = new LastFmArtist();

                reader.ReadToFollowing("name");
                artist.artist = reader.ReadElementContentAsString();
                reader.ReadToFollowing("url");
                artist.url = reader.ReadElementContentAsString();

                if (reader.ReadToNextSibling("image"))
                {
                    artist.imageUrl = reader.ReadElementContentAsString();
                    if (reader.ReadToNextSibling("image"))
                    {
                        artist.imageUrl = reader.ReadElementContentAsString();
                        if (reader.ReadToNextSibling("image"))
                        {
                            artist.imageUrl = reader.ReadElementContentAsString();
                        }
                    }
                }

                return artist;
            }
        }

        /// <summary>
        /// Gets album info from Last.fm.
        /// </summary>
        /// <param name="inputArtist">The artist of the album to search for.</param>
        /// <param name="inputAlbum">The album to search for.</param>
        /// <returns>A <see cref="LastFmAlbum"/> instance representing the returned album.</returns>
        public async Task<LastFmAlbum> GetAlbum(string inputArtist, string inputAlbum)
        {
            var values = new Dictionary<string, string>
                {
                    { "method", "album.getInfo" },
                    { "artist", inputArtist },
                    { "album", inputAlbum},
                    { "autocorrect", "1" },
                    { "api_key", ApiKey },
                };

            using (XmlReader reader = await ReadApi(values))
            {
                var album = new LastFmAlbum();

                reader.ReadToFollowing("name");
                album.album = reader.ReadElementContentAsString();
                reader.ReadToFollowing("artist");
                album.artist = reader.ReadElementContentAsString();
                reader.ReadToFollowing("url");
                album.url = reader.ReadElementContentAsString();

                if (reader.ReadToNextSibling("image"))
                {
                    album.imageUrl = reader.ReadElementContentAsString();
                    if (reader.ReadToNextSibling("image"))
                    {
                        album.imageUrl = reader.ReadElementContentAsString();
                        if (reader.ReadToNextSibling("image"))
                        {
                            album.imageUrl = reader.ReadElementContentAsString();
                        }
                    }
                }

                return album;
            }
        }

        /// <summary>
        /// Gets an artist image URL from Last.fm.
        /// </summary>
        /// <param name="inputArtist">The artist to search for.</param>
        /// <returns>A URL to the artist image.</returns>
        public async Task<string> GetArtistImage(string inputArtist)
        {
            string imageUrl = "";
            var values = new Dictionary<string, string>
                {
                    { "method", "artist.getInfo" },
                    { "artist", inputArtist },
                    { "autocorrect", "1" },
                    { "api_key", ApiKey },
                };

            using (XmlReader reader = await ReadApi(values))
            {
                reader.ReadToFollowing("artist");
                if (reader.ReadToDescendant("image"))
                {
                    imageUrl = reader.ReadElementContentAsString();
                    if (reader.ReadToNextSibling("image"))
                    {
                        imageUrl = reader.ReadElementContentAsString();
                        if (reader.ReadToNextSibling("image"))
                        {
                            imageUrl = reader.ReadElementContentAsString();
                        }
                    }
                }

            }

            return imageUrl;
        }
    }
}
