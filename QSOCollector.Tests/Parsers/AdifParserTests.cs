using QSOCollector.Models;
using QSOCollector.Parsers;
using Xunit;
using System.Collections.Generic;

namespace QSOCollector.Tests.Parsers
{
    public class AdifParserTests
    {
        [Fact]
        public void Parse_ValidAdifMessage_ReturnsCorrectRecords()
        {
            string adif = "<PROGRAMID:4>MSHV<EOH><OPERATOR:5>OH7WA<CALL:4>8R7X<QSO_DATE:8>20240221<TIME_ON:4>1059<FREQ:6>21.091<MODE:3>FT8<RST_SENT:2>04<RST_RCVD:3>-10<GRIDSQUARE:6>GJ16BN<PFX:2>8R<DXCC_PREF:2>8R<CQZ:2>09<ITUZ:2>12<BAND:3>15M<CONT:2>SA<LOTW_QSL_RCVD:1>Y<LOTW_QSLRDATE:8>20240221<QSLMSG:19>TNX For QSO TU 73!.<LOTW_QSL_SENT:1>Y<DXCC:3>129<EOR>";
            QsoMessage qsoMessage = new() {
                Source = "MSHV", 
                OriginalFormat = "ADIF",
                OriginalQsoData = adif,
                AdifQsoData = adif
            };

            var records = AdifToTableFieldsMapper.Map(qsoMessage);

            Xunit.Assert.Single(records);
            var record = records[0];
            Xunit.Assert.Equal("MSHV", record["PROGRAMID"]);
            Xunit.Assert.Equal("8R7X", record["CALL"]);
            Xunit.Assert.Equal("FT8", record["MODE"]);
        }

/*        [Fact]
        public void Parse_EmptyMessage_ReturnsEmptyList()
        {
            QsoMessage qsoMessage = new()
            {
                Source = "MSHV",
                OriginalFormat = "ADIF",
                OriginalQsoData = "",
                AdifQsoData = ""
            };
            var records = AdifToTableFieldsMapper.Map(qsoMessage);
            Assert.Empty(records);
        }*/
    }
}