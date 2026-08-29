using QSOCollector.Models;
using QSOCollector.Network.Client;
using Serilog;

namespace QSOCollector.Parsers
{
    public static class SatRuleApplier
    {
        private static readonly ILogger log = Log.ForContext<UdpClientListener>();

        public static bool AdifApplySatRule(QsoMessage qsoMessage, SatRule rule, List<Band> bands, out bool isOrigChanged)
        {
            List<Dictionary<string, string>> qsos = AdifToTableFieldsMapper.Map(qsoMessage);
            bool isApplied = ApplyRuleToQsos(qsos, rule, bands, out _, out bool updatedIsOrigChanged);
            isOrigChanged = updatedIsOrigChanged;
            return isApplied;
        }

        public static bool ApplyRuleToQsos(List<Dictionary<string, string>> qsos, SatRule rule, List<Band> bands, out List<Dictionary<string, string>> newQsos, out bool isOrigChanged)
        {
            bool isApplied = false;
            newQsos = new List<Dictionary<string, string>>();
            isOrigChanged = false;

            foreach (var qso in qsos)
            {
                // Process each QSO
                qso.TryGetValue("BAND", out string? bandName);
                double? freqTx = null;
                if (qso.TryGetValue("FREQ", out string? freqTxStr))
                {
                    freqTx = double.Parse(freqTxStr, System.Globalization.CultureInfo.InvariantCulture);
                }

                if (freqTx == null && bandName == null)
                {
                    log.Error("Invalid ADIF qso log: Freq or Band must be specified");
                    isOrigChanged = true;
                    continue;
                }

                freqTx ??= bands.FirstOrDefault(b => b.BandName.Equals(bandName, StringComparison.OrdinalIgnoreCase))?.FreqFrom;
                if (freqTx < rule.SourceFreqFrom || freqTx > rule.SourceFreqTo)
                {
                    log.Debug($"Rule {rule.Name} not applied since QSO (ADIF) freq {freqTx} mHz is out of rule range ({rule.SourceFreqFrom}-{rule.SourceFreqTo} mHz)");
                    newQsos.Add(qso);
                    continue;
                }

                isApplied = true;

                bandName ??= bands.FirstOrDefault(b => freqTx >= b.FreqFrom && freqTx <= b.FreqTo)?.BandName;

                qso.TryGetValue("BAND_RX", out string? bandRx);
                double? freqRx = null;
                if (qso.TryGetValue("FREQ_RX", out string? freqRxStr))
                {
                    freqRx = double.Parse(freqRxStr, System.Globalization.CultureInfo.InvariantCulture);
                }

                if (rule.FreqTx != null)
                {
                    freqTx = rule.FreqTx;
                    isOrigChanged = true;
                }

                if (rule.BandTx != null)
                {
                    string newBandName = rule.BandTx.BandName;
                    if (!newBandName.Equals(bandName, StringComparison.OrdinalIgnoreCase))
                    {
                        bandName = newBandName;
                        isOrigChanged = true;
                    }

                    AdjustFreq(freqTx, rule.BandTx, out double? newFreqTx);
                    if (newFreqTx != freqTx)
                    {
                        freqTx = newFreqTx;
                        isOrigChanged = true;
                    }
                }

                if (rule.BandRx != null)
                {
                    string newBandRxName = rule.BandRx.BandName;
                    if (!newBandRxName.Equals(bandRx, StringComparison.OrdinalIgnoreCase))
                    {
                        bandRx = newBandRxName;
                        isOrigChanged = true;
                    }

                    AdjustFreq(freqRx, rule.BandRx, out double? newFreqRx);
                    if (newFreqRx != freqRx)
                    {
                        freqRx = newFreqRx;
                        isOrigChanged = true;
                    }
                }

                if (isOrigChanged)
                {
                    if (freqTx.HasValue)
                    {
                        qso["FREQ"] = freqTx.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }

                    if (freqRx.HasValue)
                    {
                        qso["FREQ_RX"] = freqRx.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    }

                    qso["BAND"] = bandName;

                    if (rule.BandRx != null)
                    {
                        qso["BAND_RX"] = rule.BandRx.BandName;
                    }
                }

                qso["PROP_MODE"] = rule.PropagationMode;
                qso["SAT_NAME"] = rule.SatName;

                if (rule.SatMode != null)
                {
                    qso["SAT_MODE"] = rule.SatMode;
                }
                newQsos.Add(qso);
            }
            return isApplied;
        }

        public static bool N1mmApplySatRule(N1mmContactInfo info, SatRule rule, out bool isOrigChanged, out Dictionary<string, string>? extraFields)
        {
            extraFields = null;
            isOrigChanged = false;
            double? freqRx = FreqConverter.toMhz(info.RxFreq);
            double? freqTx = FreqConverter.toMhz(info.TxFreq);

            if (freqTx < rule.SourceFreqFrom || freqTx > rule.SourceFreqTo)
            {
                log.Debug($"Rule {rule.Name} not applied since QSO (N1MM) freq {info.TxFreq} Hz is out of rule range ({rule.SourceFreqFrom}-{rule.SourceFreqTo} mHz)");
                return false;
            }

            string bandName = info.Band;

            if (rule.FreqTx != null)
            {
                freqTx = rule.FreqTx;
                isOrigChanged = true;
            }

            if (rule.BandTx != null)
            {
                string newBandName = rule.BandTx.N1mmBandName;
                if (!newBandName.Equals(bandName, StringComparison.OrdinalIgnoreCase))
                {
                    bandName = newBandName;
                    isOrigChanged = true;
                }

                AdjustFreq(freqTx, rule.BandTx, out double? newFreqTx);
                if (newFreqTx != freqTx)
                {
                    freqTx = newFreqTx;
                    isOrigChanged = true;
                }
            }

            if (rule.FreqRx != null)
            {
                freqRx = rule.FreqRx;
                isOrigChanged = true;
            }

            if (rule.BandRx != null)
            {
                AdjustFreq(freqRx, rule.BandRx, out double? newFreqRx);
                if (newFreqRx != freqRx)
                {
                    freqRx = newFreqRx;
                    isOrigChanged = true;
                }
            }

            if (isOrigChanged)
            {
                info.TxFreq = FreqConverter.fromMhz(freqTx);
                info.RxFreq = FreqConverter.fromMhz(freqRx);
                info.Band = bandName;
            }

            extraFields = new() {
                { "PROP_MODE", rule.PropagationMode },
                { "SAT_NAME", rule.SatName }
            };

            if (rule.SatMode != null)
            {
                extraFields["SAT_MODE"] = rule.SatMode;
            }

            if (rule.BandRx != null)
            {
                extraFields["BAND_RX"] = rule.BandRx.BandName;
            }
            return true;
        }

        private static void AdjustFreq(double? freq, Band band, out double? newFreq)
        {
            newFreq = freq;
            if (freq.HasValue)
            {
                if (freq < band.FreqFrom || freq > band.FreqTo)
                {
                    newFreq = FreqConverter.fromMhz(band.FreqFrom);
                }
            }
            else
            {
                newFreq = band.FreqFrom;
            }
        }
    }
}