using System.Xml.Serialization;

namespace OSM.Types
{
    [XmlType("osm")]
    public class City
    {
        [XmlElement("bounds")]
        public Bounds Bounds { get; set; }

        [XmlElement("node")]
        public Node[] Nodes { get; set; }

        [XmlElement("way")]
        public Way[] Ways { get; set; }

        [XmlElement("relation")]
        public Relation[] Relations { get; set; }

        [XmlAttribute("version")]
        public decimal Version { get; set; }

        [XmlAttribute("generator")]
        public string Generator { get; set; }
    }
}