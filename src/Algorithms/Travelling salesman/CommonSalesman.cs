using System;
using System.Collections.Generic;
using System.Linq;
using CityMap.Algorithms.Shortest_Path;
using CityMap.Helpers;

namespace CityMap.Algorithms.Travelling_salesman
{
    public static class CommonSalesman
    {
        //pick random start point
        public static readonly ulong StartId = SvgHelper.Dictionary.Keys.ElementAt(new Random().Next(SvgHelper.Dictionary.Count));

        private static Dictionary<(ulong from, ulong to), (double distance, IEnumerable<ulong> path)> _distances;
        public static Dictionary<(ulong from, ulong to), (double distance, IEnumerable<ulong> path)> Distances => _distances ?? (_distances = CalculateDistances());


        private static Dictionary<(ulong from, ulong to), (double distance, IEnumerable<ulong> path)>CalculateDistances()
        {
            var destinations = DistanceHelper.Destinations.Keys.ToList();
            destinations.Add(StartId);
            var distances = new Dictionary<(ulong from, ulong to), (double distance, IEnumerable<ulong> path)>();

            for (int i = 0; i < destinations.Count; ++i)
            {
                var from = destinations[i];
                var (d, p) = Levit.Calculate(from);
                for (int j = i + 1; j < destinations.Count; ++j)
                {
                    var to = destinations[j];
                    distances[(from, to)] = (d[to], DistanceHelper.RestorePath(from, to, p));
                    distances[(to, from)] = (d[to], DistanceHelper.RestorePath(from, to, p).Reverse());
                }
            }

            return distances;
        }
    }
}