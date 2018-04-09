using System.Xml.Serialization;

namespace CityMap.Types.OSM
{
    [XmlType("bounds")]
    public class Bounds
    {
        [XmlAttribute("minlon")]
        public double MinimumLongitude { get; set; }

        [XmlAttribute("minlat")]
        public double MinimumLatitude { get; set; }

        [XmlAttribute("maxlon")]
        public double MaximumLongitude { get; set; }

        [XmlAttribute("maxlat")]
        public double MaximumLatitude { get; set; }

        [XmlAttribute("origin")]
        public string Origin { get; set; }
    }
}