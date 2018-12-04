using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookApp
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Main : ContentPage
    {     
        public Main()
        {
            InitializeComponent();             
            LoadItems();            
        }

        private async void LoadItems()
        {
            IFolder folder = FileSystem.Current.LocalStorage; // Access to the local directory of the app

            IFolder rootFolder = await FileSystem.Current.GetFolderFromPathAsync(folder.Path);
            IList<IFolder> folders = await rootFolder.GetFoldersAsync(); // Access to get the sub folders to the local directory

            List<listItems> li = new List<listItems>(); // Initialize list items

            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");

            foreach (var folderItem in folders) // Sub Folders
            {

                if (regexItem.IsMatch(folderItem.Name))
                {
                    listItems list = new listItems();
                    list.fileName = folderItem.Name;
                    list.file = null;
                    li.Add(list);
                }
            }



            lst.ItemsSource = li; // Passing item source to the list view
        }

        public void lst_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var doc = (listItems)e.SelectedItem;
            Navigation.PushModalAsync(new Read(doc.fileName)); // Laman ng selected genre Navigation to view details of the selected item
        }

        void Import_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new Import()); // Use to navigate import page            
        }

        void Settings_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new Settings()); // Use to navigate settings
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();         
            LoadItems();
        }


        //protected override bool OnBackButtonPressed()
        //{
        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        var result = await this.DisplayAlert("Alert!", "Do you really want to exit?", "Yes", "No");

        //        if (!result)
        //        {
        //            base.OnBackButtonPressed();
        //            await this.Navigation.PopAsync();
        //        }                    
                
        //    });
            
        //    return true;            
        //}
        
    }
}