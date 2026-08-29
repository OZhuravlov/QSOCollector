using QSOCollector.Models;
using System.Xml.Serialization;

namespace QSOCollector.Parsers
{
    public static class N1mmRadioInfoSerializer
    {
        public static N1mmRadioInfo Deserialize(string qsoData)
        {
            var serializer = new XmlSerializer(typeof(N1mmRadioInfo));
            using var reader = new StringReader(qsoData);
            return (N1mmRadioInfo)serializer.Deserialize(reader)!;
        }
    }
}