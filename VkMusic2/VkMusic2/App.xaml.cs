using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Algh.interfaces;
using Algh.Api;


namespace VkMusic2
{
	public partial class App : Application
	{
        


		public App ()
		{
            InitializeComponent();
            
			MainPage = new NavigationPage(new VkMusic2.MainPage());
            
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
