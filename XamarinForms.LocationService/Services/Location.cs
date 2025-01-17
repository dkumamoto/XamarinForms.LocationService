﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinForms.LocationService.Messages;

namespace XamarinForms.LocationService.Services
{
    public class Location
    {
		bool stopping = false;
		public Location()
		{
		}

		public async Task Run(CancellationToken token)
		{
			await Task.Run(async () => {
				while (!stopping)
				{
					stopping = token.IsCancellationRequested;
					try
					{
						await Task.Delay(2000);

						var request = new GeolocationRequest(GeolocationAccuracy.High);
						var location = await Geolocation.GetLocationAsync(request);
						if (location != null)
						{
							var message = new LocationMessage
							{
								Latitude = location.Latitude,
								Longitude = location.Longitude,
								TimeStamp = DateTime.Now
							};

							Device.BeginInvokeOnMainThread(() =>
							{
								MessagingCenter.Send<LocationMessage>(message, MessageNames.Location);
							});
						}
					}
					catch (Exception ex)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							var errormessage = new LocationErrorMessage();
							MessagingCenter.Send<LocationErrorMessage>(errormessage, MessageNames.LocationError);
						});
					}
				}
				return;
			}, token);
		}
	}
}
