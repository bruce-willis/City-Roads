using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OSM.Types;

namespace OSM
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


            using (var output = new StreamWriter("map_way.svg"))
            {
                output.WriteLine("<?xml version=\"1.0\" standalone=\"no\"?>\r\n<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">");

                foreach (var node in volgograd.Nodes)
                {
                    nodesGeo.Add(node.Id, (node.Longitude, node.Latitude, false));
                    //output.WriteLine($"<circle cx=\"{ConvertCoordinates(node.Longitude - volgograd.Bounds.MinimumLongitude)}\" cy=\"{ConvertCoordinates(node.Latitude - volgograd.Bounds.MinimumLatitude)}\" r=\"0.03\" fill=\"red\" />");
                }

                foreach (var way in volgograd.Ways)
                {
                    if (way.Tags?.FirstOrDefault(t => t.Key == "highway") != null || true)
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
                        }

                        output.Write(string.Join(", ", nodes));

                        output.WriteLine("\" stroke=\"blue\" fill=\"transparent\" stroke-width=\"0.1\"/>");
                    }
                }

                //foreach (var node in nodesGeo.Values)
                //{
                //    if (node.used)
                //    {
                //        output.WriteLine($"<circle cx=\"{ConvertCoordinates(node.longitude - volgograd.Bounds.MinimumLongitude)}\" cy=\"{ConvertCoordinates(node.latitude - volgograd.Bounds.MinimumLatitude)}\" r=\"0.05\" fill=\"red\" />");
                //    }
                //}

                output.WriteLine("</svg>");
            }

            Console.WriteLine("Hello World!");
        }
    }
}
