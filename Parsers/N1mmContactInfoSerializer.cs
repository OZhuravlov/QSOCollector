using QSOCollector.Models;
using System.Xml.Serialization;
using System.Xml;

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
            using var writer = new StringWriter();
            using var xmlWriter =XmlWriter.Create(writer, GetXmlWriterSettings());
            serializer.Serialize(xmlWriter, contactInfo, namespaces);
            return writer.ToString();
        }

        private static XmlWriterSettings GetXmlWriterSettings()
        {
            return new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false,
                NewLineHandling = NewLineHandling.None,
                NewLineChars = string.Empty
            };
        }
    }
}