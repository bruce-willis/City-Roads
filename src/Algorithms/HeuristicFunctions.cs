using System;
using CityMap.Types;

namespace CityMap.Algorithms
{
    public static class HeuristicFunctions
    {
        /// <see cref="https://ru.wikipedia.org/wiki/Расстояние_Чебышёва"/>
        public static double ChebyshevDistance(GeoPoint current, GeoPoint to)
        {
            return Math.Max(Math.Abs(current.X - to.X), Math.Abs(current.Y - to.Y)) / 1e4 * 6;
        }

        /// <see cref="https://ru.wikipedia.org/wiki/Расстояние_городских_кварталов"/>
        public static double ManhattanDistance(GeoPoint current, GeoPoint to)
        {
            return (Math.Abs(current.X - to.X) + Math.Abs(current.Y - to.Y)) / 1e4 * 4.5;
        }

        /// <see cref="https://ru.wikipedia.org/wiki/Евклидова_метрика"/>
        public static double EuclideanDistance(GeoPoint current, GeoPoint to)
        {
            return Math.Sqrt((current.X - to.X) * (current.X - to.X) + (current.Y - to.Y) * (current.Y - to.Y)) / 1e4 * 6;
        }

        public static double DummyDistance(GeoPoint current, GeoPoint to)
        {
            return 0;
        }
    }
}