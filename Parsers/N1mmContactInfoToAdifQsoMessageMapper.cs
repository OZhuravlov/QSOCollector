using QSOCollector.Models;
using System.Globalization;
using System.Text;

namespace QSOCollector.Parsers
{
    public static class N1mmContactInfoToAdifQsoMessageMapper
    {
        private static readonly string adifEndOfRecord = "<EOR>";

        // Parses massage from N1MM-format contactinfo and returns QsoMessage containing QsoData in ADIF format
        public static string Map(N1mmContactInfo contactInfo, Dictionary<string, string>? extraFields = null)
        {
            StringBuilder adif = new();
            AddToAdif(adif, "STATION_CALLSIGN", contactInfo.MyCall);
            AddToAdif(adif, "OPERATOR", contactInfo.Operator);
            AddToAdif(adif, "CALL", contactInfo.Call);
            AddToAdif(adif, "QSO_DATE", contactInfo.Timestamp.ToString("yyyyMMdd"));
            AddToAdif(adif, "TIME_ON", contactInfo.Timestamp.ToString("HHmmss"));
            AddToAdif(adif, "TIME_OFF", contactInfo.Timestamp.ToString("HHmmss"));
            string band = contactInfo.Band.Trim().Replace(",", ".");
            AddToAdif(adif, "BAND", MapToAdifBand(band));
            if (contactInfo.TxFreq != null && contactInfo.TxFreq > 0)
            {
                AddToAdif(adif, "FREQ", GetFormattedFreq(contactInfo.TxFreq.Value));
            }
            if (contactInfo.RxFreq != null && contactInfo.RxFreq > 0)
            {
                AddToAdif(adif, "FREQ_RX", GetFormattedFreq(contactInfo.RxFreq.Value));
            }

            AddToAdif(adif, "MODE", contactInfo.Mode);
            AddToAdif(adif, "RST_RCVD", contactInfo.Rcv);
            AddToAdif(adif, "SRX_STRING", $"{contactInfo.RcvNr} {contactInfo.Exchangel}");
            AddToAdif(adif, "RST_SENT", contactInfo.Snt);
            AddToAdif(adif, "STX_STRING", $"{contactInfo.SntNr} {contactInfo.SentExchange}");
            AddToAdif(adif, "CONTEST_ID", contactInfo.ContestName);
            AddToAdif(adif, "COMMENT", contactInfo.Comment);
            AddToAdif(adif, "DXCC_PREF", contactInfo.CountryPrefix);
            AddToAdif(adif, "PREF", contactInfo.WpxPrefix);
            AddToAdif(adif, "CONT", contactInfo.Continent);
            AddToAdif(adif, "QSLMSG", contactInfo.MiscText);
            AddToAdif(adif, "NAME", contactInfo.Name);
            AddToAdif(adif, "QTH", contactInfo.Qth);
            AddToAdif(adif, "TX_PWR", contactInfo.Power);
            AddToAdif(adif, "GRIDSQUARE", contactInfo.Gridsquare);
            if (extraFields != null)
            {
                foreach (var kvp in extraFields)
                {
                    AddToAdif(adif, kvp.Key, kvp.Value);
                }
            }
            adif.Append(adifEndOfRecord);
            return adif.ToString();
        }

        private static string GetFormattedFreq(int origFreq)
        {
            double freqInMhz = FreqConverter.toMhz(origFreq).Value;
            return freqInMhz.ToString(CultureInfo.InvariantCulture);
        }

        private static string MapToAdifBand(string origBand)
        {
            return origBand switch
            {
                "0.136" => "2190M",
                "1.8" => "160M",
                "3.5" => "80M",
                "5" => "60M",
                "5.3" => "60M",
                "60" => "60M",
                "7" => "40M",
                "10" => "30M",
                "10.1" => "30M",
                "14" => "20M",
                "18" => "17M",
                "21" => "15M",
                "24" => "12M",
                "24.9" => "12M",
                "27" => "11M",
                "28" => "10M",
                "50" => "6M",
                "70" => "4M",
                "144" => "2M",
                "432" => "70CM",
                "1.2GHz" => "23CM",
                "2400" => "13CM",
                "2.4GHz" => "13CM",
                "5.6GHz" => "6CM",
                "10GHz" => "3CM",
                "24GHz" => "1.2CM",
                "47GHz" => "6MM",
                _ => throw new NotImplementedException()
            };
        }

        private static void AddToAdif(StringBuilder adif, string tagName, string? value)
        {
            if (value == null) return;
            value = value.Trim();
            if (value == string.Empty) return;

            adif.Append($"<{tagName}:{value.Length}>{value}");
        }

    }
}