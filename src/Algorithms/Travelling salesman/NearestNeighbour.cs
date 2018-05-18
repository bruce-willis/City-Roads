using System;
using System.Collections.Generic;
using System.Linq;
using CityMap.Helpers;

namespace CityMap.Algorithms.Travelling_salesman
{
    public static class NearestNeighbour
    {
        public static double Calculate(string outputDirectory, bool withRandom = false)
        {
            var destinations = DistanceHelper.Destinations.Keys.ToHashSet();
            
            var startId = CommonSalesman.StartId;
            var currentId = startId;
            double totalDistance = 0;

            const double alpha = 1.3;
            var rnd = new Random();

            var order = new List<ulong>(destinations.Count) {startId};
            while (destinations.Any())
            {
                ulong minId;
                if (withRandom)
                {
                    var minDistance = destinations.Min(x => CommonSalesman.Distances[(currentId, x)].distance);
                    var candidates = destinations
                        .Where(x => CommonSalesman.Distances[(currentId, x)].distance < alpha * minDistance).ToList();
                    minId = candidates.ElementAt(rnd.Next(candidates.Count));
                }
                else
                {
                    minId = destinations.First();
                    foreach (var destination in destinations.Skip(1))
                        if (CommonSalesman.Distances[(currentId, destination)].distance <
                            CommonSalesman.Distances[(currentId, minId)].distance)
                            minId = destination;
                }

                order.Add(minId);
                totalDistance += CommonSalesman.Distances[(currentId, minId)].distance;
                destinations.Remove(minId);

                currentId = minId;
            }

            //return back to start point
            totalDistance += CommonSalesman.Distances[(currentId, startId)].distance;
            Console.WriteLine($"Total length of the path is {totalDistance}");

            SvgHelper.DisplaySalesmanPath(outputDirectory, withRandom ? "nearest_with_random" : "nearest", order);
            CsvHelper.WriteSalesmanPath(outputDirectory, withRandom ? "nearest_with_random" : "nearest", order);
            return totalDistance;
        }
    }
}