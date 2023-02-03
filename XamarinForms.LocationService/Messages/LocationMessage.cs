using System;

namespace XamarinForms.LocationService.Messages
{
    public class LocationMessage
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
