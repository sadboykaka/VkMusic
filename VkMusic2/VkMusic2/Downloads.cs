using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VkMusic2
{
    public partial class Downloads : ContentPage
    {
        public Downloads(ListView downloads)
        {
            this.Title = "Загружаются:";
            this.Content = downloads;
        }
    }
}