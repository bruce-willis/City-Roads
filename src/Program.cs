using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using CityMap.Algorithms.Travelling_salesman;
using CityMap.Helpers;
using CityMap.Types;
using CityMap.Types.OSM;
using CommandLine;

namespace CityMap
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Options options = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(res => options = res)
                .WithNotParsed(err => Environment.Exit(1));

            var stopWatch = Stopwatch.StartNew();

            if (!File.Exists(options.FileName))
            {
                Console.WriteLine($"File {options.FileName} doesn't exist.");
                return;
            }

            Console.Write($"Start parsing file: {options.FileName} ");

            City city;
            try
            {
                using (var reader = new StreamReader(options.FileName))
                {
                    city = (City)new XmlSerializer(typeof(City)).Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    $"Unable to parse osm file: {e.Message}. See about downloading here: https://github.com/bruce-willis/City-Roads/blob/develop/docs/download.md");
                return;
            }

            Console.WriteLine($"Elapsed time: {stopWatch.Elapsed}");

            TimeHelper.MeasureTime(() => SvgHelper.GenerateSvg(city, options), "generationg svg file");

            DistanceHelper.AddNodes(city);
            //DistanceHelper.CompareAlgorithms(options.OutputDirectory);

            TimeHelper.MeasureTime(() => Console.Write(CommonSalesman.Distances.Count), "calculating distances and pathes between destinations");
            TimeHelper.MeasureTime(() => NearestNeighbour.Calculate(options.OutputDirectory), "solving travelling salesman problem using nearest neighbour");
            TimeHelper.MeasureTime(() => NearestNeighbour.Calculate(options.OutputDirectory, withRandom: true), "solving travelling salesman problem using nearest neighbour and random");
            //int i = 0;
            //double d1, d2;
            //do
            //{
            //    d1 = NearestNeighbour.Calculate(city, options.OutputDirectory);
            //    d2 = NearestNeighbour.CalculateWithRandom(city, options.OutputDirectory);
            //    Console.WriteLine(i++);
            //} while (d1 <= d2);
            //Console.WriteLine(d1);
            //Console.WriteLine(d2);
            TimeHelper.MeasureTime(() => SimulatedAnnealing.Calculate(10, 0.00001, options.OutputDirectory), "solving tsp using simulated annealing");

            if (options.GenerateNodesList)
                TimeHelper.MeasureTime(() => CsvHelper.WriteNodesInfo(options.OutputDirectory), "creating csv with nodes' information");

            if (options.GenerateAdjacencyList)
                TimeHelper.MeasureTime(() => CsvHelper.WriteAdjacencyList(options.OutputDirectory), "creating csv with adjacency list");

            if (options.GenerateAdjacencyMatrix)
                TimeHelper.MeasureTime(() => CsvHelper.WriteAdjacencyMatrix(options.OutputDirectory), "creating csv with adjacency matrix");


            Console.WriteLine($"\nJob done! Now it's time for tea. Total time elapsed: {stopWatch.Elapsed}");
            Console.WriteLine(new Random().Next(0, 2) == 1
                ? "Лучший в СПбГУ - Факультет ПМ-ПУ"
                : "Ответ на главный вопрос жизни, вселенной и всего такого - 42");
        }
    }
}