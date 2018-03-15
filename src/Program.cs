using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using CityMap.Helpers;
using CityMap.Types;
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

            Console.WriteLine($"Start parsing file: {options.FileName}");

            City city = null;
            try
            {
                using (var reader = new StreamReader(options.FileName))
                {
                    city = (City) new XmlSerializer(typeof(City)).Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    $"Unable to parse osm file: {e.Message}. See about downloading here: https://github.com/bruce-willis/City-Roads/blob/develop/docs/download.md");
            }

            Console.WriteLine($"End parsing file. Spent time: {stopWatch.Elapsed}");

            SvgHelper.GenerateSvg(city, options);

            if (options.GenerateNodesList)
                CsvHelper.WriteNodesInfo(options.OutputDirectory);

            if (options.GenerateAdjacencyList)
                CsvHelper.WriteAdjacencyList(options.OutputDirectory);

            if (options.GenerateAdjacencyMatrix)
                CsvHelper.WriteAdjacencyMatrix(options.OutputDirectory);


            Console.WriteLine($"\nJob done! Now it's time for tea. Total time elapsed: {stopWatch.Elapsed}");
            Console.WriteLine(new Random().Next(0, 2) == 1
                ? "Лучший в СПбГУ - Факультет ПМ-ПУ"
                : "Ответ на главный вопрос жизни, вселенной и всего такого - 42");
        }
    }
}