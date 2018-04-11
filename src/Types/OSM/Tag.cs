using System.Xml.Serialization;

namespace CityMap.Types.OSM
{
    [XmlType("tag")]
    public class Tag
    {
        [XmlAttribute("k")]
        public string Key { get; set; }

        [XmlAttribute("v")]
        public string Value { get; set; }
    }
}