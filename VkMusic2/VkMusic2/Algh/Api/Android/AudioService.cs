using Android.App;
using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Media;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Android.Widget;

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
                    FocusChanged(this, false); // Ошибка
                    break;
            }
        }
    }

    [Serializable]
    class AudioServiceInfo
    {
        public int CurTl;
        public int CurId;
    }


    [Service (Name = "com.cfplayer.audioservice")]
    [IntentFilter(new[] { ActionNewCol, ActionPlay, ActionPlayPause, ActionNext, ActionPrev, ActionSeekTo, ActionPause, ActionClose, ActionStop })]
    public class AudioService : Service
    {
        private Algh.interfaces.ITrackList TrackList = VkMusic2.Collection.Player.TrackList;

        private MediaPlayer Player;

        private MyAudioFocusListener myAudioFocusListener = new MyAudioFocusListener();

        System.Timers.Timer timer = new System.Timers.Timer();

        //Actions
        public const string ActionSeekTo = "cfplayer.service.seekto";
        public const string ActionPlay = "cfplayer.service.play";
        public const string ActionNewCol = "cfplayer.service.newcol";
        public const string ActionPlayPause = "cfplayer.service.playpause";
        public const string ActionPause = "cfplayer.service.pause";
        public const string ActionNext = "cfplayer.service.next";
        public const string ActionPrev = "cfplayer.service.prev";
        public const string ActionClose = "cfplayer.service.close";
        public const string ActionStop = "cfplayer.service.stop";

        private AudioServiceInfo Info;

        private bool awaittoresume = false;

        int UpdateInterval = 500;

        void FocusEvent(object sender, bool b) 
        {
            if (b)
            {
                if (awaittoresume && !Player.IsPlaying)
                {
                    awaittoresume = false;
                    Start();
                }
            }
            else
            {
                if (Player.IsPlaying)
                {
                    Pause(false);
                }
            }
        }


        public override void OnCreate()
        {
            Player = new MediaPlayer();
            Player.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);
            Player.SetAudioStreamType(Android.Media.Stream.Music);
            timer.Interval = UpdateInterval;
            timer.Elapsed+= TimeEvent;

            Player.Completion += (i, e) => {PlayNext();};

            myAudioFocusListener.FocusChanged += FocusEvent;

        }

        public override void OnDestroy()
        {
            PreDestroy();
            if(!VkMusic2.Droid.MainActivity.open) Process.KillProcess(Process.MyPid());
            }

        public void PreDestroy()
        {
            TrackList = null;

            StopForeground(true);
            timer.Elapsed -= TimeEvent;
            timer.Stop();
            timer.Close();
            timer.Dispose();
            timer = null;

            Player.Stop();
            Player.Release();
            Player.Dispose();
            Player = null;


            myAudioFocusListener.FocusChanged -= FocusEvent;
            ((AudioManager)Xamarin.Forms.Forms.Context.GetSystemService(Android.Content.Context.AudioService)).AbandonAudioFocus(myAudioFocusListener);
            myAudioFocusListener.Dispose();
            myAudioFocusListener = null;

            Info = null;
            TrackList = null;
        }

        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }

        public const string REQ_TrackList = "req_tl";
        public const string REQ_Track = "req_tid";
        public const string REQ_SEEKTO = "req_seekto";
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            switch (intent.Action)
            {
                case ActionNewCol:
                    var a = intent.GetByteArrayExtra(REQ_TrackList);
                    Info = (AudioServiceInfo)ByteArrayToObject(a);
                    Play();
                    break;
                case ActionPlay:
                    int id = intent.GetIntExtra(REQ_Track, -1);
                    if (id < 0 || id >= TrackList[Info.CurTl].Count) break;
                    Info.CurId = id;
                    Play();
                    break;
                case ActionNext:
                    PlayNext(); 
                    break;
                case ActionPrev:
                    if (Info == null) break;
                    if (TrackList[Info.CurTl].Count == 0) break;
                    if (Info.CurId <= 0) Info.CurId = TrackList[Info.CurTl].Count - 1;
                    else Info.CurId--;
                    Play();
                    break;
                case ActionPlayPause:
                    PlayPause();
                    break;
                case ActionSeekTo:

                    int seek = intent.GetIntExtra(REQ_SEEKTO, -1);
                    if (seek == -1 || seek > Player.Duration) break;
                    Player.SeekTo(seek);
                    break;
                case ActionPause:
                    if (Player.IsPlaying) Pause(true);
                    break;
                case ActionClose:
                    StopSelf();
                    break;
                case ActionStop:
                    Player.Stop();
                    ((AudioManager)Xamarin.Forms.Forms.Context.GetSystemService(Android.Content.Context.AudioService)).AbandonAudioFocus(myAudioFocusListener);
                    break;
            }
            return StartCommandResult.NotSticky;
        }

        private void PlayNext()
        {
            if (Info == null) return;
            if (TrackList[Info.CurTl].Count == 0) return;
            if (TrackList[Info.CurTl].Count <= Info.CurId + 1) Info.CurId = 0;
            else Info.CurId++;
            Play();
        }

        private void PlayPause()
        {
            if (Player.IsPlaying) Pause(true);
            else Start();
        }

        private void Play()
        {
            try
            {
                Player.Reset();
                Player.SetDataSource(TrackList[Info.CurTl][Info.CurId].Url);
                Player.PrepareAsync();
                Player.Prepared += (object sender, EventArgs e) => { Start(); };
            }
            catch
            {

            }
            finally
            {
                TrackList.Curid = Info.CurId;
                TrackList.Curtl = Info.CurTl;
            }
        }
        //---------------------------------------------------------------------------------
        Intent PauseIntent = new Intent(MessageReplyReceiver.Action_pause);

        void Start()
        {
            var am = (AudioManager)Xamarin.Forms.Forms.Context.GetSystemService(Android.Content.Context.AudioService);
            if (am.RequestAudioFocus(myAudioFocusListener, Android.Media.Stream.Music, AudioFocus.Gain) == AudioFocusRequest.Granted)
            {
                if (Info == null) return;
                var curt = TrackList[Info.CurTl][Info.CurId];
                Player.Start();
                StartForeground(1, AndroidPlayerNotification.Get(curt.Name, curt.Author, true));
                SendBroadcast(new Intent(MessageReplyReceiver.Action_play)
                    .PutExtra(MessageReplyReceiver.REQUEST_DURATION, Player.Duration)
                    .PutExtra(MessageReplyReceiver.REQUEST_CURID, Info.CurId));
                timer.Start();
            }
        }

        void Pause(bool f)
        {
            var curt = TrackList[Info.CurTl][Info.CurId];
            StartForeground(1,AndroidPlayerNotification.Get(curt.Name, curt.Author, false));
            Player.Pause();
            SendBroadcast(PauseIntent);
            timer.Stop();
            if (f)
            {
               ((AudioManager)Xamarin.Forms.Forms.Context.GetSystemService(Android.Content.Context.AudioService)).AbandonAudioFocus(myAudioFocusListener);
            }
            else awaittoresume = true;
        }

        private void TimeEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendBroadcast(new Intent(MessageReplyReceiver.Action_seek).PutExtra(MessageReplyReceiver.REQUEST_SEEK, Player.CurrentPosition));
        }

        //---------------------------------------------------------------------------------
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }

}
