using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        CrossLocale? lang = null;

        public Settings()
        {
            InitializeComponent();

            if (Application.Current.Properties.ContainsKey("crossLocale"))
            {
                lblLanguage.Text = Application.Current.Properties["crossLocale"].ToString();
            }
        }
         
        private async void Choose_Clicked(object sender, EventArgs e)
        {
            var items = await CrossTextToSpeech.Current.GetInstalledLanguages(); // Getting all the languages available to the cross text to speech nuget package.
            var languageList = items.Where(i => i.DisplayName.Contains("Filipino") || i.DisplayName.Contains("English")).Select(i => i.DisplayName).ToArray(); // Limit the language to filipino and english
             
            var result = await DisplayActionSheet("Pick", "", null, languageList);
            lang = items.FirstOrDefault(i => i.DisplayName == result);
            lblLanguage.Text = (result == "OK" || string.IsNullOrEmpty(result)) ? "Default Language" : result; 
            
            Application.Current.Properties["crossLocale"] = lblLanguage.Text;
        }


        private async void Copyright_Clicked(object sender, EventArgs e)
        {
            AnimateButton.animateButton(btnCopyright);
            await Navigation.PushModalAsync(new Copyright()); // Use to navigate settings
        }

        private async void UserGuide_Clicked(object sender, EventArgs e)
        {
            AnimateButton.animateButton(btnCopyright);
            await Navigation.PushModalAsync(new UserGuide()); // Use to navigate settings
        }
    }


}