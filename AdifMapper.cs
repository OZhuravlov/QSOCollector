using System.Text.RegularExpressions;

namespace QSOCollector
{
    public static class AdifMapper
    {
        private static readonly string endOfHeader = "<EOH>";
        private static readonly string endOfRecord = "<EOR>";

        // Parses an ADIF message and returns a list of key-value maps for each QSO record
        public static List<Dictionary<string, string>> Map(QsoMessage qsoMessage, string sourceIpAddress)
        {
            var result = new List<Dictionary<string, string>>();
            var headerMap = new Dictionary<string, string>
            {
                ["PROGRAMID"] = qsoMessage.Source,
                ["ORIG_FORMAT"] = qsoMessage.OriginalFormat,
                ["SOURCE_IP_ADDRESS"] = sourceIpAddress
            };
            string adifMessage = qsoMessage.QsoData.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();

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
            string sourceKey = "PROGRAMID";
            if (!headerMap.ContainsKey(sourceKey) && !string.IsNullOrEmpty(qsoMessage.Source)) {
                headerMap[sourceKey] = qsoMessage.Source;
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
                qsoMap["ORIG_QSODATA"] = record + endOfRecord;
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
    }
}