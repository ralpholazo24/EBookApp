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

        public CancellationTokenSource cts;
        private ISpeechToText _voiceCommand;
        private ICommand SpeakCommand { get; set; }
         
        public Main()
        {
            InitializeComponent();
             
            // Intialization of voice recognition.
            try
            {
                _voiceCommand = DependencyService.Get<ISpeechToText>();
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

        async void Import_Clicked(object sender, EventArgs e)
        {
            AnimateButton.animateButton(ImportBtn);
            await Navigation.PushModalAsync(new Import()); // Use to navigate import page            
        }

        async void Settings_Clicked(object sender, EventArgs e)
        {
            AnimateButton.animateButton(SettingsBtn);
            await Navigation.PushModalAsync(new Settings()); // Use to navigate settings
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();
        }

        private async void Speak_Clicked(object sender, EventArgs e)
        {
            // command for voice recognition.
            try
            {
                AnimateButton.animateButton(SpeakBtn);
                _voiceCommand.StartSpeechToText();
            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.Message, "OK");
            }
        }

        

        private void Browser_Clicked(object sender, EventArgs e)
        { 
             
        }
         
        private async void SearchStory(string args)
        { 
            try
            {
                var story = args;
                 
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
                cts = null;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                 cts = null;
            }

        }



        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (cts != null)
            {
                cts.Cancel();
            }

        }
    }
     
     
}