using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using XamarinForms.LocationService.Droid.Helpers;
using XamarinForms.LocationService.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationHelper))]
namespace XamarinForms.LocationService.Droid.Helpers
{
    internal class NotificationHelper : INotification
    {
        private static readonly string foregroundChannelId = "9001";
        private static readonly Context context = global::Android.App.Application.Context;


        NotificationCompat.Builder _notifBuilder;
        NotificationManager _notifManager;

        public Notification ReturnNotif()
        {
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.PutExtra("Title", "Message");

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.Immutable);

            _notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                .SetContentTitle("Your Title")
                .SetContentText("Your Message")
                .SetSmallIcon(Resource.Drawable.location)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent);

            if (global::Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.High)
                {
                    Importance = NotificationImportance.High
                };
                notificationChannel.EnableLights(true);
                notificationChannel.EnableVibration(true);
                notificationChannel.SetShowBadge(true);
                notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300 });

                _notifManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
                if (_notifManager != null)
                {
                    _notifBuilder.SetChannelId(foregroundChannelId);
                    _notifManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return _notifBuilder.Build();
        }

        public void UpdateMessage(string title, string content)
        {
            _notifBuilder.SetContentTitle(title);
            _notifBuilder.SetContentText(content);
            _notifManager.Notify(AndroidLocationService.SERVICE_RUNNING_NOTIFICATION_ID, _notifBuilder.Build());
        }
    }
}