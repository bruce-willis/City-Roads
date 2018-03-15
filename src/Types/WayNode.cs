using System.Xml.Serialization;

namespace CityMap.Types
{
    [XmlType("nd")]
    public class WayNode
    {
        [XmlAttribute("ref")]
        public ulong Reference { get; set; }
    }
}