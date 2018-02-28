using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CityMap.Types;

namespace CityMap
{
    internal static class Program
    {
        private static string ConvertCoordinates(double coordinate)
        {
            return (coordinate * 1000).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private static void Main(string[] args)
        {
            City volgograd;

            using (var reader = new StreamReader("volgograd.osm"))
            {
                volgograd = (City)new XmlSerializer(typeof(City)).Deserialize(reader);
            }

            var nodesGeo = new Dictionary<ulong, (double longitude, double latitude, bool used)>(volgograd.Nodes.Length);

            var culture =
                (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;

            using (StreamWriter output = new StreamWriter("map.svg"),
                nodesWrite = new StreamWriter("nodes.csv"),
                adjacencyMatrix = new StreamWriter("adjacency_matrix.csv"),
                adjacencyList = new StreamWriter("adjacency_list.csv"))
            {
                output.WriteLine("<?xml version=\"1.0\" standalone=\"no\"?>\r\n<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">");

                foreach (var node in volgograd.Nodes)
                {
                    nodesGeo.Add(node.Id, (node.Longitude, node.Latitude, false));
                    //output.WriteLine($"<circle cx=\"{ConvertCoordinates(node.Longitude - volgograd.Bounds.MinimumLongitude)}\" cy=\"{ConvertCoordinates(node.Latitude - volgograd.Bounds.MinimumLatitude)}\" r=\"0.03\" fill=\"red\" />");
                }

                foreach (var way in volgograd.Ways)
                {
                    if (way.Tags?.FirstOrDefault(t => t.Key == "highway") != null)
                    {
                        output.Write("<polyline points=\"");
                        IList<string> nodes = new List<string>(way.Nodes.Length);
                        foreach (var node in way.Nodes)
                        {
                            if (nodesGeo.ContainsKey(node.Reference))
                            {
                                var (longitude, latitude, _) = nodesGeo[node.Reference];
                                nodesGeo[node.Reference] = (longitude, latitude, true);
                                nodes.Add(
                                    $"{ConvertCoordinates(longitude - volgograd.Bounds.MinimumLongitude)} {ConvertCoordinates(latitude - volgograd.Bounds.MinimumLatitude)}");
                            }
                            else
                            {
                                Console.WriteLine(node.Reference);
                            }
                        }

                        output.Write(string.Join(", ", nodes));

                        output.WriteLine("\" stroke=\"blue\" fill=\"transparent\" stroke-width=\"0.1\"/>");
                    }
                }
                nodesWrite.WriteLine("id,longitude,latitude");

                foreach (var node in nodesGeo)
                {
                    if (node.Value.used)
                    {
                        //output.WriteLine($"<circle cx=\"{ConvertCoordinates(node.Value.longitude - volgograd.Bounds.MinimumLongitude)}\" cy=\"{ConvertCoordinates(node.Value.latitude - volgograd.Bounds.MinimumLatitude)}\" r=\"0.05\" fill=\"red\" />");
                        nodesWrite.WriteLine($"{node.Key},{node.Value.longitude},{node.Value.latitude}");
                    }
                }

                output.WriteLine("</svg>");
            }

            Console.WriteLine("Hello World!");
        }
    }
}
