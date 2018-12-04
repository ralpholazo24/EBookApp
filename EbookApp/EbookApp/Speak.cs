using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EbookApp
{
    public class DemoViewModel : INotifyPropertyChanged
    {
        bool canDownload = true;
        string simulatedDownloadResult;

        public int Number { get; set; }

        public double SquareRootResult { get; private set; }

        public double SquareRootWithParameterResult { get; private set; }

        public string SimulatedDownloadResult
        {
            get { return simulatedDownloadResult; }
            private set
            {
                if (simulatedDownloadResult != value)
                {
                    simulatedDownloadResult = value;
                    OnPropertyChanged("SimulatedDownloadResult");
                }
            }
        }

        public ICommand SquareRootCommand { get; private set; }

        public ICommand SquareRootWithParameterCommand { get; private set; }

        public ICommand SimulateDownloadCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        
        public DemoViewModel()
        {
            Number = 25;
            SquareRootCommand = new Command(CalculateSquareRoot);
            SquareRootWithParameterCommand = new Command<string>(CalculateSquareRoot);
            SimulateDownloadCommand = new Command(async () => await SimulateDownloadAsync(), () => canDownload);

        
        }

        void CalculateSquareRoot()
        {
            
            SquareRootResult = Math.Sqrt(Number);
            OnPropertyChanged("SquareRootResult");
        }

        void CalculateSquareRoot(string value)
        {
            double num = Convert.ToDouble(value);
            SquareRootWithParameterResult = Math.Sqrt(num);
            OnPropertyChanged("SquareRootWithParameterResult");
        }

        async Task SimulateDownloadAsync()
        {
            CanInitiateNewDownload(false);
            SimulatedDownloadResult = string.Empty;
            await Task.Run(() => SimulateDownload());
            SimulatedDownloadResult = "Simulated download complete";
            CanInitiateNewDownload(true);
        }

        void CanInitiateNewDownload(bool value)
        {
            canDownload = value;
            ((Command)SimulateDownloadCommand).ChangeCanExecute();
        }

        void SimulateDownload()
        {
            // Simulate a 5 second pause
            var endTime = DateTime.Now.AddSeconds(5);
            while (true)
            {
                if (DateTime.Now >= endTime)
                {
                    break;
                }
            }
        }


        CancellationTokenSource cancelSrc;
        private IXamHelper xamHelper;
        public string word = string.Empty;


        CrossLocale? lang = null;

        async Task Speak()
        {
            //PlayBtn.Image = "Stop.png";
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
                //PlayBtn.Image = "Play.png";
            }
            catch (Exception ex)
            {
                ex.ToString();
                //await DisplayAlert("", ex.Message, "OK");
            }
            finally
            {
                //PlayBtn.Image = "Play.png";
                cancelSrc = null;
            }

        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}
