using System.Collections.Generic;
using CityMap.Helpers;

namespace CityMap.Types
{
    public class GeoPoint
    {
        public GeoPoint(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
            (X, Y) = GeoHelper.ConvertToMercatorProjection(Latitude, Longitude);
        }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool Used { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public HashSet<ulong> Adjency { get; } = new HashSet<ulong>();
    }
}