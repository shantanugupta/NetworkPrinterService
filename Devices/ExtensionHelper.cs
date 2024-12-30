using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml;

namespace Devices
{
    public static class ExtensionHelper
    {
        public static Dictionary<string, string> GetAttributesDictionary(this XmlAttributeCollection attributes)
        {
            Dictionary<string, string> attributesDict = new Dictionary<string, string>();

            if (attributes != null)
            {
                foreach (XmlAttribute attribute in attributes)
                {
                    attributesDict[attribute.Name] = attribute.Value;
                }
            }

            return attributesDict;
        }

        public static XmlNode FindElementByName(this XmlNode parent, string name)
        {
            foreach (XmlNode childNode in parent.ChildNodes)
            {
                if (childNode.Name == name)
                {
                    return childNode;
                }
            }
            return null;
        }

        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
        }

        public static string GetName(this Enum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            DisplayAttribute[] attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            var name = attributes.Length > 0 ? attributes[0].Name : enumValue.ToString();
            return name;
        }
    }
}