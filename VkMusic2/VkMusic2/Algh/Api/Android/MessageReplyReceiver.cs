using System;
using System.Collections.Generic;
using System.Text;
using Android.Media;
using Android;
using Algh.interfaces;
using System.Collections.ObjectModel;
using NotificationCompat = Android.Support.V4.App.NotificationCompat;
using Android.App;
using Android.Content;
using VkMusic2.Droid;

namespace Algh.Api
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Action_play, Action_pause, Action_seek, AudioManager.ActionAudioBecomingNoisy })]
    public class MessageReplyReceiver : BroadcastReceiver
    {
        public const string PLAYER_ACTION = "cfplayer.action.reply";
        public const string REQUEST_CODE = "req_code";
        public const string REQUEST_SEEK = "req_seek";
        public const string REQUEST_DURATION = "req_duration";
        public const string REQUEST_CURID = "req_curid";
        //-----------------------------------------------------------
        public const string Action_play = "cfplayer.action.play";
        public const string Action_pause = "cfplayer.action.pause";
        public const string Action_seek = "cfplayer.action.seek";

        public static EventHandler<int> SeekEvent;
        public static EventHandler<(bool, int, int)> PlayEvent; // playpause, id, dur

        public override void OnReceive(Context context, Intent intent)
        {
            switch (intent.Action)
            {
                case Action_play:
                    int id = intent.GetIntExtra(REQUEST_CURID, -1);
                    int dur = intent.GetIntExtra(REQUEST_DURATION, -1);
                    if (id == -1 || dur == -1) break;
                    PlayEvent(this, (true, id, dur));
                    break;
                case Action_pause:
                    PlayEvent(this, (false, -1, -1));
                    break;
                case Action_seek:
                    int seek = intent.GetIntExtra(REQUEST_SEEK, -1);
                    if (seek == -1) break;
                    SeekEvent(this, seek);
                    break;
                case AudioManager.ActionAudioBecomingNoisy:
                    context.StartService(new Intent(AudioService.ActionPause));
                    break;
            }


            /*int res = intent.GetIntExtra(REQUEST_CODE, -1);
            if (res == -1) return;
            if (res == 0) AndroidPlayerNotification.Player.PauseOrResume();
            if (res == 1) AndroidPlayerNotification.Player.PlayPrev();
            if (res == 2) AndroidPlayerNotification.Player.PlayNext();*/
        }

    }
}
