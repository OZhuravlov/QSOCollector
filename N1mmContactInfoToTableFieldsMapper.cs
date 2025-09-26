using System.Xml.Serialization;

namespace QSOCollector
{
    public static class N1mmContactInfoToTableFieldsMapper
    {
        // Parses an N1MM contact info and returns a list of key-value maps for each QSO record
        public static List<Dictionary<string, string>> Map(QsoMessage qsoMessage, string sourceIpAddress)
        {
            if (string.IsNullOrEmpty(qsoMessage.AdifQsoData)) {
                qsoMessage.AdifQsoData = DeserializeN1mmContactInfoAndMapToAdif(qsoMessage);
            }

            return AdifToTableFieldsMapper.Map(qsoMessage, sourceIpAddress);
        }

        private static string DeserializeN1mmContactInfoAndMapToAdif(QsoMessage qsoMessage)
        {
            var serializer = new XmlSerializer(typeof(N1mmContactInfo));
            N1mmContactInfo contactInfo;
            using (var reader = new StringReader(qsoMessage.OriginalQsoData))
            {
                contactInfo = (N1mmContactInfo)serializer.Deserialize(reader)!;

            }
            qsoMessage.Source = contactInfo.App;
            return N1mmContactInfoToAdifQsoMessageMapper.Map(contactInfo);
        }
    }
}