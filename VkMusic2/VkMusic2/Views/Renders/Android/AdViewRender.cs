
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Util;
using Android.Widget;
using System.ComponentModel;
using VkMusic2.Views;
using VkMusic2.Views.Renders.Android;

[assembly: ExportRenderer(typeof(adsView), typeof(AdViewRender))]
namespace VkMusic2.Views.Renders.Android
{
    class AdViewRender : ViewRenderer<adsView, AdView>
    {
        string adUnitId = string.Empty;
        AdSize adSize = AdSize.SmartBanner;
        AdView adView;
        override protected AdView CreateNativeControl()
        {
            if (adView != null)
                return adView;

            adUnitId = Element.ID;
            adView = new AdView(Forms.Context);
            adView.AdSize = adSize;
            adView.AdUnitId = adUnitId;

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            adView.LayoutParameters = adParams;

            adView.LoadAd(new AdRequest
                            .Builder()
                            .Build());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<adsView> args)
        {
            base.OnElementChanged(args);
            if (Control == null)
            {
                CreateNativeControl();
                SetNativeControl(adView);
            }
        }

    }
}
