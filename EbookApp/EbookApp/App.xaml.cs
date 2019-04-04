using EbookApp.Data;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using PCLStorage;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace EbookApp
{
	public partial class App : Application
	{
        static DAL database;
        private static IFolder folder = FileSystem.Current.LocalStorage; // Access to the local directory of the app
         
        public App ()
		{
			InitializeComponent();
             
            if (!Application.Current.Properties.ContainsKey("crossLocale"))
            {
                Application.Current.Properties["crossLocale"] = "Default Language";
            }
             
            MainPage = new NavigationPage(new SplashScreen());
        }

        public static DAL Database
        { 
            get
            {
                if (database == null)
                { 
                    database = new DAL(Path.Combine(folder.Path.ToString(), "SQLite.db3"));
                }

                return database;
            }
        }
         
        public int ResumeAtTodoId { get; set; }

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
