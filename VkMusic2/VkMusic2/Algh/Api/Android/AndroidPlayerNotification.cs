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

        static PendingIntent PIPlayPause;
        static PendingIntent PINext;
        static PendingIntent PIPrev;
        static PendingIntent PIClose;

        static AndroidPlayerNotification()
        {
            //-------------------------------------------------------------------
            PIPlayPause = PendingIntent.GetService(
                    Xamarin.Forms.Forms.Context,
                    0,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(AudioService.ActionPlayPause),
                    PendingIntentFlags.UpdateCurrent
                );

            PINext = PendingIntent.GetService(
                    Xamarin.Forms.Forms.Context,
                    1,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(AudioService.ActionNext),
                    PendingIntentFlags.UpdateCurrent
                );

            PIPrev = PendingIntent.GetService(
                    Xamarin.Forms.Forms.Context,
                    2,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(AudioService.ActionPrev),
                    PendingIntentFlags.UpdateCurrent
                );

            PIClose = PendingIntent.GetService(
                    Xamarin.Forms.Forms.Context,
                    2,
                   new Intent()
                        .AddFlags(ActivityFlags.IncludeStoppedPackages)
                        .SetAction(AudioService.ActionClose),
                    PendingIntentFlags.UpdateCurrent
                );

            notificationManager = Xamarin.Forms.Forms.Context.GetSystemService(Android.Content.Context.NotificationService) as NotificationManager;

        }

        public static Notification Get(string name, string author, bool i)
        {
            int icon = i ? VkMusic2.Droid.Resource.Drawable.ic_pause_grey600_36dp : VkMusic2.Droid.Resource.Drawable.ic_play_grey600_36dp;
            var builder = new Android.App.Notification.Builder(Xamarin.Forms.Forms.Context)
                .SetStyle(new Notification.MediaStyle()
                    .SetShowActionsInCompactView(
                        new[] { 0, 1, 2, 3 }))
                .AddAction(new Notification.Action(VkMusic2.Droid.Resource.Drawable.ic_skip_previous_grey600_36dp, "1", PIPrev))
                .AddAction(new Notification.Action(icon, "0", PIPlayPause))
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
