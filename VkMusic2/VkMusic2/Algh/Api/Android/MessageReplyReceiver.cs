using System;
using Android.Media;
using Android.App;
using Android.Content;

namespace Algh.Api
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { ActionPlay, ActionRepeat, ActionNextPrev, AudioManager.ActionAudioBecomingNoisy })]
    public class MessageReplyReceiver : BroadcastReceiver
    {
        
        public const string ActionPlay = "cfplayer.action.playpause";
        public const string ActionNextPrev = "cfplayer.action.nextprev";
        public const string ActionRepeat = "cfplayer.action.rep";

        public const string Request = "request";

        public static EventHandler<bool> NextPrevEvent;
        public static EventHandler<bool> PlayPauseEvent; 
        public static EventHandler<bool> RepeatEvent;

        public override void OnReceive(Context context, Intent intent)
        {
            bool res = intent.GetBooleanExtra(Request, false);
            switch (intent.Action)
            {
                case ActionPlay:
                    PlayPauseEvent(this, res);
                    break;
                case ActionNextPrev:
                    NextPrevEvent(this, res);
                    break;
                case ActionRepeat:
                    RepeatEvent(this, res);
                    break;
                case AudioManager.ActionAudioBecomingNoisy:
                    PlayPauseEvent(this, false);
                    break;
            }
        }

    }
}
