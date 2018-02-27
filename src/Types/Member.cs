using System.Xml.Serialization;

namespace OSM.Types
{
    [XmlType("member")]
    public class Member
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("ref")]
        public ulong Reference { get; set; }

        [XmlAttribute("role")]
        public string Role { get; set; }
    }
}