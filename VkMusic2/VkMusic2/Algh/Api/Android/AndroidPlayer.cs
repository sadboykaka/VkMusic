using System;
using System.Collections.Generic;
using System.Text;
using Android.Media;
using Android;
using Algh.interfaces;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Timers;

namespace Algh.Api
{
    public class MyAudioFocusListener : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener
    {
        public event EventHandler<bool> FocusChanged;

        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    FocusChanged(this, true);
                    break;
                case AudioFocus.LossTransient:
                    FocusChanged(this, false);
                    break;
                case AudioFocus.Loss:
                    FocusChanged(this, false);
                    break;
            }
        }
    }

    class AndroidPlayer : IPlayer
    {
        private MediaPlayer Player = new MediaPlayer();

        private MyAudioFocusListener myAudioFocusListener = new MyAudioFocusListener();

        private Timer Timer = new Timer();

        public IEqualizer Equalizer { get; }

        private const int UpdateInterval = 500;

        private bool awaittoresume;

        private bool repeat = false;

        public bool Repeat
        {
            get { return repeat; }
            set {
                repeat = value;
                if (value) SeekEvent(this, -1);
                else SeekEvent(this, -2);
                SendBroadCastBool(AudioService.ActionRepeat, AudioService.Request, value);
            }
        }

        public ITrackList TrackList { get; private set; }

        public event EventHandler<bool> PlayEvent;
        public event EventHandler<int> SeekEvent;

        public AndroidPlayer(int size)
        {
            Player.Prepared += (object sender, EventArgs e) => { IsPlay = true; };
            TrackList = new AndroidTrackList(size);
            awaittoresume = false;
            myAudioFocusListener.FocusChanged += FocusEvent;
            Timer.Interval = UpdateInterval;
            Timer.Elapsed += TimeEvent;
            Equalizer = new AndroidEqualizer(Player, HttpUser.Cookies.appdir);
            Player.Completion += (i, e) => {
                if (!Repeat) PlayNext();
                else
                {
                    Player.SeekTo(0);
                    IsPlay = true;
                }
            };

            MessageReplyReceiver.PlayPauseEvent += BPlayEvent;
            MessageReplyReceiver.NextPrevEvent += BNextPrevEvent;
            MessageReplyReceiver.RepeatEvent += BRepeatEvent;

        }
        //-------------------------------------------------
        private void BPlayEvent(object o, bool b)
        {
            IsPlay = b;
        }

        private void BRepeatEvent(object o, bool b)
        {
            Repeat = b;
        }

        private void BNextPrevEvent(object o, bool b)
        {
            if (b) PlayNext();
            else PlayPrev();
        }
        //-------------------------------------------------
        ~AndroidPlayer()
        {
            IsPlay = false;
        }

        private void SendBroadCastBool(string action, string req, bool f)
        {
            Xamarin.Forms.Forms.Context.StartService(new Android.Content.Intent(Xamarin.Forms.Forms.Context, typeof(AudioService))
                .SetAction(action)
                    .PutExtra(req, f));
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null) return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private void SendBroadCastInfo()
        {
            var tr = TrackList[TrackList.Curtl][TrackList.Curid];
            Xamarin.Forms.Forms.Context.StartService(new Android.Content.Intent(Xamarin.Forms.Forms.Context, typeof(AudioService))
                .SetAction(AudioService.ActionPlayPause)
                    .PutExtra(AudioService.Request, ObjectToByteArray(new AudioServiceInfo { Author =  tr.Author, Name = tr.Name, Cover = tr.Cover, Play = IsPlay, Repeat = repeat} )));
        }

        private void FocusEvent(object sender, bool b)
        {
            if (b)
            {
                if (awaittoresume && !IsPlay)
                {
                    awaittoresume = false;
                    IsPlay = true;
                }
            }
            else
            {
                if (IsPlay)
                {
                    awaittoresume = true;
                    IsPlay = false;
                }
            }
           
        }

        public int Seek {
            get { return Player.CurrentPosition; }
            set { if (value <= Player.Duration) Player.SeekTo(value); }
        }

        public int Duration
        {
            get { return Player.Duration; }
        }

        public bool IsPlay
        {
            get { return Player.IsPlaying; }
            private set { Action(value); }
        }

        private void Action(bool f)
        {
            if (TrackList[TrackList.Curtl] == null) return;
            if (f)
            {
                var am = (AudioManager)Xamarin.Forms.Forms.Context.GetSystemService(Android.Content.Context.AudioService);
                if (am.RequestAudioFocus(myAudioFocusListener, Android.Media.Stream.Music, AudioFocus.Gain) != AudioFocusRequest.Granted) return;
                Player.Start();
                Timer.Start();
            }
            else
            {
                if(!awaittoresume) ((AudioManager)Xamarin.Forms.Forms.Context.GetSystemService(Android.Content.Context.AudioService)).AbandonAudioFocus(myAudioFocusListener);
                Timer.Stop();
                Player.Pause();
                
            }
            SendBroadCastInfo();
            PlayEvent(this, f);
        }

        private void TimeEvent(object sender, ElapsedEventArgs e)
        {
            SeekEvent(this, Seek);
        }

        public void Pause()
        {
            IsPlay = false;
        }

        public void PauseOrResume()
        {
            if (IsPlay) IsPlay = false;
            else IsPlay = true;
        }

        /// <summary>
        /// Play Track
        /// </summary>
        /// <param name="id">id of track</param>
        /// <param name="tlid">id of tracklist</param>
        public void Play(int id, int tlid)
        {
            if (TrackList.Curtl == tlid && TrackList.Curid == id)
            {
                PauseOrResume();
                return;
            }
            TrackList.Curtl = tlid;
            TrackList.Curid = id;
            PlayCur();
        }

        private void PlayCur()
        {
            //IsPlay = false;
            Player.Reset();
            var tl = TrackList[TrackList.Curtl];
            if (tl == null) return;
            Player.SetDataSource(tl[TrackList.Curid].Url);
            Player.PrepareAsync();
        }

        public void PlayNext()
        {
           TrackList.Curid++;
           PlayCur();
        }

        public void PlayPrev()
        {
            TrackList.Curid--;
            PlayCur();
        }

        public void Stop()
        {
            if (!IsPlay) return;
            IsPlay = false;
            Player.Stop();
            TrackList.Curtl = -1;
        }
    }
}
