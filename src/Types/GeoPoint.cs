using System;
using System.Collections.Generic;

namespace CityMap.Types
{
    public class GeoPoint
    {
        private static double ConvertToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private void ConvertToMercatorProjection()
        {
            var rLat = ConvertToRadians(Latitude);
            var rLong = ConvertToRadians(Longitude);

            var a = 6378137.0;
            var b = 6356752.3142;
            var f = (a - b) / a;

            var e = Math.Sqrt(2 * f - f * f);

            X = a * rLong;
            Y = a * Math.Log(Math.Tan(Math.PI / 4.0 + rLat / 2.0) * Math.Pow((1 - e * Math.Sin(rLat)) / (1 + e * Math.Sin(rLat)), e / 2.0));
        }

        public GeoPoint(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
            ConvertToMercatorProjection();
        }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool Used { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public HashSet<ulong> Adjency { get; } = new HashSet<ulong>();
    }
}