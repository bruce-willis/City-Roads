using System;
using System.Collections.Generic;
using System.Linq;
using CityMap.Algorithms.Shortest_Path;
using CityMap.Helpers;
using CityMap.Types;
using CityMap.Types.OSM;

namespace CityMap.Algorithms.Travelling_salesman
{
    public static class NearestNeighbour
    {
        public static void Calculate(City city, string outputDirecotry)
        {
            var destinations = DistanceHelper.Destinations.Keys.ToHashSet();

            //pick random start point
            var rnd = new Random();
            ulong startId = SvgHelper.Dictionary.Keys.ElementAt(rnd.Next(SvgHelper.Dictionary.Count));
            var order = new Queue<(ulong id, IEnumerable<ulong> path)>(destinations.Count);

            var currentId = startId;
            double totalDistance = 0;

            while (destinations.Any())
            {
                var (d, p) = Levit.Calculate(currentId);
                var minId = destinations.First();
                foreach (var destination in destinations.Skip(1))
                {
                    if (d[destination] < d[minId])
                    {
                        minId = destination;
                    }
                }
                order.Enqueue((currentId, DistanceHelper.RestorePath(currentId, minId, p)));
                totalDistance += d[minId];
                destinations.Remove(minId);

                currentId = minId;
            }

            //return back to start
            var (di, an) = Levit.Calculate(currentId);
            order.Enqueue((currentId, DistanceHelper.RestorePath(currentId, startId, an)));
            totalDistance += di[startId];

            Console.WriteLine($"Total length of the path is {totalDistance}");

            SvgHelper.DisplaySalesmanPath(outputDirecotry, "nearest", order);
        }

        public static void CalculateWithRandom(City city, string outputDirecotry)
        {
            var destinations = DistanceHelper.Destinations.Keys.ToHashSet();

            //pick random start point
            var rnd = new Random();
            ulong startId = SvgHelper.Dictionary.Keys.ElementAt(rnd.Next(SvgHelper.Dictionary.Count));
            var order = new Queue<(ulong id, IEnumerable<ulong> path)>(destinations.Count);

            var currentId = startId;
            double totalDistance = 0;

            const double alpha = 1.3;

            while (destinations.Any())
            {
                var (d, p) = Levit.Calculate(currentId);
                var minDistance = destinations.Min(x => d[x]);
                var candidats = destinations.Where(x => d[x] < alpha * minDistance).ToList();
                var minId = candidats.ElementAt(rnd.Next(candidats.Count));
                order.Enqueue((currentId, DistanceHelper.RestorePath(currentId, minId, p)));
                totalDistance += d[minId];
                destinations.Remove(minId);

                currentId = minId;
            }

            //return back to start
            var (di, an) = Levit.Calculate(currentId);
            order.Enqueue((currentId, DistanceHelper.RestorePath(currentId, startId, an)));
            totalDistance += di[startId];

            Console.WriteLine($"Total length of the path is {totalDistance}");

            SvgHelper.DisplaySalesmanPath(outputDirecotry, "nearest_with_random", order);
        }
    }
}