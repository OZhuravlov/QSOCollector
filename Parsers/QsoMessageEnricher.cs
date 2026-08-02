using QSOCollector.Models;
using QSOCollector.Network.Client;
using Serilog;

namespace QSOCollector.Parsers
{
    public static class QsoMessageEnricher
    {
        private static readonly ILogger log = Log.ForContext<UdpClientListener>();

        public static void EnrichMessage(QsoMessage qsoMessage, List<SatRule> satRules, List<Band> bands, out string? newQsoData)
        {
            newQsoData = null;
            if (!IsExpectedMessageFormat(qsoMessage))
            {
                return;
            }

            switch (qsoMessage.OriginalFormat)
            {
                case "ADIF":
                    AdifEnrichMessage(qsoMessage, satRules, bands, out newQsoData);
                    break;
                case "N1MM":
                    N1mmEnrichMessage(qsoMessage, satRules, out newQsoData);
                    break;
                default:
                    throw new NotImplementedException($"Message format {qsoMessage.OriginalFormat} is not supported for enrichment");
            }
        }

        public static bool IsExpectedMessageFormat(QsoMessage qsoMessage)
        {
            string[] requiredTexts = qsoMessage.OriginalFormat switch
            {
                "ADIF" => ["<EOR>", "<QSO_DATE:"],
                "N1MM" => qsoMessage.Replace ? ["<contactreplace", "</contactreplace>"] : ["<contactinfo", "</contactinfo>"],
                _ => []
            };
            foreach (string text in requiredTexts)
            {
                if (!qsoMessage.OriginalQsoData.Contains(text, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }

        private static void N1mmEnrichMessage(QsoMessage qsoMessage, List<SatRule> satRules, out string? newQsoData)
        {
            string rootName = qsoMessage.Replace ? "contactreplace" : "contactinfo";
            N1mmContactInfo info = N1mmContactInfoSerializer.Deserialize(qsoMessage.OriginalQsoData, rootName);
            qsoMessage.AdifQsoData = N1mmContactInfoToAdifQsoMessageMapper.Map(info);
            qsoMessage.ProgramId = info.App;
            qsoMessage.ExternalId = info.Id;
            newQsoData = null;
            
            if (satRules.Count == 0)
            {
                return;
            }

            bool isOrigChanged = false;
            foreach (var rule in satRules)
            {
                bool isApplied = SatRuleApplier.N1mmApplySatRule(info, rule, out isOrigChanged, out Dictionary<string, string>? extraFields);
                if (isApplied)
                {
                    log.Debug("Rule {RuleName} applied to QSO message from {Format}", rule.Name, qsoMessage.OriginalFormat);
                    qsoMessage.AdifQsoData = N1mmContactInfoToAdifQsoMessageMapper.Map(info, extraFields);
                    break;
                }
            }

            if (isOrigChanged)
            {
                qsoMessage.OriginalQsoData = N1mmContactInfoSerializer.Serialize(info, rootName);
                newQsoData = qsoMessage.OriginalQsoData;
            }
        }

        private static void AdifEnrichMessage(QsoMessage qsoMessage, List<SatRule> satRules, List<Band> bands, out string? newQsoData)
        {
            AdifToTableFieldsMapper.GetHeader(qsoMessage.OriginalQsoData).TryGetValue("PROGRAMID", out string? programId);
            qsoMessage.ProgramId = programId;
            qsoMessage.AdifQsoData = AdifToTableFieldsMapper.ExtractAdifBody(qsoMessage.OriginalQsoData);
            newQsoData = null;
            if (satRules.Count == 0)
            {
                return;
            }

            bool isOrigChanged = false;
            foreach (var rule in satRules)
            {
                bool isApplied = SatRuleApplier.AdifApplySatRule(qsoMessage, rule, bands, out isOrigChanged);
                if (isApplied)
                {
                    log.Debug("Rule {RuleName} applied to QSO message from {Format}", rule.Name, qsoMessage.OriginalFormat);
                    break;
                }
            }
            if (isOrigChanged)
            {
                newQsoData = qsoMessage.AdifQsoData;
            }
        }
    }
}