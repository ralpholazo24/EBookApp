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
            var items = await CrossTextToSpeech.Current.GetInstalledLanguages();
            var result = await DisplayActionSheet("Pick", "", null, items.Select(i => i.DisplayName).ToArray());
            lang = items.FirstOrDefault(i => i.DisplayName == result);
            lblLanguage.Text = (result == "OK" || string.IsNullOrEmpty(result)) ? "Default Language" : result;
            
            Application.Current.Properties["crossLocale"] = lblLanguage.Text;
        }
    }


}