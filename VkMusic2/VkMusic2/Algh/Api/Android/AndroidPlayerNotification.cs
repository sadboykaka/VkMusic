using System;
using System.Collections.Generic;
using System.Text;
using Android.Media;
using Android;
using Algh.interfaces;
using System.Collections.ObjectModel;
using Android.App;
using Android.Content;

namespace Algh.Api
{
    static public class AndroidPlayerNotification
    {


        static NotificationManager notificationManager;

        static PendingIntent PIPlay;
        static PendingIntent PIPause;
        static PendingIntent PINext;
        static PendingIntent PIPrev;
        static PendingIntent PIClose;
        static PendingIntent PIRepeatOn;
        static PendingIntent PIRepeatOff;
        static AndroidPlayerNotification()
        {
            //-------------------------------------------------------------------
            PIPlay = PendingIntent.GetBroadcast(
                    Xamarin.Forms.Forms.Context,
                    0,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(MessageReplyReceiver.ActionPlay)
                        .PutExtra(MessageReplyReceiver.Request,true),
                    PendingIntentFlags.UpdateCurrent
                );

            PIPause = PendingIntent.GetBroadcast(
                    Xamarin.Forms.Forms.Context,
                    1,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(MessageReplyReceiver.ActionPlay)
                        .PutExtra(MessageReplyReceiver.Request, false),
                    PendingIntentFlags.UpdateCurrent
                );

            PINext = PendingIntent.GetBroadcast(
                    Xamarin.Forms.Forms.Context,
                    2,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(MessageReplyReceiver.ActionNextPrev)
                        .PutExtra(MessageReplyReceiver.Request, true),
                    PendingIntentFlags.UpdateCurrent
                );

            PIPrev = PendingIntent.GetBroadcast(
                    Xamarin.Forms.Forms.Context,
                    3,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(MessageReplyReceiver.ActionNextPrev)
                        .PutExtra(MessageReplyReceiver.Request, false),
                    PendingIntentFlags.UpdateCurrent
                );

            PIClose = PendingIntent.GetService(
                    Xamarin.Forms.Forms.Context,
                    4,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(AudioService.ActionStop),
                    PendingIntentFlags.UpdateCurrent
                );

            PIRepeatOn = PendingIntent.GetBroadcast(
                    Xamarin.Forms.Forms.Context, 
                    5,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(MessageReplyReceiver.ActionRepeat)
                        .PutExtra(MessageReplyReceiver.Request, true),
                    PendingIntentFlags.UpdateCurrent
                );

            PIRepeatOff = PendingIntent.GetBroadcast(
                    Xamarin.Forms.Forms.Context,
                   6,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(MessageReplyReceiver.ActionRepeat)
                        .PutExtra(MessageReplyReceiver.Request, false),
                    PendingIntentFlags.UpdateCurrent
                );

            notificationManager = Xamarin.Forms.Forms.Context.GetSystemService(Android.Content.Context.NotificationService) as NotificationManager;

        }

        //ImageSource repeaton = ImageSource.FromFile("ic_repeat_grey600_36dp.png");
        //ImageSource repeatoff = ImageSource.FromFile("ic_repeat_off_grey600_36dp.png");

        public static Notification Get(string name, string author, string cover, bool i, bool r)
        {
            int icon = i ? VkMusic2.Droid.Resource.Drawable.ic_pause_grey600_36dp : VkMusic2.Droid.Resource.Drawable.ic_play_grey600_36dp;
            int repeaticon = r ? VkMusic2.Droid.Resource.Drawable.ic_repeat_off_grey600_36dp : VkMusic2.Droid.Resource.Drawable.ic_repeat_grey600_36dp;
            PendingIntent playm = i ? PIPause : PIPlay;
            PendingIntent repeatm = r ? PIRepeatOff : PIRepeatOn;
            var builder = new Android.App.Notification.Builder(Xamarin.Forms.Forms.Context)
                .SetStyle(new Notification.MediaStyle()
                    .SetShowActionsInCompactView(
                        new[] { 1, 2, 3 }))
                .AddAction(new Notification.Action(repeaticon, "3", repeatm))
                .AddAction(new Notification.Action(VkMusic2.Droid.Resource.Drawable.ic_skip_previous_grey600_36dp, "1", PIPrev))
                .AddAction(new Notification.Action(icon, "0", playm))
                .AddAction(new Notification.Action(VkMusic2.Droid.Resource.Drawable.ic_skip_next_grey600_36dp, "2", PINext))
                .AddAction(new Notification.Action(VkMusic2.Droid.Resource.Drawable.ic_close_grey600_36dp, "2", PIClose))
                .SetContentTitle(name)
                .SetContentText(author)
                .SetSmallIcon(VkMusic2.Droid.Resource.Mipmap.ic_launcher)
                .SetVisibility(NotificationVisibility.Public)
                .SetShowWhen(false)
                .SetOngoing(true)
                .SetAutoCancel(true);



            return builder.Build();
         
            
        }


    }
}
