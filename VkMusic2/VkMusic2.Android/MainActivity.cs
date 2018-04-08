using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace VkMusic2.Droid
{
	[Activity (Label = "Cleef Player", Icon = "@mipmap/ic_launcher", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
        public static bool open { get; private set; }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            open = false;
        }
        protected override void OnCreate (Bundle bundle)
		{
            open = true;
            TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;
            Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, "ca-app-pub-5544910402146685~3548675731");
     
            base.OnCreate (bundle);
			global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new VkMusic2.App ());
        }
	}
}

