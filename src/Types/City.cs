using System.Collections.Generic;
using System.Xml.Serialization;

namespace CityMap.Types
{
    [XmlType("osm")]
    public class City
    {
        [XmlElement("bounds")]
        public Bounds Bounds { get; set; }

        [XmlElement("node")]
        public List<Node> Nodes { get; set; }

        [XmlElement("way")]
        public List<Way> Ways { get; set; }

        [XmlElement("relation")]
        public List<Relation> Relations { get; set; }

        [XmlAttribute("version")]
        public decimal Version { get; set; }

        [XmlAttribute("generator")]
        public string Generator { get; set; }
    }
}