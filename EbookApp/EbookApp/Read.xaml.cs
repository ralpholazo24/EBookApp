
using PCLStorage;
using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Read : ContentPage
    {
        public ICommand DelCommand { get; set; }

        string _genre = string.Empty;
        public Read(string genre)
        {
            _genre = genre;
            InitializeComponent();
            LoadItems(); // Display list items       
        }


        private async void LoadItems()
        {

            IFolder folder = FileSystem.Current.LocalStorage; // Access to the local directory of the app

            IFolder rootFolder = await FileSystem.Current.GetFolderFromPathAsync(folder.Path);
            IList<IFolder> folders = await rootFolder.GetFoldersAsync(); // Access to get the sub folders to the local directory

            List<listItems> li = new List<listItems>(); // Initialize list items

            foreach (var folderItem in folders) // Sub Folders
            {
                if (folderItem.Name == _genre)
                {
                    IList<IFile> files = await folderItem.GetFilesAsync();

                    foreach (var item in files) // Items per sub folders
                    {
                        if (item.Name != null)
                        {
                            listItems list = new listItems();
                            list.file = item;
                            list.fileName = item.Name.Replace(".pdf", "").Replace(".txt", "").Replace(".docx", "").Replace(".doc", "").ToUpper();
                            list.genre = _genre;                            
                            li.Add(list);
                        }
                    }
                }
            }

            lst.ItemsSource = li; // Passing item source to the list view  
            Genre.Text = _genre;

        }

        private void lst_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                var doc = (listItems)e.SelectedItem;
                Navigation.PushModalAsync(new Details(doc)); // Navigation to view details of the selected item
            }
            catch (Exception ex)
            {
                DisplayAlert("Delete:", ex.Message, "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeComponent();
            LoadItems();
        }




    }
}