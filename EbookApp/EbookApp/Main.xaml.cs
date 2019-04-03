using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookApp
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Main : ContentPage
    { 
        public CancellationTokenSource cancelSrc;
        private ISpeechToText _searchRecongnitionInstance;
        private ICommand SpeakCommand { get; set; }
         
        public Main()
        {
            InitializeComponent();
             
            // Intialization of voice recognition.
            try
            {
                _searchRecongnitionInstance = DependencyService.Get<ISpeechToText>();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            MessagingCenter.Subscribe<ISpeechToText, string>(this, "STT", (sender, args) =>
            {
                SearchStory(args);
            });

            MessagingCenter.Subscribe<ISpeechToText>(this, "Final", (sender) =>
            {
                SpeakBtn.IsEnabled = true;
            });

            MessagingCenter.Subscribe<IMessageSender, string>(this, "STT", (sender, args) =>
            {
                SearchStory(args);
            });
             
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

        private void Speak_Clicked(object sender, EventArgs e)
        {
            // command for voice recognition.
            try
            {
                _searchRecongnitionInstance.StartSpeechToText();
            }
            catch (Exception ex)
            {
                DisplayAlert("", ex.Message, "OK");
            }
        }


        private async void SearchStory(string args)
        { 
            try
            {
                var story = args.Replace("read ", ""); // keyword for searching


                IFolder folder = FileSystem.Current.LocalStorage; // Access to the local directory of the app

                IFolder rootFolder = await FileSystem.Current.GetFolderFromPathAsync(folder.Path);
                IList<IFolder> folders = await rootFolder.GetFoldersAsync(); // Access to get the sub folders to the local directory

                List<listItems> li = new List<listItems>(); // Initialize list items

                foreach (var folderItem in folders) // Sub Folders
                {
                    IList<IFile> files = await folderItem.GetFilesAsync();

                    foreach (var item in files) // Items per sub folders
                    {
                        if (item.Name.ToLower().Contains(story))
                        {
                            listItems list = new listItems();
                            list.file = item;
                            list.fileName = item.Name.Replace(".pdf", "").Replace(".txt", "").Replace(".docx", "").Replace(".doc", "");
                            list.genre = folderItem.Name;
                            li.Add(list);
                        }
                    }
                }

                listItems doc = li[0];

                if (doc == null)
                {
                    await DisplayAlert("", "Invalid keyword. Please try again!", "OK");
                }
                else
                {
                    await Navigation.PushModalAsync(new Details(doc)); // Navigation to view details of the selected item
                }

            }
            catch (OperationCanceledException)
            {
                //PlayBtn.Image = "Play.png";
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                //PlayBtn.Image = "Play.png";
                cancelSrc = null;
            }

        }
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