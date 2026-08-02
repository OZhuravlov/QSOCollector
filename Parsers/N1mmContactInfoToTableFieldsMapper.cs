using QSOCollector.Models;

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
                if (contactInfoId != null)
                {
                    qsoMessage.ExternalId = contactInfoId;
                }
            }

            return AdifToTableFieldsMapper.Map(qsoMessage, externalId: contactInfoId, sourceIpAddress: sourceIpAddress);
        }

        private static string DeserializeN1mmContactInfoAndMapToAdif(QsoMessage qsoMessage, out string? id)
        {
            string rootName = qsoMessage.Replace ? "contactreplace" : "contactinfo";
            N1mmContactInfo contactInfo = N1mmContactInfoSerializer.Deserialize(qsoMessage.OriginalQsoData, rootName);
            id = contactInfo.Id;
            return N1mmContactInfoToAdifQsoMessageMapper.Map(contactInfo);
        }
    }
}