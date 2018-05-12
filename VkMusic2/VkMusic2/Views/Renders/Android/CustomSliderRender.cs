using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Util;
using Android.Widget;
using System.ComponentModel;
using VkMusic2.Views;
using VkMusic2.Views.Renders.Android;

[assembly: ExportRenderer(typeof(CustomSlider), typeof(HeaderViewRenderer))]
namespace VkMusic2.Views.Renders.Android
{
    public class HeaderViewRenderer : ViewRenderer<CustomSlider, SeekBar>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomSlider> args)
        {
            base.OnElementChanged(args);
            if (Control == null)
            {
                SeekBar bar = new SeekBar(Context);
                bar.ProgressChanged += (i, e) => {
                    if (e.FromUser)
                    {
                        Element.UserValue = e.Progress;
                    }
                };
                bar.Max = Element.Maximum;
                bar.Progress = Element.Value;
                SetNativeControl(bar);
                
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == CustomSlider.MaximumProperty.PropertyName)
            {
                if (Element.Maximum > Control.Progress) Control.Progress = 0;
                Control.Max = Element.Maximum;
                
            }
            else if (e.PropertyName == CustomSlider.ValueProperty.PropertyName)
            {
                if(Element.Value <= Control.Max) Control.Progress = Element.Value;
                 
            }
        }

    }
}
