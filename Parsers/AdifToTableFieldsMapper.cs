using QSOCollector.Models;
using System.Text.RegularExpressions;
using System.Text;

namespace QSOCollector.Parsers
{
    public static class AdifToTableFieldsMapper
    {
        public static readonly string endOfRecord = "<EOR>";
        private static readonly string endOfHeader = "<EOH>";
        private static readonly List<string> nonRecordTags = ["PROGRAMID", "ORIG_FORMAT", "SOURCE_IP_ADDRESS", "EXTERNAL_ID", "SOURCE_NAME", "ORIG_QSODATA", "ADIF_QSODATA", "IS_REPLACE", "QSO_TIME", "MODE_GROUP"];

        // Parses an ADIF message and returns a list of key-value maps for each QSO record
        public static List<Dictionary<string, string>> Map(
            QsoMessage qsoMessage,
            string? externalId = null,
            string? sourceIpAddress = null,
            Func<string, Task>? progressUpdater = null
            )
        {
            progressUpdater?.Invoke("Parsing ADIF...prepare");
            var result = new List<Dictionary<string, string>>();

            var headerMap = GetHeader(qsoMessage.AdifQsoData);
            headerMap["ORIG_FORMAT"] = qsoMessage.OriginalFormat;
            if (qsoMessage.ProgramId != null)
            {
                headerMap["PROGRAMID"] = qsoMessage.ProgramId;
            }

            if (sourceIpAddress != null)
            {
                headerMap["SOURCE_IP_ADDRESS"] = sourceIpAddress;
            }

            if (externalId != null)
            {
                headerMap["EXTERNAL_ID"] = externalId;
            }

            string sourceKey = "SOURCE_NAME";
            if (!string.IsNullOrEmpty(qsoMessage.Source))
            {
                headerMap[sourceKey] = qsoMessage.Source;
            }

            string qsoSection = ExtractAdifBody(qsoMessage.AdifQsoData);

            // Split QSO records by <EOR>
            var qsoRecords = Regex.Split(qsoSection, @endOfRecord, RegexOptions.IgnoreCase);
            int n = 0;
            foreach (var record in qsoRecords)
            {
                n++;
                if (string.IsNullOrWhiteSpace(record)) continue;
                var qsoMap = new Dictionary<string, string>(headerMap, StringComparer.OrdinalIgnoreCase);
                foreach (var kv in ParseTags(record))
                {
                    qsoMap[kv.Key] = kv.Value;
                }
                string adifRecord = record + endOfRecord;
                qsoMap["IS_REPLACE"] = qsoMessage.Replace.ToString();
                qsoMap["ORIG_QSODATA"] = qsoMessage.OriginalQsoData;
                qsoMap["ADIF_QSODATA"] = adifRecord;
                qsoMap["QSO_TIME"] = GetQsoTime(qsoMap).ToString("yyyy-MM-dd HH:mm:ss");
                if (!qsoMap.ContainsKey("STATION_CALLSIGN") && qsoMap.TryGetValue("OPERATOR", out string? qsoOperator))
                {
                    qsoMap["STATION_CALLSIGN"] = qsoOperator;
                }
                qsoMap["MODE_GROUP"] = GetModeGroup(qsoMap["MODE"]);

                result.Add(qsoMap);
                if (n % 10 == 0) progressUpdater?.Invoke($"Parsing {n} of {qsoRecords.Length}");
            }
            progressUpdater?.Invoke($"Parsed{result.Count} of {qsoRecords.Length}");
            return result;
        }

        public static Dictionary<string, string> GetHeader(string adifQsoData)
        {
            var headerMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string adifMessage = adifQsoData.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();

            // Find header section (if any)
            int headerEnd = adifMessage.IndexOf(endOfHeader, StringComparison.OrdinalIgnoreCase);
            string? headerSection = headerEnd >= 0 ? adifMessage[..(headerEnd + 5)] : string.Empty;
            // Parse header tags (ignore marker-tags)
            if (!string.IsNullOrEmpty(headerSection))
            {
                foreach (var kv in ParseTags(headerSection))
                {
                    headerMap[kv.Key] = kv.Value;
                }
            }
            return headerMap;
        }

        public static string ExtractAdifBody(string adifQsoData)
        {
            string adifMessage = adifQsoData.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();

            // Find header section (if any)
            int headerEnd = adifMessage.IndexOf(endOfHeader, StringComparison.OrdinalIgnoreCase);
            string qsoSection = headerEnd >= 0 ? adifMessage[(headerEnd + 5)..] : adifMessage;
            qsoSection = qsoSection[..qsoSection.LastIndexOf(@endOfRecord, StringComparison.OrdinalIgnoreCase)];
            return qsoSection;
        }

        public static string Map(List<Dictionary<string, string?>> qsoRecords, bool withHeader = false)
        {
            if (qsoRecords == null || qsoRecords.Count == 0) {
                throw new ArgumentNullException("Qso record must not be null or empty");
            }

            StringBuilder adif = new();
            if (withHeader && qsoRecords[0].TryGetValue("PROGRAMID", out string? programId))
            {
                adif.Append($"<PROGRAMID:{programId.Length}>{programId}");
            }
            adif.Append(endOfHeader);
            qsoRecords.ForEach(qsoRecord =>
            {
                foreach (var (key, value) in qsoRecord)
                {
                    if (nonRecordTags.Contains(key))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }
                    adif.Append($"<{key}:{value.Length}>{value}");
                }
                adif.AppendLine(endOfRecord);
            });
            return adif.ToString();
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
                string value = match.Groups[3].Value.ToUpperInvariant();

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

        private static string GetModeGroup(string mode)
        {
            return mode switch
            {
                "CW" => "CW",
                "SSB" or "USB" or "LSB" or "AM" or "FM" => "PHONE",
                "FT8" or "FT4" or "RTTY" or "PSK31" or "PSK63" or "JT65" or "JT9" or "DIGU" => "DATA",
                "SAT" => "SAT",
                _ => "OTHER"
            };
        }
    }
}