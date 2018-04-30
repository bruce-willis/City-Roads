using System;
using System.Collections.Generic;
using System.Linq;
using CityMap.Algorithms;
using CityMap.Types;
using CityMap.Types.OSM;

namespace CityMap.Helpers
{
    public static class DistanceHelper
    {
        private const string Amenity = "school";
        private const double SmallDistance = 0.3;

        public static void AddNodes(City city)
        {
            var destinations = city.Nodes.Where(n => n.Tags.Exists(t => t.Key == "amenity" && t.Value == Amenity)).ToDictionary(k => k.Id, v => new GeoPoint(v.Longitude, v.Latitude));

            var size = destinations.Count;

            foreach (var node in city.Nodes)
            {
                if (!SvgHelper.Dictionary.ContainsKey(node.Id)) continue;
                foreach (var dest in destinations)
                {
                    if (dest.Value.DistanceBetweenPoints(new GeoPoint(node.Longitude, node.Latitude)) < SmallDistance)
                    {
                        dest.Value.Adjency.Add(node.Id);
                        SvgHelper.Dictionary[node.Id].Adjency.Add(dest.Key);
                    }
                }
            }

            SvgHelper.Dictionary = SvgHelper.Dictionary.Union(destinations).ToDictionary(d => d.Key, d => d.Value);

            //int s = 0, max = -1;

            //foreach (var node in SvgHelper.Dictionary.Values)
            //{
            //    s += node.Adjency.Count;
            //    max = Math.Max(max, node.Adjency.Count);
            //}

            var (distD, pD) = TimeHelper.MeasureTimeAlgorithm(() => Dijkstra.Calculate(), "Dijkstra");
            var (dist, p) = TimeHelper.MeasureTimeAlgorithm(() => Levit.Calculate(), "Levit");

            var differences = dist.Values.Zip(distD.Values, (d, l) => Math.Abs(d - l)).ToList();

            foreach (var pred in pD)
            {
                if (pred.Value != p[pred.Key])
                {
                    Console.WriteLine("Bad");
                }
            }

            Console.WriteLine($"sum - {differences.Sum()}\t max - {differences.Max()}\t avg - {differences.Average()}");
        }
    }
}