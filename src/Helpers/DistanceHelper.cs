using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public static void CompareAlgorithms(City city)
        {
            var destinations = city.Nodes.Where(n => n.Tags.Exists(t => t.Key == "amenity" && t.Value == Amenity))
                .ToDictionary(k => k.Id, v => new GeoPoint(v.Longitude, v.Latitude));

            var size = destinations.Count;

            foreach (var node in SvgHelper.Dictionary)
            {
                foreach (var dest in destinations)
                    if (dest.Value.DistanceBetweenPoints(new GeoPoint(node.Value.Longitude, node.Value.Latitude)) < SmallDistance)
                    {
                        var realDistance = dest.Value.DistanceBetweenPoints(new GeoPoint(node.Value.Longitude, node.Value.Latitude));
                        var chebyshev = HeuristicFunctions.ChebyshevDistance(dest.Value, new GeoPoint(node.Value.Longitude, node.Value.Latitude));
                        var euclidean = HeuristicFunctions.EuclideanDistance(dest.Value, new GeoPoint(node.Value.Longitude, node.Value.Latitude));
                        var manhattan = HeuristicFunctions.ManhattanDistance(dest.Value, new GeoPoint(node.Value.Longitude, node.Value.Latitude));
                        dest.Value.Adjency.Add(node.Key);
                        SvgHelper.Dictionary[node.Key].Adjency.Add(dest.Key);
                    }
            }

            SvgHelper.Dictionary = SvgHelper.Dictionary.Union(destinations).ToDictionary(d => d.Key, d => d.Value);


            var s = SvgHelper.Dictionary.Values.Sum(x => x.Adjency.Count);
            var m = SvgHelper.Dictionary.Values.Max(x => x.Adjency.Count);

            //pick random start point
            var rnd = new Random();
            ulong startId = SvgHelper.Dictionary.Keys.ElementAt(rnd.Next(SvgHelper.Dictionary.Count));

            var (distD, pD) = TimeHelper.MeasureTimeAlgorithm(() => Dijkstra.Calculate(startId), "Dijkstra");
            var (distL, pL) = TimeHelper.MeasureTimeAlgorithm(() => Levit.Calculate(startId), "Levit");

            // compare Dijkstra and Levit
            var differences = distL.Values.Zip(distD.Values, (d, l) => Math.Abs(d - l)).ToList();
            Console.WriteLine($"sum - {differences.Sum()}\t max - {differences.Max()}\t avg - {differences.Average()}");
            Console.WriteLine(pD.OrderBy(x => x.Key).SequenceEqual(pL.OrderBy(x => x.Key)));

            var heuristicsFunctions = new List<Func<GeoPoint, GeoPoint, double>>
            {
                HeuristicFunctions.ChebyshevDistance,
                HeuristicFunctions.EuclideanDistance,
                HeuristicFunctions.ManhattanDistance,
                HeuristicFunctions.DummyDistance
            };

            var heuristicsDifferences = heuristicsFunctions.ToDictionary(k => k, v => new List<double>());
            foreach (var goalId in destinations.Keys)
            {
                var pathD = RestorePath(pD, startId, goalId).ToList();
                foreach (var function in heuristicsFunctions)
                {
                    var (distA, pa) = Astar.Calculate(goalId: goalId, heuristic: function, startId: startId);//TimeHelper.MeasureTimeAlgorithm(() => Astar.Calculate(heuristic: function, goalId: goalId, startId: startId),$"A* with {function.Method.Name} heuristics"); //
                    heuristicsDifferences[function].Add(Math.Abs(distD[goalId] - distA[goalId]));
                    var pathA = RestorePath(pa, startId, goalId);
                    //Console.WriteLine(pathA.SequenceEqual(pathD));
                }
            }

            foreach (var difference in heuristicsDifferences)
                Console.WriteLine($"For {difference.Key.Method.Name} heuristic\n" +
                                  $"sum - {difference.Value.Sum()}\t max - {difference.Value.Max()}\t avg - {difference.Value.Average()}");
        }

        private static IEnumerable<ulong> RestorePath(IReadOnlyDictionary<ulong, ulong> ancestors, ulong startId, ulong goalId)
        {
            // check if path exist to goalId
            if (!ancestors.ContainsKey(goalId)) return new Stack<ulong>();

            var s = new Stack<ulong>();
            s.Push(goalId);
            do
            {
                s.Push(ancestors[s.Peek()]);
            } while (s.Peek() != startId);

            return s;
        }
    }
}