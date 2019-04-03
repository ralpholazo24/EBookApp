using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace EbookApp
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();
             
            if (!Application.Current.Properties.ContainsKey("crossLocale"))
            {
                Application.Current.Properties["crossLocale"] = "Default Language";
            }
             
            MainPage = new NavigationPage(new SplashScreen());
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
            // Handle when your app sleeps
            Application.Current.Quit();

        }

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}


        

    }
}
