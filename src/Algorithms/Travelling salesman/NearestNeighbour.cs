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
                foreach (var destination in destinations)
                {
                    if (d[destination] < d[minId])
                    {
                        minId = destination;
                    }
                }

                
                order.Enqueue((currentId, DistanceHelper.RestorePath(currentId, minId, p)));

                currentId = minId;
                totalDistance += d[minId];
                destinations.Remove(minId);
            }

            //return back to start
            var (di, an) = Levit.Calculate(currentId);
            order.Enqueue((currentId, DistanceHelper.RestorePath(currentId, startId, an)));
            totalDistance += di[startId];

            Console.WriteLine($"Total length of the path is {totalDistance}");

            SvgHelper.DisplaySalesmanPath(outputDirecotry, order);
        }
    }
}