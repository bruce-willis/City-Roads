using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CityMap.Types
{
    [XmlType("way")]
    public class Way
    {
        [XmlElement("nd")]
        public List<WayNode> Nodes { get; set; }

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

        public static readonly Dictionary<string, (string color, double width)> AcceptedHighways = new Dictionary<string, (string color, double width)>
        {
            ["motorway"] = ("palevioletred", 1.2),
            ["motorway_link"] = ("palevioletred", 1.2),

            ["trunk"] = ("chocolate", 1.4),
            ["trunk_link"] = ("chocolate", 1.4),

            ["primary"] = ("lightsalmon", 1.2),
            ["primary_link"] = ("lightsalmon", 1.2),

            ["secondary"] = ("indianred", 0.8),
            ["secondary_link"] = ("indianred", 0.8),

            ["tertiary"] = ("darkred", 0.1),
            ["tertiary_link"] = ("darkred", 0.1),

            ["unclassified"] = ("darkred", 0.1),
            ["residential"] = ("darkred", 0.1),
            ["service"] = ("darkred", 0.1),
            ["living_street"] = ("darkred", 0.1),
            ["road"] = ("darkred", 0.1)
        };
    }
}