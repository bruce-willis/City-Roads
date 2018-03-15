using System.Xml.Serialization;

namespace CityMap.Types
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