using CommandLine;

namespace CityMap.Types
{
    public class Options
    {
        [Option('f', "file-name", Default = "map.osm", HelpText = "Name of the OSM file")]
        public string FileName { get; set; }

        [Option('o', "output", Default = "Output", HelpText = "Output directory name")]
        public string OutputDirectory { get; set; }

        [Option('n', "write-nodes", Default = false, HelpText = "Create csv file with information about nodes")]
        public bool GenerateNodesList { get; set; }

        [Option('l', "write-list", Default = false, HelpText = "Create csv file with adjacency list")]
        public bool GenerateAdjacencyList { get; set; }

        [Option('m', "write-matrix", Default = false,
            HelpText =
                "Create csv file with adjacency matrix. Be very careful! It's very long process (~30 min for 80 Mb of input file) " +
                "and the output file will be very-very-very big (~41 Gb in my case)")]
        public bool GenerateAdjacencyMatrix { get; set; }
    }
}