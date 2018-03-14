using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using CityMap.Types;

namespace CityMap
{
    internal static class Program
    {
        private const string OutputDirectory = "Output";
        private static Dictionary<ulong, GeoPoint> _dictionary;
        private static City _city;

        private static string ConvertToGeo(GeoPoint currentPoint, GeoPoint basicPoint)
        {
            return $"{(currentPoint.X - basicPoint.X) / 100.0} {(basicPoint.Y - currentPoint.Y) / 100.0}";
        }

        private static void WriteNodesInfo()
        {
            using (var nodeWriter = new StreamWriter(Path.Combine(OutputDirectory, "nodes.csv")))
            {
                nodeWriter.WriteLine("Id, Latitude, Longitude, X, Y");
                foreach (var node in _dictionary)
                {
                    nodeWriter.WriteLine(
                        $"{node.Key},{node.Value.Longitude},{node.Value.Latitude},{node.Value.X},{node.Value.Y}");
                }
            }
        }

        private static void WriteAdjacencyList()
        {
            using (var adjacencyList = new StreamWriter(Path.Combine(OutputDirectory, "adjacency_list.csv")))
            {
                adjacencyList.WriteLine("Node, Adjacent nodes");
                foreach (var node in _dictionary)
                {
                    adjacencyList.WriteLine($"{node.Key}:,[{string.Join(" ", node.Value.Adjency)}]");
                }
            }
        }

        private static string AdjustMatrixRow(KeyValuePair<ulong, GeoPoint> point, IEnumerable<ulong> keys)
        {
            var sb = new StringBuilder();
            foreach (var key in keys) sb.Append(point.Value.Adjency.Contains(key) ? ", 1" : ", 0");
            return sb.ToString();
        }

        private static void WriteAdjacencyMatrix()
        {
            var count = 0;
            using (var adjacencyMatrix = new StreamWriter(Path.Combine(OutputDirectory, "adjacency_matrix.csv")))
            {
                adjacencyMatrix.WriteLine($", {string.Join(", ", _dictionary.Keys)}");
                foreach (var node in _dictionary)
                {
                    adjacencyMatrix.WriteLine($"{node.Key} {AdjustMatrixRow(node, _dictionary.Keys)}");
                    if (++count % 1000 == 0)
                        Console.WriteLine(
                            $"Done {count} nodes of {_dictionary.Count}. It's {count * 100.0 / _dictionary.Count}%");
                }
            }
        }

        private static void Main(string[] args)
        {
            var culture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = culture;

            using (var reader = new StreamReader("VGG.osm"))
            {
                _city = (City)new XmlSerializer(typeof(City)).Deserialize(reader);
            }

            if (_city.Bounds == null)
            {
                _city.Bounds = new Bounds
                {
                    MinimumLatitude = _city.Nodes.Min(n => n.Latitude),
                    MaximumLatitude = _city.Nodes.Max(n => n.Latitude),
                    MinimumLongitude = _city.Nodes.Min(n => n.Longitude),
                    MaximumLongitude = _city.Nodes.Max(n => n.Longitude)
                };
            }

            var basicPoint = new GeoPoint(_city.Bounds.MinimumLongitude, _city.Bounds.MaximumLatitude);

            _dictionary = new Dictionary<ulong, GeoPoint>(_city.Nodes.Count);

            foreach (var node in _city.Nodes)
                _dictionary.Add(node.Id, new GeoPoint(node.Longitude, node.Latitude));

            Directory.CreateDirectory(OutputDirectory);

            using (var output = new StreamWriter(Path.Combine(OutputDirectory, "map.svg")))
            {
                output.WriteLine(
                    "<?xml version=\"1.0\" standalone=\"no\"?>\r\n<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">");

                var highways = new Dictionary<string, (string color, double width)>
                {
                    ["motorway"] = ("palevioletred", 1.2),
                    ["motorway_link"] = ("palevioletred", 1.2),

                    ["trunk"] = ("chocolate", 1.4),
                    ["trunk_link"] = ("chocolate", 1.4),

                    ["primary"] = ("lightsalmon", 1.2),
                    ["primary_link"] = ("lightsalmon", 1.2),

                    ["secondary"] = ("indianred", 0.8),
                    ["secondary_link"] = ("indianred", 0.8),

                    ["tertiary"] = ("darkred", 0.1),
                    ["tertiary_link"] = ("darkred", 0.1),

                    ["unclassified"] = ("darkred", 0.1),
                    ["residential"] = ("darkred", 0.1),
                    ["service"] = ("darkred", 0.1),
                    ["living_street"] = ("darkred", 0.1),
                    ["road"] = ("darkred", 0.1)
                };

                foreach (var way in _city.Ways)
                {
                    var isHighway = way.Tags?.FirstOrDefault(t => t.Key == "highway" && highways.ContainsKey(t.Value));
                    if (isHighway == null) continue;

                    var index = way.Nodes.FindIndex(n => _dictionary.ContainsKey(n.Reference));

                    if (index == -1) continue;

                    var previuosNode = way.Nodes[index].Reference;

                    output.Write("<polyline points=\"");
                    output.Write(ConvertToGeo(_dictionary[previuosNode], basicPoint));

                    foreach (var node in way.Nodes.ToList().Skip(index + 1))
                        if (_dictionary.ContainsKey(node.Reference))
                        {
                            var point = _dictionary[node.Reference];
                            point.Used = true;

                            point.Adjency.Add(previuosNode);
                            _dictionary[previuosNode].Adjency.Add(node.Reference);


                            output.Write($", {ConvertToGeo(point, basicPoint)}");
                            previuosNode = node.Reference;
                        }
                        else
                        {
                            Console.WriteLine(node.Reference);
                        }

                    output.WriteLine($"\" stroke=\"{highways[isHighway.Value].color}\" fill=\"transparent\" stroke-width=\"{highways[isHighway.Value].width}\"/>");
                }

                output.WriteLine("</svg>");

                _dictionary = _dictionary.Where(n => n.Value.Used).ToDictionary(n => n.Key, n => n.Value);
            }

            WriteNodesInfo();
            WriteAdjacencyList();
            //WriteAdjacencyMatrix();
            //Console.WriteLine($"There are {edges} of {(ulong)_dictionary.Count * (ulong)_dictionary.Count} edges. It's {edges * 100.0 / _dictionary.Count / _dictionary.Count}% of possible");
            Console.WriteLine("Job done! Now it's time for tea");
        }
    }
}