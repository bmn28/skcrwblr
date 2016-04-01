// Code adapted from NAudio source code. Used under the Microsoft Public License.
// https://naudio.codeplex.com/SourceControl/latest#NAudioDemo/Mp3StreamingDemo/MP3StreamingPanel.cs

using NAudio.Wave;
using System;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Skcrwblr
{
    public class Mp3Streamer
    {
        private BufferedWaveProvider bufferedWaveProvider;
        private IWavePlayer waveOut;
        private volatile StreamingPlaybackState playbackState;
        private volatile bool fullyDownloaded;
        private HttpWebRequest webRequest;
        private VolumeWaveProvider16 volumeProvider;
        private System.Timers.Timer timer;
        public GetVolume getVolume = (() => 1.0f); // default volume setting

        public Mp3Streamer(string url = "")
        {
            StreamUrl = url;
            timer = new System.Timers.Timer(250);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Tick);
            timer.Enabled = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
                timer.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public delegate float GetVolume();
        public delegate void StreamError(string message);
        public delegate void StreamingPlaybackStateChanged();

        public event StreamError Error;
        public event StreamingPlaybackStateChanged Buffering;
        public event StreamingPlaybackStateChanged Paused;
        public event StreamingPlaybackStateChanged Playing;
        public event StreamingPlaybackStateChanged Stopped;

        public enum StreamingPlaybackState
        {
            Stopped,
            Playing,
            Buffering,
            Paused
        }

        private bool IsBufferNearlyFull
        {
            get
            {
                return bufferedWaveProvider != null &&
                       bufferedWaveProvider.BufferLength - bufferedWaveProvider.BufferedBytes
                       < bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4;
            }
        }

        public StreamingPlaybackState PlaybackState
        {
            get
            {
                return playbackState;
            }
            private set
            {
                if (value != playbackState)
                {
                    switch (value)
                    {
                        case StreamingPlaybackState.Buffering:
                            Buffering();
                            break;
                        case StreamingPlaybackState.Paused:
                            Paused();
                            break;
                        case StreamingPlaybackState.Playing:
                            Playing();
                            break;
                        case StreamingPlaybackState.Stopped:
                            Stopped();
                            break;
                        default:
                            break;
                    }
                }
                playbackState = value;
            }
        }

        public string StreamUrl { get; set; }

        private static IMp3FrameDecompressor CreateFrameDecompressor(Mp3Frame frame)
        {
            WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                frame.FrameLength, frame.BitRate);
            return new AcmMp3FrameDecompressor(waveFormat);
        }

        private IWavePlayer CreateWaveOut()
        {
            return new WaveOut();
        }

        private void StreamMp3(object state)
        {
            fullyDownloaded = false;
            var url = (string)state;
            webRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException e)
            {
                Stop();
                if (e.Status != WebExceptionStatus.RequestCanceled) Error(e.Message);
                return;
            }
            var buffer = new byte[16384 * 4];

            IMp3FrameDecompressor decompressor = null;

            try
            {
                using (var responseStream = resp.GetResponseStream())
                {
                    var readFullyStream = new ReadFullyStream(responseStream);
                    do
                    {
                        if (IsBufferNearlyFull)
                        {
                            Debug.WriteLine("Buffer getting full, taking a break");
                            Thread.Sleep(500);
                        }
                        else
                        {
                            Mp3Frame frame;
                            try
                            {
                                frame = Mp3Frame.LoadFromStream(readFullyStream);
                            }
                            catch (EndOfStreamException)
                            {
                                fullyDownloaded = true;
                                // reached the end of the MP3 file / stream
                                break;
                            }
                            catch (WebException)
                            {
                                // probably we have aborted download from the GUI thread
                                break;
                            }
                            if (decompressor == null)
                            {
                                // don't think these details matter too much - just help ACM select the right codec
                                // however, the buffered provider doesn't know what sample rate it is working at
                                // until we have a frame
                                decompressor = CreateFrameDecompressor(frame);
                                bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat);
                                bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(20); // allow us to get well ahead of ourselves
                                //this.bufferedWaveProvider.BufferedDuration = 250;
                            }
                            int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                            //Debug.WriteLine(String.Format("Decompressed a frame {0}", decompressed));
                            bufferedWaveProvider.AddSamples(buffer, 0, decompressed);
                        }

                    } while (PlaybackState != StreamingPlaybackState.Stopped);
                    Stop();
                    Debug.WriteLine("Exiting");
                    // was doing this in a finally block, but for some reason
                    // we are hanging on response stream .Dispose so never get there
                    decompressor.Dispose();
                }
            }
            finally
            {
                if (decompressor != null)
                {
                    decompressor.Dispose();
                }
            }
        }

        private void Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            if (PlaybackState != StreamingPlaybackState.Stopped)
            {
                if (waveOut == null && bufferedWaveProvider != null)
                {
                    Debug.WriteLine("Creating WaveOut Device");
                    waveOut = CreateWaveOut();
                    waveOut.PlaybackStopped += OnPlaybackStopped;
                    volumeProvider = new VolumeWaveProvider16(bufferedWaveProvider);
                    UpdateVolume();
                    waveOut.Init(volumeProvider);
                }
                else if (bufferedWaveProvider != null)
                {
                    var bufferedSeconds = bufferedWaveProvider.BufferedDuration.TotalSeconds;
                    // make it stutter less if we buffer up a decent amount before playing
                    if (bufferedSeconds < 0.5 && PlaybackState == StreamingPlaybackState.Playing && !fullyDownloaded)
                    {
                        Buffer();
                    }
                    else if (bufferedSeconds > 4 && PlaybackState == StreamingPlaybackState.Buffering)
                    {
                        Continue();
                    }
                    else if (fullyDownloaded && bufferedSeconds == 0)
                    {
                        Debug.WriteLine("Reached end of stream");
                        Stop();
                    }
                }
            }
        }

        private void Continue()
        {
            waveOut.Play();
            Debug.WriteLine(String.Format("Started playing, waveOut.PlaybackState={0}", waveOut.PlaybackState));
            PlaybackState = StreamingPlaybackState.Playing;
        }

        public void Play()
        {
            if (PlaybackState == StreamingPlaybackState.Stopped)
            {
                PlaybackState = StreamingPlaybackState.Buffering;
                bufferedWaveProvider = null;
                ThreadPool.QueueUserWorkItem(StreamMp3, StreamUrl);
                timer.Enabled = true;
            }
            else if (PlaybackState == StreamingPlaybackState.Paused)
            {
                PlaybackState = StreamingPlaybackState.Buffering;
            }
        }

        public void Pause()
        {
            if (PlaybackState == StreamingPlaybackState.Playing || PlaybackState == StreamingPlaybackState.Buffering)
            {
                waveOut.Pause();
                Debug.WriteLine(String.Format("User requested Pause, waveOut.PlaybackState={0}", waveOut.PlaybackState));
                PlaybackState = StreamingPlaybackState.Paused;
            }
        }

        private void Buffer()
        {
            PlaybackState = StreamingPlaybackState.Buffering;
            waveOut.Pause();
            Debug.WriteLine(String.Format("Paused to buffer, waveOut.PlaybackState={0}", waveOut.PlaybackState));
        }

        public void Stop()
        {
            if (PlaybackState != StreamingPlaybackState.Stopped)
            {
                if (!fullyDownloaded)
                {
                    webRequest.Abort();
                }

                PlaybackState = StreamingPlaybackState.Stopped;
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }
                timer.Enabled = false;
                // n.b. streaming thread may not yet have exited
                Thread.Sleep(500);
                // ShowBufferState(0);
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            Debug.WriteLine("Playback Stopped");
            if (e.Exception != null)
            {
                //MessageBox.Show(String.Format("Playback Error {0}", e.Exception.Message));
            }
        }

        public void UpdateVolume()
        {
            if (volumeProvider != null) volumeProvider.Volume = getVolume();
        }
    }
}