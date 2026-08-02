using QSOCollector.Models;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace QSOCollector.Parsers
{
    public static class N1mmContactInfoSerializer
    {
        public static N1mmContactInfo Deserialize(string qsoData, string rootName)
        {
            var serializer = new XmlSerializer(typeof(N1mmContactInfo), new XmlRootAttribute(rootName));
            using var reader = new StringReader(qsoData);
            return (N1mmContactInfo)serializer.Deserialize(reader)!;
        }

        public static string Serialize(N1mmContactInfo contactInfo, string rootName)
        {
            XmlSerializerNamespaces namespaces = new ();
            namespaces.Add(string.Empty, string.Empty);
            var serializer = new XmlSerializer(contactInfo.GetType(), new XmlRootAttribute(rootName));
            using var writer = new Utf8StringWriter();
            using var xmlWriter =XmlWriter.Create(writer, GetXmlWriterSettings());
            serializer.Serialize(xmlWriter, contactInfo, namespaces);
            return writer.ToString();
        }

        private static XmlWriterSettings GetXmlWriterSettings()
        {
            return new XmlWriterSettings
            {
                Indent = false,
                NewLineHandling = NewLineHandling.None,
                NewLineChars = string.Empty
            };
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}