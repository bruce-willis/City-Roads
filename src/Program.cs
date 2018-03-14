using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using CityMap.Helpers;
using CityMap.Types;
using CommandLine;
using CommandLine.Text;

namespace CityMap
{
    internal static class Program
    {
        private static City _city;
        private static Options _options = new Options();
        

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(res => _options = res)
                .WithNotParsed(err => Environment.Exit(1));

            if (!File.Exists(_options.FileName))
            {
                Console.WriteLine($"File {_options.FileName} doesn't exist.");
                return;
            }

            try
            {
                using (var reader = new StreamReader(_options.FileName))
                    _city = (City) new XmlSerializer(typeof(City)).Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to parse osm file: {e.Message}. See about downloading here: https://github.com/bruce-willis/City-Roads/blob/develop/docs/download.md");
            }



            SvgHelper.GenerateSvg(_city, _options);

            if (_options.GenerateNodesList)
                CsvHelper.WriteNodesInfo(_options.OutputDirectory);

            if (_options.GenerateAdjacencyList)
                CsvHelper.WriteAdjacencyList(_options.OutputDirectory);

            if (_options.GenerateAdjacencyMatrix)
                CsvHelper.WriteAdjacencyMatrix(_options.OutputDirectory);

            
            Console.WriteLine("Job done! Now it's time for tea");
        }
    }
}