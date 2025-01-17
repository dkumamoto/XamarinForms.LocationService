﻿using Android.App;
using Android.Content;
using System.Threading.Tasks;
using Android.OS;
using System.Threading;
using Xamarin.Forms;
using XamarinForms.LocationService.Services;
using XamarinForms.LocationService.Messages;
using XamarinForms.LocationService.Droid.Helpers;
using System;

namespace XamarinForms.LocationService.Droid.Services
{
    [Service]
    public class AndroidLocationService : Service
    {
		CancellationTokenSource _cts;
		public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			_cts = new CancellationTokenSource();
			var notifService = DependencyService.Get<INotification>();
            Notification notif = notifService.ReturnNotif();
			StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notif);
            MessagingCenter.Subscribe<LocationMessage>(this, MessageNames.Location, message => {
				notifService.UpdateMessage(message.TimeStamp.ToString(), "location " + message.Latitude + "," + message.Longitude);
            });

            Task.Run(() => {
				try
				{
					var locShared = new Location();
					locShared.Run(_cts.Token).Wait();
				}
				catch (Android.Accounts.OperationCanceledException)
				{
				}
				finally
				{
					if (_cts.IsCancellationRequested)
					{
						var message = new StopServiceMessage();
						Device.BeginInvokeOnMainThread(
							() => MessagingCenter.Send(message, MessageNames.ServiceStopped)
						);
					}
				}
			}, _cts.Token);

			return StartCommandResult.Sticky;
		}



		public override void OnDestroy()
		{
			if (_cts != null)
			{
				_cts.Token.ThrowIfCancellationRequested();
				_cts.Cancel();
			}
			MessagingCenter.Unsubscribe<LocationMessage>(this, MessageNames.Location);
			base.OnDestroy();
		}
	}
}