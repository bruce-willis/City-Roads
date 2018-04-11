using System;
using CityMap.Types;

namespace CityMap.Helpers
{
    public static class GeoHelper
    {
        private const double AverageRadiusOfEarthKm = 6371;
        public static string ConvertToGeo(GeoPoint currentPoint)
        {
            return
                $"{(currentPoint.X - SvgHelper.BasicPoint.X) / 100.0} {(SvgHelper.BasicPoint.Y - currentPoint.Y) / 100.0}";
        }

        private static double ConvertToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static (double X, double Y) ConvertToMercatorProjection(double latitude, double longitude)
        {
            var rLat = ConvertToRadians(latitude);
            var rLong = ConvertToRadians(longitude);

            var a = 6378137.0;
            var b = 6356752.3142;
            var f = (a - b) / a;

            var e = Math.Sqrt(2 * f - f * f);

            var x = a * rLong;
            var y = a * Math.Log(Math.Tan(Math.PI / 4.0 + rLat / 2.0) *
                                 Math.Pow((1 - e * Math.Sin(rLat)) / (1 + e * Math.Sin(rLat)), e / 2.0));

            return (x, y);
        }

        public static double DistanceBetweenPoints(this GeoPoint current, GeoPoint to)
        {
            var deltaLat = ConvertToRadians(current.Latitude - to.Latitude);
            var deltaLon = ConvertToRadians(current.Longitude - to.Longitude);

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) + Math.Cos(ConvertToRadians(current.Latitude)) *
                    Math.Cos(ConvertToRadians(to.Latitude)) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            var angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * AverageRadiusOfEarthKm;
        }
    }
}