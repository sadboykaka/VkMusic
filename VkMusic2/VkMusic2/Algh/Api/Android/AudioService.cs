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

    [Serializable]
    class AudioServiceInfo
    {
        public string Name;
        public string Author;
        public string Cover;
        public bool Play;
        public bool Repeat;

    }


    [Service (Name = "com.cfplayer.audioservice")]
    [IntentFilter(new[] { ActionPlayPause, ActionRepeat, ActionStop })]
    public class AudioService : Service
    {
        //Actions
        public const string ActionRepeat = "cfplayer.service.repeat";
        public const string ActionPlayPause = "cfplayer.service.playpause";
        public const string ActionStop = "cfplayer.service.stop";

        public const string Request = "req_val";

        private AudioServiceInfo cur;

        public override void OnDestroy()
        {
            StopForeground(true);
            if (!VkMusic2.Droid.MainActivity.open) Process.KillProcess(Process.MyPid());
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

        
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            switch (intent.Action)
            {
                case ActionPlayPause:
                    var res = intent.GetByteArrayExtra(Request);
                    if (res == null) break;
                    cur = (AudioServiceInfo)ByteArrayToObject(res);
                    Notification();
                    break;
                case ActionRepeat:
                    if (cur == null) break;
                    cur.Repeat = intent.GetBooleanExtra(Request, false);
                    Notification();
                    break;
                case ActionStop:
                    StopSelf();
                    break;
            }
            return StartCommandResult.NotSticky;
        }

       

        private void Notification()
        {
            if (cur == null) return;
            StartForeground(1, AndroidPlayerNotification.Get(cur.Name, cur.Author, cur.Cover, cur.Play, cur.Repeat));
        }

       

        //---------------------------------------------------------------------------------
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }

}
