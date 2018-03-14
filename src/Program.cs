using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CityMap.Types;

namespace CityMap
{
    internal static class Program
    {
        private static string ConvertToGeo(GeoPoint currentPoint, GeoPoint minPoint)
        {
            return $"{(currentPoint.X - minPoint.X) / 100.0} {(currentPoint.Y - minPoint.Y) / 100.0}";
        }

        private static string WriteAdjust(KeyValuePair<ulong, GeoPoint> point, IEnumerable<ulong> keys)
        {
            var sb = new StringBuilder();
            foreach (var key in keys)
            {
                sb.Append(point.Value.Adjency.Contains(key) ? ", 1" : ", 0");
            }
            return sb.ToString();
        }

        private static void Main(string[] args)
        {
            City volgograd;

            using (var reader = new StreamReader("VGG.osm"))
            {
                volgograd = (City)new XmlSerializer(typeof(City)).Deserialize(reader);
            }

            var nodesGeo = new Dictionary<ulong, GeoPoint>(volgograd.Nodes.Length);
            // TODO: if no bounds
            var minimumPoint = new GeoPoint(volgograd.Bounds.MinimumLongitude, volgograd.Bounds.MinimumLatitude);
            var maxPoint = new GeoPoint(volgograd.Bounds.MaximumLongitude, volgograd.Bounds.MaximumLatitude);

            var culture =
                (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;

            const string outputDirectory = "Output";
            Directory.CreateDirectory(outputDirectory);

            using (StreamWriter output = new StreamWriter(Path.Combine(outputDirectory, "map.svg")),
                nodesWrite = new StreamWriter(Path.Combine(outputDirectory, "nodes.csv")),
                adjacencyMatrix = new StreamWriter(Path.Combine(outputDirectory, "adjacency_matrix.csv")),
                adjacencyList = new StreamWriter(Path.Combine(outputDirectory, "adjacency_list.csv")))
            {
                output.WriteLine("<?xml version=\"1.0\" standalone=\"no\"?>\r\n<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">");

                foreach (var node in volgograd.Nodes)
                {
                    nodesGeo.Add(node.Id, new GeoPoint(node.Longitude, node.Latitude));
                    //output.WriteLine($"<circle cx=\"{ConvertCoordinates(node.Longitude - volgograd.Bounds.MinimumLongitude)}\" cy=\"{ConvertCoordinates(node.Latitude - volgograd.Bounds.MinimumLatitude)}\" r=\"0.03\" fill=\"red\" />");
                }

                foreach (var way in volgograd.Ways)
                {
                    if (way.Tags?.FirstOrDefault(t => t.Key == "highway") != null)
                    {
                        var index = way.Nodes.ToList().FindIndex(n => nodesGeo.ContainsKey(n.Reference));

                        if (index == -1) continue;

                        var previuosNode = way.Nodes[index].Reference;

                        output.Write("<polyline points=\"");
                        output.Write(ConvertToGeo(nodesGeo[previuosNode], minimumPoint));

                        foreach (var node in way.Nodes.ToList().Skip(index + 1))
                        {
                            if (nodesGeo.ContainsKey(node.Reference))
                            {
                                var point = nodesGeo[node.Reference];
                                point.Used = true;

                                point.Adjency.Add(previuosNode);
                                nodesGeo[previuosNode].Adjency.Add(node.Reference);


                                output.Write($", {ConvertToGeo(point, minimumPoint)}");
                                previuosNode = node.Reference;
                            }
                            else
                            {
                                Console.WriteLine(node.Reference);
                            }
                        }

                        output.WriteLine(way.Tags?.FirstOrDefault(t => t.Value == "primary") != null
                            ? "\" stroke=\"brown\" fill=\"transparent\" stroke-width=\"0.6\"/>"
                            : "\" stroke=\"blue\" fill=\"transparent\" stroke-width=\"0.1\"/>");
                    }
                }
                output.WriteLine("</svg>");

                nodesGeo = nodesGeo.Where(n => n.Value.Used).ToDictionary(n => n.Key, n => n.Value);

                nodesWrite.WriteLine("Id, Latitude, Longitude, X, Y");
                adjacencyMatrix.WriteLine($", {string.Join(", ", nodesGeo.Keys)}");
                adjacencyList.WriteLine("Node, Adjust");

                var count = 0;
                var edges = 0;
                foreach (var node in nodesGeo.AsParallel())
                {
                    ++count;
                    edges += node.Value.Adjency.Count;
                    nodesWrite.WriteLine(
                        $"{node.Key},{node.Value.Longitude},{node.Value.Latitude},{node.Value.X},{node.Value.Y}");
                    //adjacencyMatrix.WriteLine($"{node.Key} {WriteAdjust(node, nodesGeo.Keys)}");
                    adjacencyList.WriteLine($"{node.Key}:,[{string.Join(" ", node.Value.Adjency)}]");
                    if (count % 1000 == 0)
                    {
                        Console.WriteLine($"Done {count} nodes of {nodesGeo.Count}. It's {count * 100.0 / nodesGeo.Count}%");
                    }
                }
                Console.WriteLine($"There are {edges} of {nodesGeo.Count * nodesGeo.Count} edges. It's {edges * 100.0 / nodesGeo.Count / nodesGeo.Count}% of possible");
            }

            Console.WriteLine("Job done! Now it's time for tea");
        }
    }
}
