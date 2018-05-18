using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CityMap.Algorithms;
using CityMap.Algorithms.Shortest_Path;
using CityMap.Types;
using CityMap.Types.OSM;

namespace CityMap.Helpers
{
    public static class DistanceHelper
    {
        public static Dictionary<ulong, GeoPoint> Destinations;
        private const string Amenity = "college";
        private const double SmallDistance = 0.3;

        public static void AddNodes(City city)
        {
            Destinations = city.Nodes.Where(n => n.Tags.Exists(t => t.Key == "amenity" && t.Value == Amenity))
                .ToDictionary(k => k.Id, v => new GeoPoint(v.Longitude, v.Latitude));

            var size = Destinations.Count;

            foreach (var node in SvgHelper.Dictionary)
            {
                foreach (var dest in Destinations)
                    if (dest.Value.DistanceBetweenPoints(new GeoPoint(node.Value.Longitude, node.Value.Latitude)) <
                        SmallDistance)
                    {
                        var realDistance =
                            dest.Value.DistanceBetweenPoints(new GeoPoint(node.Value.Longitude, node.Value.Latitude));
                        var chebyshev = HeuristicFunctions.ChebyshevDistance(dest.Value,
                            new GeoPoint(node.Value.Longitude, node.Value.Latitude));
                        var euclidean = HeuristicFunctions.EuclideanDistance(dest.Value,
                            new GeoPoint(node.Value.Longitude, node.Value.Latitude));
                        var manhattan = HeuristicFunctions.ManhattanDistance(dest.Value,
                            new GeoPoint(node.Value.Longitude, node.Value.Latitude));
                        dest.Value.Adjency.Add(node.Key);
                        SvgHelper.Dictionary[node.Key].Adjency.Add(dest.Key);
                    }
            }

            SvgHelper.Dictionary = SvgHelper.Dictionary.Union(Destinations).ToDictionary(d => d.Key, d => d.Value);


            //var s = SvgHelper.Dictionary.Values.Sum(x => x.Adjency.Count);
            //var m = SvgHelper.Dictionary.Values.Max(x => x.Adjency.Count);
            //var a = SvgHelper.Dictionary.Values.Average(x => x.Adjency.Count);
        }

        public static void CompareAlgorithms(string outputDirectory)
        {
            //pick random start point
            var rnd = new Random();
            ulong startId = SvgHelper.Dictionary.Keys.ElementAt(rnd.Next(SvgHelper.Dictionary.Count));

            var (distD, pD) = TimeHelper.MeasureTimeAlgorithm(() => Dijkstra.Calculate(startId), "Dijkstra");
            var (distL, pL) = TimeHelper.MeasureTimeAlgorithm(() => Levit.Calculate(startId), "Levit");

            // compare Dijkstra and Levit
            var differences = distL.Values.Zip(distD.Values, (d, l) => Math.Abs(d - l)).ToList();
            Console.WriteLine("Compare results for Dijkstra and Levit algorithms:");
            Console.WriteLine($"Difference between distances:\t sum - {differences.Sum()}\t max - {differences.Max()}\t avg - {differences.Average()}");
            Console.WriteLine($"Ancestors lists are {(pD.OrderBy(x => x.Key).SequenceEqual(pL.OrderBy(x => x.Key)) ? "equal" : "not equal")}");

            TimeHelper.MeasureTime(() => CsvHelper.WriteShortestPathes(outputDirectory, startId, Destinations.Keys.OrderBy(x => distD[x]), distD, pD), "writing shortest pathes to csv");
            TimeHelper.MeasureTime(() => SvgHelper.DisplayShortestPathes(outputDirectory, startId, Destinations.Keys.OrderBy(x => distD[x]), pD), "writing shortest pathes to svg");

            var heuristicsFunctions =
                new Dictionary<Func<GeoPoint, GeoPoint, double>, (List<double> differences, List<long> timings)>
                {
                    [HeuristicFunctions.ChebyshevDistance] = ValueTuple.Create(new List<double>(), new List<long>()),
                    [HeuristicFunctions.DummyDistance] = ValueTuple.Create(new List<double>(), new List<long>()),
                    [HeuristicFunctions.EuclideanDistance] = ValueTuple.Create(new List<double>(), new List<long>()),
                    [HeuristicFunctions.ManhattanDistance] = ValueTuple.Create(new List<double>(), new List<long>())
                };

            TimeHelper.MeasureTime(() =>
            {
                foreach (var goalId in Destinations.Keys)
                {
                    var pathD = RestorePath(startId, goalId, pD).ToList();
                    foreach (var function in heuristicsFunctions)
                    {
                        var timer = Stopwatch.StartNew();
                        var (distA, pa) = Astar.Calculate(startId, goalId, function.Key);
                        function.Value.timings.Add(timer.Elapsed.Ticks);
                        function.Value.differences.Add(Math.Abs(distD[goalId] - distA[goalId]));
                        var pathA = RestorePath(startId, goalId, pa);
                        //Console.WriteLine(pathA.SequenceEqual(pathD));
                    }
                }
            }, $"A* with {heuristicsFunctions.Count} different heuristics");

            foreach (var function in heuristicsFunctions)
            {
                Console.WriteLine($"{function.Key.Method.Name} - difference between distances with Dijkstra: " +
                                  $"sum - {function.Value.differences.Sum()} max - {function.Value.differences.Max()} avg - {function.Value.differences.Average()}");
                Console.WriteLine($"timing information:\tsum - {new TimeSpan(function.Value.timings.Sum())}\t max - {new TimeSpan(function.Value.timings.Max())}\t avg - {new TimeSpan(Convert.ToInt64(function.Value.timings.Average()))}");
            }
        }

        public static IEnumerable<ulong> RestorePath(ulong startId, ulong goalId,
            IReadOnlyDictionary<ulong, ulong> ancestors)
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