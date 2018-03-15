using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CityMap.Types;

namespace CityMap.Helpers
{
    public static class SvgHelper
    {
        public static Dictionary<ulong, GeoPoint> Dictionary;
        public static GeoPoint BasicPoint;
        public static void GenerateSvg(City city, Options options)
        {
            var stopWatch = Stopwatch.StartNew();

            Console.Write("Start generationg svg file. ");

            if (city.Bounds == null)
            {
                city.Bounds = new Bounds
                {
                    MinimumLatitude = city.Nodes.Min(n => n.Latitude),
                    MaximumLatitude = city.Nodes.Max(n => n.Latitude),
                    MinimumLongitude = city.Nodes.Min(n => n.Longitude),
                    MaximumLongitude = city.Nodes.Max(n => n.Longitude)
                };
            }

            BasicPoint = new GeoPoint(city.Bounds.MinimumLongitude, city.Bounds.MaximumLatitude);
            Dictionary = city.Nodes.ToDictionary(n => n.Id, n => new GeoPoint(n.Longitude, n.Latitude));

            var culture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = culture;

            Directory.CreateDirectory(options.OutputDirectory);
            using (var output = new StreamWriter(Path.Combine(options.OutputDirectory, "map.svg")))
            {
                output.WriteLine(
                    "<?xml version=\"1.0\" standalone=\"no\"?>\r\n<svg version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">");

                foreach (var way in city.Ways)
                {
                    var isHighway = way.Tags?.FirstOrDefault(t => t.Key == "highway" && Way.AcceptedHighways.ContainsKey(t.Value));
                    if (isHighway == null) continue;

                    var index = way.Nodes.FindIndex(n => Dictionary.ContainsKey(n.Reference));

                    if (index == -1) continue;

                    var previuosNode = way.Nodes[index].Reference;

                    output.Write("<polyline points=\"");
                    output.Write(GeoHelper.ConvertToGeo(Dictionary[previuosNode]));

                    foreach (var node in way.Nodes.ToList().Skip(index + 1))
                    {
                        if (!Dictionary.ContainsKey(node.Reference)) continue;
                        var point = Dictionary[node.Reference];
                        point.Used = true;

                        point.Adjency.Add(previuosNode);
                        Dictionary[previuosNode].Adjency.Add(node.Reference);

                        output.Write($", {GeoHelper.ConvertToGeo(point)}");
                        previuosNode = node.Reference;
                    }
                    output.WriteLine($"\" stroke=\"{Way.AcceptedHighways[isHighway.Value].color}\" fill=\"transparent\" stroke-width=\"{Way.AcceptedHighways[isHighway.Value].width}\"/>");
                }

                output.WriteLine("</svg>");

                Dictionary = Dictionary.Where(n => n.Value.Used).ToDictionary(n => n.Key, n => n.Value);
                Console.WriteLine($"Elapsed time: {stopWatch.Elapsed}");
                //Console.WriteLine($"There are {edges} of {(ulong)_dictionary.Count * (ulong)_dictionary.Count} edges. It's {edges * 100.0 / _dictionary.Count / _dictionary.Count}% of possible");
            }
        }
    }
}