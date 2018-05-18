using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CityMap.Algorithms.Travelling_salesman;
using CityMap.Types;
using CityMap.Types.OSM;

namespace CityMap.Helpers
{
    public static class SvgHelper
    {
        public static Dictionary<ulong, GeoPoint> Dictionary;
        public static GeoPoint BasicPoint;
        public static void GenerateSvg(City city, Options options)
        {
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
                    Dictionary[previuosNode].Used = true;

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
                //Console.WriteLine($"There are {edges} of {(ulong)_dictionary.Count * (ulong)_dictionary.Count} edges. It's {edges * 100.0 / _dictionary.Count / _dictionary.Count}% of possible");
            }
        }

        public static void DisplayShortestPathes(string outputDirectory, ulong startId, IEnumerable<ulong> goals,
            IReadOnlyDictionary<ulong, ulong> ancestors)
        {
            var lines = File.ReadAllLines(Path.Combine(outputDirectory, "map.svg")).SkipLast(1).ToList();
            File.WriteAllLines(Path.Combine(outputDirectory, "pathes.svg"), lines);
            using (var output = new StreamWriter(Path.Combine(outputDirectory, "pathes.svg"), true))
            {
                string[] coordinates;
                foreach (var goalId in goals.Skip(1))
                {
                    output.WriteLine($"<polyline points=\"{string.Join(", ", DistanceHelper.RestorePath(startId, goalId, ancestors).Select(x => GeoHelper.ConvertToGeo(Dictionary[x])))}\" " +
                                 "stroke=\"darkcyan\" fill=\"transparent\" stroke-width=\"2\"/>");
                    coordinates = GeoHelper.ConvertToGeo(Dictionary[goalId]).Split();
                    output.WriteLine($"<circle cx=\"{coordinates.First()}\" cy=\"{coordinates.Last()}\" r=\"3\" fill=\"navy(16)\" />");
                }
                output.WriteLine($"<polyline points=\"{string.Join(", ", DistanceHelper.RestorePath(startId, goals.First(), ancestors).Select(x => GeoHelper.ConvertToGeo(Dictionary[x])))}\" " +
                                 "stroke=\"cornflowerblue\" fill=\"transparent\" stroke-width=\"2.5\"/>");
                coordinates = GeoHelper.ConvertToGeo(Dictionary[goals.First()]).Split();
                output.WriteLine($"<circle cx=\"{coordinates.First()}\" cy=\"{coordinates.Last()}\" r=\"3.5\" fill=\"mediumblue\" />");
                coordinates = GeoHelper.ConvertToGeo(Dictionary[startId]).Split();
                output.WriteLine($"<circle cx=\"{coordinates.First()}\" cy=\"{coordinates.Last()}\" r=\"4\" fill=\"limegreen\" />");
                output.WriteLine("</svg>");
            }
        }

        public static void DisplaySalesmanPath(string outputDirectory, string filename, IReadOnlyList<ulong> order)
        {
            var lines = File.ReadAllLines(Path.Combine(outputDirectory, "map.svg")).SkipLast(1).ToList();
            File.WriteAllLines(Path.Combine(outputDirectory, $"salesman_{filename}.svg"), lines);
            using (var output = new StreamWriter(Path.Combine(outputDirectory, $"salesman_{filename}.svg"), true))
            {
                var coordinates = GeoHelper.ConvertToGeo(Dictionary[order.First()]).Split();
                output.WriteLine($"<circle cx=\"{coordinates.First()}\" cy=\"{coordinates.Last()}\" r=\"7\" fill=\"cornflowerblue\" />");

                for (int i = 0; i < order.Count - 1; ++i)
                {
                    output.WriteLine($"<polyline points=\"{string.Join(", ", CommonSalesman.Distances[(order[i], order[i + 1])].path.Select(x => GeoHelper.ConvertToGeo(Dictionary[x])))}\" " +
                                      "stroke=\"darkcyan\" fill=\"transparent\" stroke-width=\"2\"/>");
                    coordinates = GeoHelper.ConvertToGeo(Dictionary[order[i]]).Split();
                    output.WriteLine($"<circle cx=\"{coordinates.First()}\" cy=\"{coordinates.Last()}\" r=\"7\" stroke=\"darkblue\" stroke-width=\"1\" fill=\"none\"/> <text x=\"{coordinates.First()}\" y=\"{coordinates.Last()}\" text-anchor=\"middle\" stroke=\"black\" stroke-width=\"1px\" dy=\".3em\">{i}</text>");
                }
                output.WriteLine($"<polyline points=\"{string.Join(", ", CommonSalesman.Distances[(order.Last(), order.First())].path.Select(x => GeoHelper.ConvertToGeo(Dictionary[x])))}\" " +
                                 "stroke=\"darkcyan\" fill=\"transparent\" stroke-width=\"2\"/>");
                coordinates = GeoHelper.ConvertToGeo(Dictionary[order.Last()]).Split();
                output.WriteLine($"<circle cx=\"{coordinates.First()}\" cy=\"{coordinates.Last()}\" r=\"7\" stroke=\"darkblue\" stroke-width=\"1\" fill=\"none\"/> <text x=\"{coordinates.First()}\" y=\"{coordinates.Last()}\" text-anchor=\"middle\" stroke=\"black\" stroke-width=\"1px\" dy=\".3em\">{order.Count - 1}</text>");
                output.WriteLine("</svg>");
            }
        }
    }
}