using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinForms.LocationService.Messages;
using XamarinForms.LocationService.Utils;

namespace XamarinForms.LocationService.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region vars
        private double latitude;
        private double longitude;
        public string userMessage;
        public bool startEnabled;
        public bool stopEnabled;
        private string timeStr;
        #endregion vars

        #region properties
        public double Latitude
        {
            get => latitude;
            set => SetProperty(ref latitude, value);
        }
        public double Longitude
        {
            get => longitude;
            set => SetProperty(ref longitude, value);
        }
        public string UserMessage
        {
            get => userMessage;
            set => SetProperty(ref userMessage, value);
        }
        public bool StartEnabled
        {
            get => startEnabled;
            set => SetProperty(ref startEnabled, value);
        }
        public bool StopEnabled
        {
            get => stopEnabled;
            set => SetProperty(ref stopEnabled, value);
        }

        public string TimeStr
        {
            get => timeStr;
            set => SetProperty(ref timeStr, value);
        }
        #endregion properties

        #region commands
        public Command StartCommand { get; }
        public Command EndCommand { get; }
        #endregion commands

        readonly ILocationConsent locationConsent;

        public MainPageViewModel()
        {
            locationConsent = DependencyService.Get<ILocationConsent>();
            StartCommand = new Command(() => OnStartClick());
            EndCommand = new Command(() => OnStopClick());
            HandleReceivedMessages();
            locationConsent.GetLocationConsent();
            StartEnabled = true;
            StopEnabled = false;
            ValidateStatus();
        }

        public void OnStartClick()
        {
            Start();
        }

        public void OnStopClick()
        {
            var message = new StopServiceMessage();
            MessagingCenter.Send(message, MessageNames.ServiceStopped);
            UserMessage = "Location Service has been stopped!";
            SecureStorage.SetAsync(Constants.SERVICE_STATUS_KEY, "0");
            StartEnabled = true;
            StopEnabled = false;
        }

        void ValidateStatus() 
        {
            var status = SecureStorage.GetAsync(Constants.SERVICE_STATUS_KEY).Result;
            if (status != null && status.Equals("1")) 
            {
                Start();
            }
        }

        void Start() 
        {
            var message = new StartServiceMessage();
            MessagingCenter.Send(message, MessageNames.ServiceStarted);
            UserMessage = "Location Service has been started!";
            SecureStorage.SetAsync(Constants.SERVICE_STATUS_KEY, "1");
            StartEnabled = false;
            StopEnabled = true;
        }


        bool InBackground = false;

        void HandleReceivedMessages()
        {
            MessagingCenter.Subscribe<LocationMessage>(this, MessageNames.Location, message => {
                if (!InBackground)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Latitude = message.Latitude;
                        Longitude = message.Longitude;
                        TimeStr = message.TimeStamp.ToString();
                        UserMessage = "Location Updated";
                    });
                }
                Console.WriteLine(message.TimeStamp.ToString() + ": location " + message.Latitude + "," + message.Longitude );
            });
            MessagingCenter.Subscribe<BackgroundState>(this, MessageNames.BackgroundState, message =>
            {
                InBackground = message.InBackground;
            });
            MessagingCenter.Subscribe<StopServiceMessage>(this, MessageNames.ServiceStopped, message => {
                Device.BeginInvokeOnMainThread(() => {
                    UserMessage = "Location Service has been stopped!";
                });
            });
            MessagingCenter.Subscribe<LocationErrorMessage>(this, MessageNames.LocationError, message => {
                Device.BeginInvokeOnMainThread(() => {
                    UserMessage = "There was an error updating location!";
                });
            });
        }

    }
}
