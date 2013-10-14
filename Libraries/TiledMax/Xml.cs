using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TiledMax
{
    public static class Xml
    {
        public static string ReadTag(this XmlNode node, string tag_name, string default_value = "")
        {
            if (node.Attributes != null)
            {
                return node.Attributes[tag_name] != null ? node.Attributes[tag_name].Value : default_value;
            }
            else
            {
                return default_value;
            }
        }

        public static int ReadInt(this XmlNode node, string tag_name, int default_value = 0)
        {
            if (node.Attributes != null)
            {
                return node.Attributes[tag_name] != null ? int.Parse(node.Attributes[tag_name].Value) : default_value;
            }
            else
            {
                return default_value;
            }
        }

        public static double ReadDouble(this XmlNode node, string tag_name, double default_value = 1)
        {
            if (node.Attributes != null)
            {
                return node.Attributes[tag_name] != null ? double.Parse(node.Attributes[tag_name].Value) : default_value;
            }
            else
            {
                return default_value;
            }
        }
    }
}
