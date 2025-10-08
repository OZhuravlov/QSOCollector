using System.Text.RegularExpressions;

namespace QSOCollector
{
    public static class AdifToTableFieldsMapper
    {
        public static readonly string endOfRecord = "<EOR>";
        private static readonly string endOfHeader = "<EOH>";

        // Parses an ADIF message and returns a list of key-value maps for each QSO record
        public static List<Dictionary<string, string?>> Map(QsoMessage qsoMessage, string? sourceIpAddress = null)
        {
            if (qsoMessage.OriginalFormat == "ADIF")
            {
                qsoMessage.AdifQsoData = qsoMessage.OriginalQsoData;
            }

            var result = new List<Dictionary<string, string>>();
            var headerMap = new Dictionary<string, string>
            {
                ["ORIG_FORMAT"] = qsoMessage.OriginalFormat,
            };

            if (sourceIpAddress != null)
            {
                headerMap["SOURCE_IP_ADDRESS"] = sourceIpAddress;
            }

            string sourceKey = "PROGRAMID";
            if (!string.IsNullOrEmpty(qsoMessage.Source))
            {
                headerMap[sourceKey] = qsoMessage.Source;
            }

            string adifMessage = qsoMessage.AdifQsoData.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();

            // Find header section (if any)
            int headerEnd = adifMessage.IndexOf(endOfHeader, StringComparison.OrdinalIgnoreCase);
            string headerSection = headerEnd >= 0 ? adifMessage[..(headerEnd + 5)] : string.Empty;
            string qsoSection = headerEnd >= 0 ? adifMessage[(headerEnd + 5)..] : adifMessage;

            // Parse header tags (ignore marker-tags)
            if (!string.IsNullOrEmpty(headerSection))
            {
                foreach (var kv in ParseTags(headerSection))
                {
                    headerMap[kv.Key] = kv.Value;
                }
            }

            // Split QSO records by <EOR>
            var qsoRecords = Regex.Split(qsoSection, @endOfRecord, RegexOptions.IgnoreCase);
            foreach (var record in qsoRecords)
            {
                if (string.IsNullOrWhiteSpace(record)) continue;
                var qsoMap = new Dictionary<string, string>(headerMap, StringComparer.OrdinalIgnoreCase);
                foreach (var kv in ParseTags(record))
                {
                    qsoMap[kv.Key] = kv.Value;
                }
                string adifRecord = record + endOfRecord;
                qsoMap["ORIG_QSODATA"] = qsoMessage.OriginalFormat == "ADIF" ? adifRecord : qsoMessage.OriginalQsoData;
                qsoMap["ADIF_QSODATA"] = adifRecord;
                qsoMap["QSO_TIME"] = GetQsoTime(qsoMap).ToString("yyyy-MM-dd HH:mm:ss");
                string? qsoOperator;
                if (!qsoMap.ContainsKey("STATION_CALLSIGN") && qsoMap.TryGetValue("OPERATOR", out qsoOperator)) {
                    qsoMap["STATION_CALLSIGN"] = qsoOperator;
                }

                result.Add(qsoMap);
            }

            return result;
        }

        // Helper: Parses only tags with a value length from a section
        private static IEnumerable<KeyValuePair<string, string>> ParseTags(string section)
        {
            var tagRegex = new Regex(@"<([A-Za-z0-9_]+):(\d+)>([^<]*)", RegexOptions.Compiled);
            int pos = 0;
            while (pos < section.Length)
            {
                var match = tagRegex.Match(section, pos);
                if (!match.Success) break;

                string tag = match.Groups[1].Value.ToUpperInvariant();
                string lenStr = match.Groups[2].Value;
                string value = match.Groups[3].Value;

                if (int.TryParse(lenStr, out int len))
                {
                    value = value.Length >= len ? value[..len] : value;
                    yield return new KeyValuePair<string, string>(tag, value);
                }

                pos = match.Index + match.Length;
            }
        }

        private static DateTime GetQsoTime(Dictionary<string, string?> qsoRecord)
        {
            string? adifQsoDate = null;
            string? adifQsoTime = null;
            if (qsoRecord.ContainsKey("QSO_DATE_OFF") && qsoRecord.ContainsKey("TIME_OFF"))
            {
                adifQsoDate = qsoRecord["QSO_DATE_OFF"]!;
                adifQsoTime = qsoRecord["TIME_OFF"]!;
            }

            if (string.IsNullOrEmpty(adifQsoDate) || string.IsNullOrEmpty(adifQsoTime))
            {
                adifQsoDate = qsoRecord["QSO_DATE"]!;
                adifQsoTime = qsoRecord["TIME_ON"]!;
            }

            if (string.IsNullOrEmpty(adifQsoDate) || string.IsNullOrEmpty(adifQsoTime))
            {
                throw new ArgumentException("QSO_DATE or TIME is missing or empty in the QSO record.");
            }

            string dateTimeString = $"{adifQsoDate} {adifQsoTime}";
            if (DateTime.TryParseExact(dateTimeString, "yyyyMMdd HHmmss"[..dateTimeString.Length], null, System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime qsoTime))
            {
                return qsoTime;
            }
            throw new ArgumentException("Cannot convert ADIF QSO Date and Time to DATETIME value"); ;
        }
    }
}