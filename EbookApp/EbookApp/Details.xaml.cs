

using EbookApp.Model;
using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Details : ContentPage
    {
        private CancellationTokenSource cancelSrc;
        private IXamHelper xamHelper;
        public string word = string.Empty;
        CrossLocale? lang = null;
        listItems document;
        private ISpeechToText _speechRecongnitionInstance;
        private ICommand SpeakCommand { get; set; }
        public float? speakRateSpeed = 0.7f; // Adjust the value to change the spped of the speak rate


        public Details(listItems element)
        {
            try
            {
                InitializeComponent();
                 
                CrossTextToSpeech.Current.MaxSpeechInputLength.Equals(-1);
                 
                document = element;
                
                // Intialization of voice recognition.
                try
                {
                    _speechRecongnitionInstance = DependencyService.Get<ISpeechToText>();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                 
                MessagingCenter.Subscribe<ISpeechToText, string>(this, "STT", (sender, args) =>
                {
                    SpeechToTextFinalResultRecieved(args);                    
                });

                MessagingCenter.Subscribe<ISpeechToText>(this, "Final", (sender) =>
                {
                    SpeakBtn.IsEnabled = true;
                });

                MessagingCenter.Subscribe<IMessageSender, string>(this, "STT", (sender, args) =>
                {
                    SpeechToTextFinalResultRecieved(args);                    
                });


                // Initialization of xam helper wherein this function is used for converting text content of file to string.
                try
                {
                    xamHelper = DependencyService.Get<IXamHelper>();
                }
                catch (Exception ex)
                {                    
                    ex.ToString();
                }

                try
                {
                    // Getting text content from pdf, word and text file.                    
                    var filepath = document.file.Path;
                    word = ConvertText.TextFile(filepath); // By default is text.

                    if (filepath.Contains(".pdf")) // if the file type is pdf.
                    {
                        word = xamHelper.PDTtoText(filepath);
                    }

                    if (filepath.Contains(".docx") || filepath.Contains(".doc")) // if the file type is word document.
                    {
                        word = xamHelper.WordToText(filepath);
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error:", "Please try again to upload valid file.", "OK");                    
                }

                TitleName.Text = element.fileName.Replace(".pdf", "").Replace(".txt", "").Replace(".docx", "").Replace(".doc", "").ToUpper();
                BookContent.Text = word; // displaying text content in screen.

            }
            catch (Exception ex)
            {
                DisplayAlert("", ex.Message, "OK");
            }
            finally
            {
            }

        }

        private async void SpeechToTextFinalResultRecieved(string args)
        {
            if (args.Contains("stop"))
            {
                try
                {
                    if (cancelSrc != null)
                    {
                        cancelSrc.Cancel();
                    }
                }
                catch (OperationCanceledException)
                {
                    PlayBtn.Image = "Play.png";
                    cancelSrc = null;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                finally
                {
                    PlayBtn.Image = "Play.png";
                    cancelSrc = null;
                }
            }
            else if (args.Contains("read") || args.Contains("play"))
            {
                PlayBtn.Image = "Stop.png";

                try
                {
                    if (cancelSrc == null)
                    {
                        string result = Application.Current.Properties["crossLocale"].ToString();
                        var items = await CrossTextToSpeech.Current.GetInstalledLanguages();
                        lang = items.FirstOrDefault(i => i.DisplayName == result);

                        cancelSrc = new CancellationTokenSource();

                        float? vol = 100;
                        //if (volumeSwitch.IsToggled)
                        //    vol = (float)volumeSlider.Value;

                        await CrossTextToSpeech.Current.Speak(word, lang, speakRate: speakRateSpeed, volume: vol, cancelToken: cancelSrc.Token);
                         
                        cancelSrc = null;
                    }
                    else
                    {
                        cancelSrc.Cancel();
                    }
                }
                catch (OperationCanceledException)
                {
                    PlayBtn.Image = "Play.png";
                    cancelSrc = null;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                finally
                {
                    PlayBtn.Image = "Play.png";
                    cancelSrc = null;
                }
            }
            else
            {
                try
                {
                    await DisplayAlert("", "Invalid keyword. Please try again!", "OK");
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
        }

        private async Task Delete_Clicked(object sender, EventArgs e)
        {
            AnimateButton.animateButton(DeleteBtn);
            bool isDelete = await DisplayAlert("Delete:", "Are you sure you want to delete this document?", "Yes", "No");

            if (isDelete)
            {
                QuestionItem questionItem = await App.Database.GetItemAsync(document.genre, TitleName.Text);

                if (questionItem != null)
                {
                    await App.Database.DeleteItemAsync(questionItem);
                }
                 
                await DisplayAlert("", "Deleted successfully!", "OK");

                await document.file.DeleteAsync();
                await Navigation.PopModalAsync();
            }
        }

        private async void Play_Clicked(object sender, EventArgs e)
        {            
            PlayBtn.Image = "Stop.png";
            AnimateButton.animateButton(PlayBtn);
            try
            {
                if (cancelSrc == null)
                {
                    string result = Application.Current.Properties["crossLocale"].ToString();

                    var items = await CrossTextToSpeech.Current.GetInstalledLanguages();
                    lang = items.FirstOrDefault(i => i.DisplayName == result);

                    cancelSrc = new CancellationTokenSource();

                    float? vol = 100;
                    //if (volumeSwitch.IsToggled)
                    //    vol = (float)volumeSlider.Value; 
                    
                    await CrossTextToSpeech.Current.Speak(word, lang, speakRate: speakRateSpeed, volume: vol, cancelToken: cancelSrc.Token);    

                    cancelSrc = null;
                }
                else
                {
                    cancelSrc.Cancel();
                }
            }
            catch (OperationCanceledException)
            {
                PlayBtn.Image = "Play.png";
                cancelSrc = null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("", ex.Message, "OK");
            }
            finally
            {
                PlayBtn.Image = "Play.png";
                cancelSrc = null;
            }
        }

        private void Speak_Clicked(object sender, EventArgs e)
        {
            AnimateButton.animateButton(SpeakBtn);
            // command for voice recognition.
            try
            {
                _speechRecongnitionInstance.StartSpeechToText();

                if (cancelSrc != null)
                {
                    cancelSrc.Cancel();
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("", ex.Message, "OK");
            }
        }

        private void OpenYoutube_Clicked(object sender, EventArgs e)
        {
            AnimateButton.animateButton(OpenYoutubeBtn);
            string title = TitleName.Text.Replace(" ","+");
            string url = $"https://www.youtube.com/results?search_query={title}"; 
             
            Device.OpenUri(new Uri(url)); // open youtube link
        }

        private void Search_Clicked(object sender, EventArgs e)
        {
            string title = InputSearch.Text.Replace(" ", "+");
            string url = $"https://www.google.com/search?q={title}";

            Device.OpenUri(new Uri(url)); // open youtube link
        }

        async void Question_Clicked(object sender, EventArgs e)
        {
            AnimateButton.animateButton(QuestionBtn);
            QuestionItem questionItem = await App.Database.GetItemAsync(document.genre, TitleName.Text);

            if (questionItem == null)
            {
                questionItem = new QuestionItem
                {  
                    Genre = document.genre,
                    Title = TitleName.Text,
                    QOne = string.Empty,
                    QTwo = string.Empty,
                    QThree = string.Empty
                };
            }
             
            await Navigation.PushModalAsync(new Question(questionItem)); // Use to navigate question
             
        }

        async void Settings_Clicked(object sender, EventArgs e)
        {
            AnimateButton.animateButton(SettingsBtn);
            await Navigation.PushModalAsync(new Settings()); // Use to navigate settings
        }




        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (cancelSrc != null)
            {
                cancelSrc.Cancel();
            }

            word = string.Empty;


        }

        private void BtnSearch_Clicked(object sender, EventArgs e)
        {
            if (!InputSearch.IsVisible)
                InputSearch.IsVisible = true;
            else
                InputSearch.IsVisible = false;
        }
    }
}