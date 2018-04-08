using System;
using System.Collections.Generic;
using System.Text;
using Android.Media;
using Android;
using Algh.interfaces;
using System.Collections.ObjectModel;
using NotificationCompat = Android.Support.V4.App.NotificationCompat;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Algh.Api
{
    class AndroidAudioServicePlayer : interfaces.IPlayer
    {
        private int seek = 0;
        private int curid = 0;
        private int curdur = 0;
        public int Seek
        {
            get { return seek; } 
            set
            {
                Xamarin.Forms.Forms.Context.StartService(new Android.Content.Intent(Xamarin.Forms.Forms.Context, typeof(AudioService))
                .SetAction(AudioService.ActionSeekTo)
                    .PutExtra(AudioService.REQ_SEEKTO, value));
            }
        }

        

        public int Duration { get { return curdur; } }

        public ITrackList TrackList { get; private set; }

        public bool IsPlay { get; private set; }

        public event EventHandler<bool> PlayEvent;

        public event EventHandler<int> SeekEvent;

        public void Pause()
        {
            Xamarin.Forms.Forms.Context.StartService(new Android.Content.Intent(new Android.Content.Intent(Xamarin.Forms.Forms.Context, typeof(AudioService))
                .SetAction(AudioService.ActionPause)));
        }

        public void PauseOrResume()
        {
            Xamarin.Forms.Forms.Context.StartService(new Android.Content.Intent(Xamarin.Forms.Forms.Context, typeof(AudioService))
                .SetAction(AudioService.ActionPlayPause));
        }

        private void mrPlayEvent(object o, (bool, int, int) i)
        {
            IsPlay = i.Item1;
            if (!i.Item1)
            {
                PlayEvent(this, false);
            }
            else
            {
                curid = i.Item2;
                curdur = i.Item3;
                PlayEvent(this, true);
            }
        }

        private void mrSeekEvent(object o, int i)
        {
            seek = i;
            SeekEvent(this,i);
        }

        public AndroidAudioServicePlayer(int TrackListsCount)
        {
            MessageReplyReceiver.PlayEvent += mrPlayEvent;
            MessageReplyReceiver.SeekEvent += mrSeekEvent;
            TrackList = new AndroidTrackList(TrackListsCount);
        }

        ~AndroidAudioServicePlayer()
        {
            MessageReplyReceiver.PlayEvent -= mrPlayEvent;
            MessageReplyReceiver.SeekEvent -= mrSeekEvent;
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

        public void Play(int id, int tlid)
        {
            IsPlay = false;
            if (id == TrackList.Curid && tlid == TrackList.Curtl)
            {
                PauseOrResume();
                return;
            }
            var a = ObjectToByteArray(new AudioServiceInfo { CurTl = tlid, CurId = id });
            //var b = new Android.Content.Intent(AudioService.ActionNewCol).PutExtra(AudioService.REQ_TrackList, a);
            var b = new Android.Content.Intent(Xamarin.Forms.Forms.Context, typeof(AudioService))
                .SetAction(AudioService.ActionNewCol)
                .PutExtra(AudioService.REQ_TrackList,a);
            Xamarin.Forms.Forms.Context.StartService(b);
            
        }

        public void PlayNext()
        {
            Xamarin.Forms.Forms.Context.StartService(new Android.Content.Intent(Xamarin.Forms.Forms.Context, typeof(AudioService))
                .SetAction(AudioService.ActionNext));
        }

        public void PlayPrev()
        {
            Xamarin.Forms.Forms.Context.StartService(new Android.Content.Intent(Xamarin.Forms.Forms.Context, typeof(AudioService))
                .SetAction(AudioService.ActionPrev));
        }

        public void Stop()
        {
            Xamarin.Forms.Forms.Context.StartService(new Android.Content.Intent(Xamarin.Forms.Forms.Context, typeof(AudioService))
                .SetAction(AudioService.ActionStop));
        }
    }
}
