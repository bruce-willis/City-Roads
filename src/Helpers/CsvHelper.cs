using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CityMap.Types;

namespace CityMap.Helpers
{
    public static class CsvHelper
    {
        public static void WriteNodesInfo(string outputDirectory)
        {
            using (var nodeWriter = new StreamWriter(Path.Combine(outputDirectory, "nodes_list.csv")))
            {
                nodeWriter.WriteLine("Id, Latitude, Longitude, X, Y");
                foreach (var node in SvgHelper.Dictionary)
                {
                    nodeWriter.WriteLine(
                        $"{node.Key},{node.Value.Longitude},{node.Value.Latitude},{node.Value.X},{node.Value.Y}");
                }
            }
        }

        public static void WriteAdjacencyList(string outputDirectory)
        {
            using (var adjacencyList = new StreamWriter(Path.Combine(outputDirectory, "adjacency_list.csv")))
            {
                adjacencyList.WriteLine("Node, Adjacent nodes");
                foreach (var node in SvgHelper.Dictionary)
                {
                    adjacencyList.WriteLine($"{node.Key}:,[{string.Join(" ", node.Value.Adjency)}]");
                }
            }
        }

        private static string AdjustMatrixRow(KeyValuePair<ulong, GeoPoint> point, IEnumerable<ulong> keys)
        {
            var sb = new StringBuilder(keys.Count());
            foreach (var key in keys) sb.Append(point.Value.Adjency.Contains(key) ? ", 1" : ", 0");
            return sb.ToString();
        }

        public static void WriteAdjacencyMatrix(string outputDirectory)
        {
            var stopWatch = Stopwatch.StartNew();
            Console.WriteLine("You better be patient");
            var count = 0;
            using (var adjacencyMatrix = new StreamWriter(Path.Combine(outputDirectory, "adjacency_matrix.csv")))
            {
                adjacencyMatrix.WriteLine($", {string.Join(", ", SvgHelper.Dictionary.Keys)}");
                foreach (var node in SvgHelper.Dictionary)
                {
                    adjacencyMatrix.WriteLine($"{node.Key} {AdjustMatrixRow(node, SvgHelper.Dictionary.Keys)}");
                    if (++count % 1000 == 0)
                        Console.WriteLine(
                            $"Done {count} nodes of {SvgHelper.Dictionary.Count}. It's {count * 100.0 / SvgHelper.Dictionary.Count}%. Elapsed time: {stopWatch.Elapsed}");
                }
            }
        }

        public static void WriteShortestPathes(string outputDirectory, ulong startId, IEnumerable<ulong> goals, IReadOnlyDictionary<ulong, ulong> ancestors)
        {
            using (var pathWriter = new StreamWriter(Path.Combine(outputDirectory, "shrotest_pathes.csv")))
            {
                pathWriter.WriteLine("Start id, Goal id, Nodes id in shortest path");
                foreach (var goalId in goals)
                {
                    pathWriter.WriteLine($"{startId}, {goalId}, [{string.Join("->", DistanceHelper.RestorePath(startId, goalId, ancestors))}]");
                }
            }
        }
    }
}