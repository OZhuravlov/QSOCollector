using QSOCollector.Models;
using System.Xml.Serialization;

namespace QSOCollector.Parsers
{
    public static class N1mmContactInfoToTableFieldsMapper
    {
        // Parses an N1MM contact info and returns a list of key-value maps for each QSO record
        public static List<Dictionary<string, string>> Map(QsoMessage qsoMessage, string sourceIpAddress)
        {
            string? contactInfoId = null;
            if (string.IsNullOrEmpty(qsoMessage.AdifQsoData))
            {
                qsoMessage.AdifQsoData = DeserializeN1mmContactInfoAndMapToAdif(qsoMessage, out contactInfoId);
            }

            return AdifToTableFieldsMapper.Map(qsoMessage, externalId: contactInfoId, sourceIpAddress: sourceIpAddress);
        }

        private static string DeserializeN1mmContactInfoAndMapToAdif(QsoMessage qsoMessage, out string? id)
        {
            string rootName = qsoMessage.Replace ? "contactreplace" : "contactinfo";
            var serializer = new XmlSerializer(typeof(N1mmContactInfo), new XmlRootAttribute(rootName));
            N1mmContactInfo contactInfo;
            using (var reader = new StringReader(qsoMessage.OriginalQsoData))
            {
                contactInfo = (N1mmContactInfo)serializer.Deserialize(reader)!;
            }
            id = contactInfo.Id;
            return N1mmContactInfoToAdifQsoMessageMapper.Map(contactInfo);
        }
    }
}