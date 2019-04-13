

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
         
        public Details(listItems element)
        {
            try
            {
                InitializeComponent();
                 
                CrossTextToSpeech.Current.MaxSpeechInputLength.Equals(-1);

                document = element;
                lbl.Text = element.fileName;
                
   
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
                    lbl.Text = ex.Message;
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
                    lbl.Text = ex.Message;
                }

                Title.Text = element.fileName.Replace(".pdf", "").Replace(".txt", "").Replace(".docx", "");
                Content.Text = word; // displaying text content in screen.

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

                        await CrossTextToSpeech.Current.Speak(word, lang, volume: vol, cancelToken: cancelSrc.Token);
                         
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
            bool isDelete = await DisplayAlert("Delete:", "Are you sure you want to delete this document?", "Yes", "No");

            if (isDelete)
            {
                QuestionItem questionItem = await App.Database.GetItemAsync(document.genre, Title.Text);

                if (questionItem != null)
                {
                    await App.Database.DeleteItemAsync(questionItem);
                }
                 
                await DisplayAlert("", "Deleted successfully!", "OK");

                await document.file.DeleteAsync();
                await Navigation.PopModalAsync();
            }
        }

        private async void Read_Clicked(object sender, EventArgs e)
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

                    await CrossTextToSpeech.Current.Speak(word, lang, volume: vol, cancelToken: cancelSrc.Token);
                     
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

        async void Question_Clicked(object sender, EventArgs e)
        {
            QuestionItem questionItem = await App.Database.GetItemAsync(document.genre, Title.Text);

            if (questionItem == null)
            {
                questionItem = new QuestionItem
                {  
                    Genre = document.genre,
                    Title = Title.Text,
                    QOne = string.Empty,
                    QTwo = string.Empty,
                    QThree = string.Empty
                };
            }
             
            await Navigation.PushModalAsync(new Question(questionItem)); // Use to navigate question
             
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

    }
}