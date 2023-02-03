using Android.App;

namespace XamarinForms.LocationService.Droid.Helpers
{
    public interface INotification
    {
        Notification ReturnNotif();

        void UpdateMessage(string title, string content);
    }
}