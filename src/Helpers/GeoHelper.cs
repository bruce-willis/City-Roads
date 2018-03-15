using System;
using CityMap.Types;

namespace CityMap.Helpers
{
    public static class GeoHelper
    {
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
    }
}