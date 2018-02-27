using System.Xml.Serialization;

namespace OSM.Types
{
    [XmlType("nd")]
    public class WayNode
    {
        [XmlAttribute("ref")]
        public ulong Reference { get; set; }
    }
}