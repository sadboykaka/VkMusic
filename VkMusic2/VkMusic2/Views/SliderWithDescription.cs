using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace VkMusic2.Views
{
    class SliderWithDescription : StackLayout
    {
        private CustomSlider slider;
        private int Min;
        public SliderWithDescription(string name, string param, int min, int max, int value, EventHandler<int> f)
        {
            Label Name = new Label();
            Name.Text = name;

            Label LeftParam = new Label();
            LeftParam.Text = min + param;

            Label RightParam = new Label();
            RightParam.Text = max + param;

            slider = new CustomSlider();
            slider.Maximum = max - min;
            slider.Value = value - min;
            slider.ValueChangeByUser += (i, e) => {f(this, e + min); };
            StackLayout Sbase = new StackLayout { Children = { LeftParam, slider, RightParam } };
            //Sbase.Orientation = StackOrientation.Horizontal;

            Children.Add(Name);
            Children.Add(Sbase);
            Min = min;
        }

        public void ChangeValue(int value)
        {
            slider.Value = value - Min;
        }

    }
}
