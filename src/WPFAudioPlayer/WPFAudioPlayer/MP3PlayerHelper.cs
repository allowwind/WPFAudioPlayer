using NAudio.Wave;
using System;


namespace WPFAudioPlayer.LIstenBack
{
    public delegate void DelegateOnPlayStop();
    public class MP3PlayerHelper : IDisposable
    {
        public event DelegateOnPlayStop OnPlayStop;
        private bool isrun = true;
        MediaFoundationReader mf = null;
        private WaveStream wavStream = null;
        private BlockAlignReductionStream baStream = null;
        private WaveOutEvent waveOut;


        public void Stop()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (baStream != null)
            {
                baStream.Dispose();
                baStream = null;
            }
            if (wavStream != null)
            {
                wavStream.Dispose();
                wavStream = null;
            }

            if (mf != null)
            {
                mf.Close();
                mf.Dispose();
                mf = null;
            }
        }
        public void Dispose()
        {
            Stop();
        }

        public double GetPositionInSeconds()
        {
            if (wavStream != null)
            {
                return wavStream.CurrentTime.TotalSeconds;
            }
            else
            {
                return 0;
            }
        }

        public double AudioLength = 0;
        public bool InitURL(string url)
        {
            Stop();
            mf = new MediaFoundationReader(url);
            {
                wavStream = WaveFormatConversionStream.CreatePcmStream(mf);
                AudioLength = wavStream.TotalTime.TotalSeconds;
                baStream = new BlockAlignReductionStream(wavStream);
                waveOut = new WaveOutEvent();
                waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
                waveOut.Init(baStream);
                return true;
            }
        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            OnPlayStop?.Invoke();
        }
        /// <summary>
        /// 设置值并且播放
        /// </summary>
        /// <param name="val"></param>
        /// <param name="play"></param>
        public void SetValue(double val, bool play = false)
        {
            if (val >= 0 && val <= AudioLength && waveOut != null && wavStream != null)
            {
                wavStream?.SetPosition(TimeSpan.FromSeconds(val));
                if (play)
                {
                    //if (waveOut.PlaybackState != PlaybackState.Playing)
                    //{
                    waveOut.Play();
                    //}
                }
                else
                {
                    waveOut.Stop();
                }
            }
        }
    }
}
