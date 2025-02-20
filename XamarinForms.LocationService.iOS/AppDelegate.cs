﻿using CoreLocation;
using Foundation;
using System;
using UIKit;
using Xamarin.Forms;
using XamarinForms.LocationService.iOS.Services;
using XamarinForms.LocationService.Messages;

namespace XamarinForms.LocationService.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        iOsLocationService locationService;
        readonly CLLocationManager locMgr = new CLLocationManager();
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            locationService = new iOsLocationService();
            SetServiceMethods();
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            //Background Location Permissions
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locMgr.RequestAlwaysAuthorization();
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locMgr.AllowsBackgroundLocationUpdates = true;
            }

            return base.FinishedLaunching(app, options);
        }

        void SetServiceMethods()
        {
            MessagingCenter.Subscribe<StartServiceMessage>(this, MessageNames.ServiceStarted, async message => {
                if (!locationService.isStarted)
                    await locationService.Start();
            });

            MessagingCenter.Subscribe<StopServiceMessage>(this, MessageNames.ServiceStopped, message => {
                if (locationService.isStarted)
                    locationService.Stop();
            });
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            try
            {
                completionHandler(UIBackgroundFetchResult.NewData);
            }
            catch (Exception)
            {
                completionHandler(UIBackgroundFetchResult.NoData);
            }
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            base.DidEnterBackground(uiApplication);
            var message = new BackgroundState { InBackground = true };
            Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(message, MessageNames.BackgroundState));

        }

        [Export("applicationWillEnterForeground:")]
        public override void WillEnterForeground(UIApplication application)
        {
            var message = new BackgroundState { InBackground = false };
            Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(message, MessageNames.BackgroundState));

        }
    }
}
