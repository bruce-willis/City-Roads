using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CityMap.Types.OSM
{
    [XmlType("Node")]
    public class Node
    {
        [XmlElement("tag")]
        public List<Tag> Tags { get; set; }

        [XmlAttribute("id")]
        public ulong Id { get; set; }

        [XmlAttribute("version")]
        public byte Version { get; set; }

        [XmlAttribute("timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlAttribute("uid")]
        public uint Uid { get; set; }

        [XmlAttribute("user")]
        public string User { get; set; }

        [XmlAttribute("changeset")]
        public uint Changeset { get; set; }

        [XmlAttribute("lat")]
        public double Latitude { get; set; }

        [XmlAttribute("lon")]
        public double Longitude { get; set; }
    }
}