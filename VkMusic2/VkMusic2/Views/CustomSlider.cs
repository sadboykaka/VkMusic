using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace VkMusic2.Views
{
    public class CustomSlider : View
    {
        public EventHandler<int> ValueChangeByUser;

        public static readonly BindableProperty MaximumProperty = BindableProperty.Create("Maximum", typeof(int), typeof(CustomSlider), 0);
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(int), typeof(CustomSlider), 0);
        public static readonly BindableProperty UserValueProperty = BindableProperty.Create("UserValue", typeof(int), typeof(CustomSlider), 0, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var bind = (CustomSlider)bindable;
            bind.ValueChangeByUser?.Invoke(bind, (int)newValue);
        });

        public int Maximum
        {
            get
            {
                return (int)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        public int UserValue
        {
            set
            {
                SetValue(UserValueProperty, value);
                SetValue(ValueProperty, value);
            }
        }

        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public CustomSlider()
        {
            
        }

    }

    

}
