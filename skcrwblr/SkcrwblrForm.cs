using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using Microsoft.Win32;

namespace Skcrwblr
{
    /// <summary>
    /// The main skcrwblr application window.
    /// </summary>
    public partial class SkcrwblrForm : Form
    {
        private static List<Tracklist> tracklists = new List<Tracklist>
        {
            new Tracklist("http://tracklist-api.kcrw.com/Simulcast/all"),
            new Tracklist("http://tracklist-api.kcrw.com/Music/all"),
        };

        private static List<Stream> streams = new List<Stream>
        {
            new Stream("KCRW Radio 89.9", "http://kcrw.ic.llnwd.net/stream/kcrw_live", tracklists[0]),
            new Stream("Eclectic 24", "http://kcrw.ic.llnwd.net/stream/kcrw_music", tracklists[1]),
            new Stream("KCRW Radio 89.9 (alternate)", "http://sc11.sjc.llnw.net/stream/kcrw_live", tracklists[0]),
        };

        private LastFmScrobbler scrobbler;
        private Mp3Streamer streamer;
        private bool loggedIn;
        private bool lovedCurrent;
        private bool textChanged;
        private bool exiting;

        public KcrwResponse LastTrack
        {
            get
            {
                return ((Stream)comboBoxStream.SelectedItem).Tracklist.Last.Value;
            }
        }

        private LinkedListNode<KcrwResponse> selectedNode = null;

        public KcrwResponse SelectedTrack
        {
            get
            {
                return selectedNode.Value;
            }
        }

        public SkcrwblrForm()
        {
            InitializeComponent();
            scrobbler = new LastFmScrobbler();
            streamer = new Mp3Streamer(streams[0].StreamUrl);
            streamer.getVolume = () => volumeControl.Volume;
            streamer.Error += (message => { log("Playback error: " + message); });
            streamer.Playing += (() => { setButtonStates(false, true, true); });
            streamer.Paused += (() => { setButtonStates(true, false, true); });
            streamer.Stopped += (() => { setButtonStates(true, false, false); });
            streamer.Buffering += (() => { setButtonStates(true, true, true); });

            loggedIn = false;
            LovedCurrent = false;
            textChanged = false;
            exiting = false;

            if (scrobbler.ReadSession()) completeLogin();

            comboBoxStream.DataSource = streams;
            comboBoxStream.DisplayMember = "Title";

            readRegistry();

            // enable transparency for labelAlbumArt
            var pos = PointToScreen(labelAlbumArt.Location);
            pos = albumArt.PointToClient(pos);
            labelAlbumArt.Parent = albumArt;
            labelAlbumArt.Location = pos;

            deselect();
            buttonScrobble.Focus();
        }

        /// <summary>
        /// Attempts to read settings from the registry.
        /// </summary>
        private void readRegistry()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("skcrwblr");
            if (rk != null)
            {
                object keyValue = rk.GetValue("Autocorrect");
                if (keyValue != null)
                {
                    checkBoxAutoCorrect.Checked = Convert.ToBoolean(keyValue);
                }
                keyValue = rk.GetValue("Autoscrobble");
                if (keyValue != null)
                {
                    checkBoxAutoScrobble.Checked = Convert.ToBoolean(keyValue);
                }
                keyValue = rk.GetValue("Channel");
                if (keyValue != null)
                {
                    try
                    {
                        comboBoxStream.SelectedIndex = Convert.ToInt32(keyValue);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // ok
                    }
                }
                keyValue = rk.GetValue("Volume");
                if (keyValue != null)
                {
                    try
                    {
                        volumeControl.VolumeAsPercent = Convert.ToInt32(keyValue);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // ok
                    }
                }

            }
        }

        /// <summary>
        /// Attempts to write settings to the registry.
        /// </summary>
        private void writeRegistry()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("skcrwblr");
            rk.SetValue("Autocorrect", checkBoxAutoCorrect.Checked);
            rk.SetValue("Autoscrobble", checkBoxAutoScrobble.Checked);
            rk.SetValue("Channel", comboBoxStream.SelectedIndex);
            rk.SetValue("Volume", volumeControl.VolumeAsPercent);
        }

        private async void updateTracklist(bool enableScrobbling = true)
        {
            try
            {
                Tracklist tracklist = ((Stream)comboBoxStream.SelectedItem).Tracklist;
                LinkedListNode<KcrwResponse> lastNode = tracklist.Last;
                int numAdded = tracklist.Update();
                if (numAdded > 0)
                {
                    if (lastNode != null)
                    {
                        bool autoscrobbling = loggedIn && checkBoxAutoScrobble.Checked && enableScrobbling;
                        while (lastNode != tracklist.Last)
                        {
                            if (autoscrobbling)
                            {
                                if (checkBoxAutoCorrect.Checked)
                                {
                                    try
                                    {
                                        await scrobbler.Search(lastNode.Value, false);
                                    }
                                    catch (Exception)
                                    {
                                        // ok
                                    }
                                }
                                try
                                {
                                    if (!String.IsNullOrEmpty(lastNode.Value.UserArtist) && !String.IsNullOrEmpty(lastNode.Value.UserTitle))
                                    {
                                        await scrobbler.Scrobble(lastNode.Value, checkBoxAutoCorrect.Checked);
                                        log("Scrobbled " + lastNode.Value.UserArtist + " - " + lastNode.Value.UserTitle + ".");
                                    }
                                }
                                catch (Exception ex) when (ex is WebException || ex is System.Runtime.Remoting.ServerException)
                                {
                                    log("Error scrobbling: " + ex.Message);
                                }
                            }
                            lastNode = lastNode.Next;
                        }
                    }
                    buttonScrobble.Text = "Scrobble now";
                    buttonScrobble.Enabled = true;
                    LovedCurrent = false;
                    selectedNode = tracklist.Last;
                    await findCorrection(LastTrack);
                    textChanged = false;
                    populateFields(LastTrack);
                    updateNowPlaying();
                    labelProgram.Text = "Program: " + tracklist.Last.Value.ProgramTitle;
                }
            }
            catch (Exception ex)
            {
                log("Error getting tracklist: " + ex.Message);
                //connectedToKcrw = false;
                labelProgram.Text = "Program: ";
            }
        }

        /// <summary>
        /// Populates the metadata fields with the current track.
        /// </summary>
        private void populateFields(KcrwResponse response)
        {
            if (!response.LastFmFound)
            {
                resetTrackDisplay();
            }
            else
            {
                buttonCorrectSpelling.Text = "Correct spelling";
            }
            if (!string.IsNullOrEmpty(response.LastFmImageUrl))
            {
                try
                {
                    albumArt.Load(response.LastFmImageUrl);
                    labelAlbumArt.Text = "";
                }
                catch (Exception ex) when (ex is ArgumentException || ex is WebException)
                {
                    albumArt.Image = null;
                    labelAlbumArt.Text = "No album art";
                }
            }
            else
            {
                albumArt.Image = null;
                labelAlbumArt.Text = "No album art";
            }
            LovedCurrent = response.UserLoved;
            if (checkBoxAutoCorrect.Checked)
            {
                useCorrectSpelling(response);
            }
            else
            {
                textBoxAlbum.Text = response.UserAlbum;
                textBoxTitle.Text = response.UserTitle;
                textBoxArtist.Text = response.UserArtist;
                colorize(response);
            }
            updateTime(response.ParsedDateTime, response == LastTrack);
            if (response.UserScrobbled)
            {
                buttonScrobble.Text = "Scrobbled";
                buttonScrobble.Enabled = false;
            }
            else
            {
                buttonScrobble.Text = "Scrobble now";
                buttonScrobble.Enabled = true;
            }
        }

        private void updateTime(DateTime dateTime, bool includeDuration = true)
        {
            TimeSpan timeSpan = DateTime.Now.ToUniversalTime() - dateTime;
            string timeCode = timeSpan.TotalHours >= 1F ? timeSpan.ToString(@"h\:mm\:ss") : timeSpan.ToString(@"m\:ss");
            labelTime.Text = dateTime.ToLocalTime().ToShortTimeString();
            if (includeDuration)
            {
                labelTime.Text += " - " + timeCode;
            }
        }

        /// <summary>
        /// Searches Last.fm for corrections, storing the retrieved metadata in lfmTrack.
        /// </summary>
        private async Task findCorrection(KcrwResponse response)
        {
            if (!string.IsNullOrWhiteSpace(response.UserArtist) && !string.IsNullOrWhiteSpace(response.UserTitle))
            {
                try
                {
                    if (textChanged || !response.LastFmFound)
                    {
                        buttonCorrectSpelling.Text = "Searching...";
                        buttonCorrectSpelling.Enabled = false;

                        await scrobbler.Search(response, true);

                        buttonCorrectSpelling.Text = "Correct spelling";
                        buttonCorrectSpelling.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Equals("Track not found"))
                    {
                        log("Error searching for track: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Resets the UI to indicate that no track was found.
        /// </summary>
        private void resetTrackDisplay()
        {
            albumArt.Image = null;
            labelAlbumArt.Text = "Track not found";

            textBoxArtist.BackColor = SystemColors.Window;
            textBoxTitle.BackColor = SystemColors.Window;
            textBoxAlbum.BackColor = SystemColors.Window;

            buttonCorrectSpelling.Text = "Search";
            buttonCorrectSpelling.Enabled = true;
        }

        /// <summary>
        /// Populates the metadata fields with the data from Last.fm, colorizing to indicate
        /// consistency with the data from KCRW.
        /// </summary>
        private void useCorrectSpelling(KcrwResponse response)
        {
            if (response.LastFmFound)
            {
                textBoxArtist.Text = response.LastFmArtist;
                textBoxTitle.Text = response.LastFmTitle;
                textBoxAlbum.Text = response.LastFmAlbum;
                textBoxArtist.BackColor = response.LastFmArtist.Trim().ToLower().Equals(response.UserArtist.Trim().ToLower()) ?
                    Color.Honeydew : Color.LavenderBlush;
                textBoxTitle.BackColor = response.LastFmTitle.Trim().ToLower().Equals(response.UserTitle.Trim().ToLower()) ?
                    Color.Honeydew : Color.LavenderBlush;
                textBoxAlbum.BackColor = response.LastFmAlbum.Trim().ToLower().Equals(response.UserAlbum.Trim().ToLower()) ?
                    Color.Honeydew : Color.LavenderBlush;
            }
            else
            {
                textBoxArtist.BackColor = SystemColors.Window;
                textBoxTitle.BackColor = SystemColors.Window;
                textBoxAlbum.BackColor = SystemColors.Window;
            }
            updateNowPlaying();
        }

        /// <summary>
        /// Colorizes the metadata fields to indicate if they conform to the data from Last.fm.
        /// </summary>
        private void colorize(KcrwResponse response)
        {
            if (response.LastFmFound)
            {
                textBoxArtist.BackColor = response.UserArtist.Trim().ToLower().Equals(response.LastFmArtist.Trim().ToLower()) ?
                    Color.Honeydew : Color.LavenderBlush;
                textBoxTitle.BackColor = response.UserTitle.Trim().ToLower().Equals(response.LastFmTitle.Trim().ToLower()) ?
                    Color.Honeydew : Color.LavenderBlush;
                textBoxAlbum.BackColor = string.IsNullOrWhiteSpace(response.LastFmAlbum) || response.UserAlbum.Trim().ToLower().Equals(response.LastFmAlbum.Trim().ToLower()) ?
                    Color.Honeydew : Color.LavenderBlush;
            }
            else
            {
                textBoxArtist.BackColor = SystemColors.Window;
                textBoxTitle.BackColor = SystemColors.Window;
                textBoxAlbum.BackColor = SystemColors.Window;
            }
        }

        /// <summary>
        /// Update the now playing track on Last.fm, if logged in.
        /// </summary>
        private async void updateNowPlaying()
        {
            if (loggedIn && !String.IsNullOrWhiteSpace(LastTrack.UserArtist) && !String.IsNullOrWhiteSpace(LastTrack.UserTitle))
            {
                try
                {
                    await scrobbler.UpdateNowPlaying(LastTrack.UserArtist, LastTrack.UserTitle, LastTrack.UserAlbum);
                }
                catch (Exception ex)
                {
                    log("Error updating now playing: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Attempts to scrobble the current track. Assumes the user is logged in.
        /// </summary>
        private async Task scrobbleTrack(KcrwResponse response)
        {
            if (String.IsNullOrEmpty(response.UserArtist) || String.IsNullOrEmpty(response.UserTitle))
            {
                if (!response.UserArtist.Equals("[BREAK]"))
                {
                    log("Artist or song title is blank.");
                }
            }
            else
            {
                try
                {
                    await scrobbler.Scrobble(response, checkBoxAutoCorrect.Checked);
                    response.UserScrobbled = true;
                    log("Scrobbled " + response.UserArtist + " - " + response.UserTitle + ".");
                    buttonScrobble.Text = "Scrobbled";
                    buttonScrobble.Enabled = false;
                }                    
                catch (Exception ex)
                {
                    log("Error scrobbling: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Attempts to love the current track. Assumes the user is logged in.
        /// </summary>
        private async Task loveTrack(KcrwResponse response)
        {
            if (String.IsNullOrEmpty(response.UserArtist) || String.IsNullOrEmpty(response.UserTitle))
            {
                log("Artist or song title is blank.");
            }
            else
            {
                try
                {
                    await scrobbler.Love(response.UserArtist, response.UserTitle);
                    log("Successfully loved " + response.UserArtist + " - " + response.UserTitle + ".");
                    LovedCurrent = true;
                }
                catch (Exception ex)
                {
                    log("Error loving track: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Attempts to unlove the current track. Assumes the user is logged in.
        /// </summary>
        private async Task unloveTrack(KcrwResponse response)
        {
            if (String.IsNullOrEmpty(response.UserArtist) || String.IsNullOrEmpty(response.UserTitle))
            {
                log("Artist or song title is blank.");
            }
            else
            {
                try
                {
                    await scrobbler.Unlove(response.UserArtist, response.UserTitle);
                    log("Successfully unloved " + response.UserArtist + " - " + response.UserTitle + ".");
                    LovedCurrent = false;
                }
                catch (Exception ex)
                {
                    log("Error unloving track: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Attempts to login to Last.fm.
        /// </summary>
        private async Task attemptLogin()
        {
            try
            {
                if (await scrobbler.Authorize())
                {
                    completeLogin();
                }
                else
                {
                    log("Failed to log in.");
                    loggedIn = false;
                }
            }
            catch (Exception ex)
            {
                log("Failed to log in: " + ex.Message);
                loggedIn = false;
            }
        }

        /// <summary>
        /// Updates the UI for the logged in state. Called by attemptLogin when successful.
        /// </summary>
        private void completeLogin()
        {
            Debug.WriteLine("Logged in to Last.fm as " + scrobbler.Username + ".");
            loggedIn = true;
            linkLabelLogin.Text = scrobbler.Username;
            buttonScrobble.Enabled = true;
            buttonLove.Enabled = true;
            menuItemLogin.Visible = false;
            menuItemLogout.Visible = true;
            menuItemProfile.Visible = true;
        }

        /// <summary>
        /// Logs out from Last.fm, and updates the UI for the logged out state.
        /// </summary>
        private void logout()
        {
            scrobbler.EndSession();
            loggedIn = false;
            Debug.WriteLine("Logged out from Last.fm.");
            linkLabelLogin.Text = "Login to Last.fm";
            buttonScrobble.Enabled = false;
            buttonLove.Enabled = false;
            menuItemLogin.Visible = true;
            menuItemLogout.Visible = false;
            menuItemProfile.Visible = false;
        }

        /// <summary>
        /// Gets or sets whether the current track is loved, and updates the UI.
        /// </summary>
        private bool LovedCurrent
        {
            get
            {
                return lovedCurrent;
            }
            set
            {
                lovedCurrent = value;
                buttonLove.Text = lovedCurrent ? "Unlove track" : "Love track";
            }
        }

        /// <summary>
        /// Invocation delegate for the setButtonStates method.
        /// </summary>
        /// <param name="playEnabled">true if the play button should be enabled; otherwise, false.</param>
        /// <param name="pauseEnabled">true if the pause button should be enabled; otherwise, false.</param>
        /// <param name="stopEnabled">true if the stop button should be enabled; otherwise, false.</param>
        private delegate void SetButtonStatesCallback(bool playEnabled, bool pauseEnabled, bool stopEnabled);

        /// <summary>
        /// Sets the enabled states of the playback buttons.
        /// </summary>
        /// <param name="playEnabled">true if the play button should be enabled; otherwise, false.</param>
        /// <param name="pauseEnabled">true if the pause button should be enabled; otherwise, false.</param>
        /// <param name="stopEnabled">true if the stop button should be enabled; otherwise, false.</param>
        private void setButtonStates(bool playEnabled, bool pauseEnabled, bool stopEnabled)
        {
            if (!exiting)
            {
                if (buttonPlay.InvokeRequired || buttonPause.InvokeRequired || buttonStop.InvokeRequired)
                {
                    SetButtonStatesCallback d = new SetButtonStatesCallback(setButtonStates);
                    Invoke(d, new object[] { playEnabled, pauseEnabled, stopEnabled });
                }
                else
                {
                    buttonPlay.Enabled = playEnabled;
                    buttonPause.Enabled = pauseEnabled;
                    buttonStop.Enabled = stopEnabled;
                } 
            }
        }

        /// <summary>
        /// Invocation delegate for the log method.
        /// </summary>
        /// <param name="msg"></param>
        private delegate void LogCallback(string msg);

        /// <summary>
        /// Writes a message to the log box.
        /// </summary>
        /// <param name="msg">The message to write.</param>
        private void log(string msg)
        {
            if (textBoxLog.InvokeRequired)
            {
                LogCallback d = new LogCallback(log);
                Invoke(d, new object[] { msg });
            }
            else
            {
                textBoxLog.AppendText(msg);
                textBoxLog.AppendText("\n");
            }
        }

        /// <summary>
        /// Deselects the metadata fields.
        /// </summary>
        private void deselect()
        {
            textBoxArtist.SelectionStart = 0;
            textBoxArtist.SelectionLength = 0;
            textBoxTitle.SelectionStart = 0;
            textBoxTitle.SelectionLength = 0;
            textBoxAlbum.SelectionStart = 0;
            textBoxAlbum.SelectionLength = 0;
        }

        #region Form events

        private void SkcrwblrForm_MouseDown(object sender, MouseEventArgs e)
        {
            deselect();
            if (textBoxArtist.Focused || textBoxTitle.Focused || textBoxAlbum.Focused)
            {
                buttonScrobble.Focus();
            }
        }

        private void timerGetTrack_Tick(object sender, EventArgs e)
        {
            updateTracklist();
        }

        private void timerUpdateTime_Tick(object sender, EventArgs e)
        {
            if (SelectedTrack == LastTrack)
            {
                updateTime(SelectedTrack.ParsedDateTime);
            }
        }

        private async void linkLabelLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (loggedIn)
            {
                contextMenuLastFm.Show(linkLabelLogin, linkLabelLogin.PointToClient(Cursor.Position));
            }
            else
            {
                await attemptLogin();
            }
        }

        private async void linkLabelArtist_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var url = (await scrobbler.GetArtist(textBoxArtist.Text)).url;
                Uri uri = null;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    Process.Start(url);
                }
            }
            catch
            {
                // ok
            }
        }

        private async void linkLabelTitle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var url = (await scrobbler.GetInfo(textBoxArtist.Text, textBoxTitle.Text)).url;
                Uri uri = null;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    Process.Start(url);
                }
            }
            catch
            {
                // ok
            }
        }

        private async void linkLabelAlbum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var url = (await scrobbler.GetAlbum(textBoxArtist.Text, textBoxAlbum.Text)).url;
                Uri uri = null;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    Process.Start(url);
                }
            }
            catch
            {
                // ok
            }
        }

        private void textBoxArtist_Enter(object sender, EventArgs e)
        {
            //artist.BackColor = SystemColors.Window;
        }

        private async void textBoxArtist_Leave(object sender, EventArgs e)
        {
            if (textChanged)
            {
                await findCorrection(SelectedTrack);
                textChanged = false;
                populateFields(SelectedTrack);
            }
            if (SelectedTrack == LastTrack)
            {
                updateNowPlaying();
            }
        }

        private void textBoxArtist_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
            SelectedTrack.UserArtist = textBoxArtist.Text;
        }

        private void textBoxTitle_Enter(object sender, EventArgs e)
        {
            //title.BackColor = SystemColors.Window;
        }

        private async void textBoxTitle_Leave(object sender, EventArgs e)
        {
            if (textChanged)
            {
                await findCorrection(SelectedTrack);
                textChanged = false;
                populateFields(SelectedTrack);
            }
            if (SelectedTrack == LastTrack)
            {
                updateNowPlaying();
            }
        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
            SelectedTrack.UserTitle = textBoxTitle.Text;
        }

        private void textBoxAlbum_Enter(object sender, EventArgs e)
        {
            //album.BackColor = SystemColors.Window;
        }

        private void textBoxAlbum_Leave(object sender, EventArgs e)
        {
            // don't call findCorrection because changing the album will not affect the results
        }

        private void textBoxAlbum_TextChanged(object sender, EventArgs e)
        {
            SelectedTrack.UserAlbum = textBoxAlbum.Text;
        }

        private async void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (selectedNode.Previous != null)
            {
                selectedNode = selectedNode.Previous;
                if (!SelectedTrack.LastFmFound)
                {
                    await findCorrection(SelectedTrack);
                }
                populateFields(SelectedTrack);
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (selectedNode.Next != null)
            {
                selectedNode = selectedNode.Next;
                populateFields(SelectedTrack);
            }
        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            selectedNode = ((Stream)comboBoxStream.SelectedItem).Tracklist.Last;
            populateFields(SelectedTrack);
        }

        private async void buttonCorrectSpelling_Click(object sender, EventArgs e)
        {
            if (!SelectedTrack.LastFmFound)
            {
                await findCorrection(SelectedTrack);
                textChanged = false;
                populateFields(SelectedTrack);
            }
            useCorrectSpelling(SelectedTrack);
            if (SelectedTrack == LastTrack)
            {
                updateNowPlaying();
            }
        }

        private void buttonOriginalSpelling_Click(object sender, EventArgs e)
        {
            textBoxAlbum.Text = SelectedTrack.Album;
            textBoxTitle.Text = SelectedTrack.Title;
            textBoxArtist.Text = SelectedTrack.Artist;
            colorize(SelectedTrack);
        }

        private void checkBoxAutoCorrect_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoCorrect.Checked)
            {
                useCorrectSpelling(SelectedTrack);
            }
        }

        private async void buttonScrobble_Click(object sender, EventArgs e)
        {
            if (loggedIn)
            {
                await scrobbleTrack(SelectedTrack);
            }
        }

        private async void buttonLove_Click(object sender, EventArgs e)
        {
            if (loggedIn)
            {
                if (!LovedCurrent)
                {
                    await loveTrack(SelectedTrack);
                }
                else
                {
                    await unloveTrack(SelectedTrack);
                }
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            streamer.Play();
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            streamer.Pause();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            streamer.Stop();
        }

        private void volumeControl_VolumeChanged(object sender, EventArgs e)
        {
            streamer.UpdateVolume();
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private async void menuItemLogin_Click(object sender, EventArgs e)
        {
            await attemptLogin();
        }

        private void menuItemLogout_Click(object sender, EventArgs e)
        {
            logout();
        }

        private void menuItemProfile_Click(object sender, EventArgs e)
        {
            Process.Start("http://last.fm/user/" + scrobbler.Username);
        }

        private void menuItemClear_Click(object sender, EventArgs e)
        {
            textBoxLog.Clear();
        }

        private void menuItemCopy_Click(object sender, EventArgs e)
        {
            textBoxLog.Copy();
        }

        private void menuItemSelectAll_Click(object sender, EventArgs e)
        {
            textBoxLog.SelectAll();
        }

        private void contextMenuLog_Popup(object sender, EventArgs e)
        {
            menuItemClear.Enabled = textBoxLog.TextLength != 0;
            menuItemCopy.Enabled = textBoxLog.SelectionLength != 0;
            menuItemSelectAll.Enabled = textBoxLog.TextLength != 0;
        }

        private void comboBoxStream_SelectedIndexChanged(object sender, EventArgs e)
        {
            streamer.Stop();
            streamer.StreamUrl = ((Stream)comboBoxStream.SelectedItem).StreamUrl;
            updateTracklist(false);
        }

        private void Skcrwblr_FormClosing(object sender, FormClosingEventArgs e)
        {
            writeRegistry();
            exiting = true;
            streamer.Stop();
        }

        #endregion

        /// <summary>
        /// Represents streams available for playback and track fetching.
        /// </summary>
        public class Stream
        {
            public string Title { get; set; }
            public string StreamUrl { get; set; }
            public Tracklist Tracklist { get; set; }

            public Stream(string title, string streamUrl, Tracklist tracklist)
            {
                Title = title;
                StreamUrl = streamUrl;
                Tracklist = tracklist;
            }
        }
    }
}
