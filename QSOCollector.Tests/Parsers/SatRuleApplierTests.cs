using QSOCollector.Models;
using QSOCollector.Parsers;
using Assert = Xunit.Assert;

namespace QSOCollector.Tests.Parsers
{
    public class SatRuleApplierTests
    {
        [Fact]
        public void ApplyRuleToQsos_AppliesMetadataAtInclusiveFrequencyBounds()
        {
            var rule = CreateRule(145.8, 146.0);
            var qsos = new List<Dictionary<string, string>>
            {
                new() { ["FREQ"] = "145.8", ["BAND"] = "2m" },
                new() { ["FREQ"] = "146.0", ["BAND"] = "2m" }
            };

            var applied = SatRuleApplier.ApplyRuleToQsos(qsos, rule, [], out var newQsos, out var originalChanged);

            Assert.True(applied);
            Assert.False(originalChanged);
            Assert.Equal(2, newQsos.Count);
            Assert.All(newQsos, qso =>
            {
                Assert.Equal("SAT", qso["PROP_MODE"]);
                Assert.Equal("AO-91", qso["SAT_NAME"]);
                Assert.Equal("FM", qso["SAT_MODE"]);
            });
        }

        [Fact]
        public void ApplyRuleToQsos_OutOfRangeFrequencyDoesNotApplyRule()
        {
            var qso = new Dictionary<string, string> { ["FREQ"] = "146.1", ["BAND"] = "2m" };

            var applied = SatRuleApplier.ApplyRuleToQsos([qso], CreateRule(145.8, 146.0), [], out var newQsos, out var originalChanged);

            Assert.False(applied);
            Assert.False(originalChanged);
            Assert.Same(qso, Assert.Single(newQsos));
            Assert.DoesNotContain("SAT_NAME", qso.Keys);
        }

        [Fact]
        public void ApplyRuleToQsos_MapsTransmitBandAndFrequency()
        {
            var qso = new Dictionary<string, string> { ["FREQ"] = "145.9" };
            var rule = CreateRule(145.8, 146.0);
            rule.BandTx = new Band
            {
                BandName = "70cm",
                AltBandName = "70cm",
                N1mmBandName = "432",
                FullBandName = "70 centimeters",
                FreqFrom = 430.0,
                FreqTo = 440.0,
                Designator = "U",
                IsActive = true
            };

            var applied = SatRuleApplier.ApplyRuleToQsos([qso], rule, [rule.BandTx], out var newQsos, out var originalChanged);

            Assert.True(applied);
            Assert.True(originalChanged);
            var result = Assert.Single(newQsos);
            Assert.Equal("70cm", result["BAND"]);
            Assert.Equal("430", result["FREQ"]);
            Assert.Equal("AO-91", result["SAT_NAME"]);
        }

        [Fact]
        public void N1mmApplySatRule_InRangeAddsSatelliteFieldsWithoutChangingContact()
        {
            var contact = new N1mmContactInfo
            {
                MyCall = "N0CALL",
                Band = "2m",
                Mode = "FM",
                Call = "K1ABC",
                TxFreq = 14590000,
                RxFreq = 14590000
            };

            var applied = SatRuleApplier.N1mmApplySatRule(contact, CreateRule(145.8, 146.0), out var originalChanged, out var extraFields);

            Assert.True(applied);
            Assert.False(originalChanged);
            Assert.Equal(14590000, contact.TxFreq);
            Assert.NotNull(extraFields);
            Assert.Equal("SAT", extraFields["PROP_MODE"]);
            Assert.Equal("AO-91", extraFields["SAT_NAME"]);
            Assert.Equal("FM", extraFields["SAT_MODE"]);
        }

        private static SatRule CreateRule(double sourceFreqFrom, double sourceFreqTo) => new()
        {
            Name = "AO-91",
            SourceFreqFrom = sourceFreqFrom,
            SourceFreqTo = sourceFreqTo,
            PropagationMode = "SAT",
            SatName = "AO-91",
            SatMode = "FM",
            IsActive = true
        };
    }
}
