using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CityMap.Types.OSM
{
    [XmlType("relation")]
    public class Relation
    {
        [XmlElement("member")]
        public List<Member> Members { get; set; }

        [XmlElement("tag")]
        public List<Tag> Tags { get; set; }

        [XmlAttribute("id")]
        public ulong Id { get; set; }

        [XmlAttribute("version")]
        public ushort Version { get; set; }

        [XmlAttribute("timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlAttribute("uid")]
        public uint Uid { get; set; }

        [XmlAttribute("user")]
        public string User { get; set; }

        [XmlAttribute("changeset")]
        public uint Changeset { get; set; }
    }
}