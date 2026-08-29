using System.Xml;
using System.Xml.Serialization;

namespace QSOCollector.Models
{
    [XmlRoot("RadioInfo")]
    public class N1mmRadioInfo
    {
        [XmlElement("app")]
        public string? App { get; set; }

        [XmlElement("logger")]
        public string? Logger { get; set; }

        [XmlElement("StationName")]
        public string? StationName { get; set; }

        [XmlElement("Station")]
        public string? Station { get; set; }

        [XmlElement("RadioNr")]
        public string? RadioNr { get; set; }

        [XmlElement("Freq")]
        public int? Freq { get; set; }

        [XmlElement("TXFreq")]
        public int? TxFreq { get; set; }

        [XmlElement("InactiveFreq")]
        public int? InactiveFreq { get; set; }

        [XmlElement("Mode")]
        public string? Mode { get; set; }

        [XmlElement("OpCall")]
        public string? OpCall { get; set; }

        [XmlElement("mycall")]
        public string? myCall { get; set; }

        [XmlIgnore]
        public bool? IsRunning { get; set; }

        [XmlElement("IsRunning")]
        public string? IsRunningSerialize
        {
            get { return BoolToString(IsRunning); }
            set => IsRunning = ParseBool(value);
        }

        [XmlElement("FocusEntry")]
        public string? FocusEntry { get; set; }

        [XmlElement("EntryWindowHwnd")]
        public string? EntryWindowHwnd { get; set; }

        [XmlElement("Antenna")]
        public string? Antenna { get; set; }

        [XmlElement("Rotors")]
        public string? Rotors { get; set; }

        [XmlElement("FocusRadioNr")]
        public string? FocusRadioNr { get; set; }

        [XmlIgnore]
        public bool? IsStereo { get; set; }
        [XmlElement("IsStereo")]
        public string? IsStereoSerialize
        {
            get { return BoolToString(IsStereo); }
            set => IsStereo = ParseBool(value);
        }

        [XmlIgnore]
        public bool? IsSplit { get; set; }

        [XmlElement("IsSplit")]
        public string? IsSplitSerialize
        {
            get { return BoolToString(IsSplit); }
            set => IsSplit = ParseBool(value);
        }

        [XmlElement("ActiveRadioNr")]
        public string? ActiveRadioNr { get; set; }

        [XmlIgnore]
        public bool? IsTransmitting { get; set; }

        [XmlElement("IsTransmitting")]
        public string? IsTransmittingSerialize
        {
            get { return BoolToString(IsTransmitting); }
            set => IsTransmitting = ParseBool(value);
        }

        [XmlElement("Technique")]
        public string? Technique { get; set; }

        [XmlElement("StationType")]
        public string? StationType { get; set; }

        [XmlElement("IFFrequency")]
        public int? IfFrequency { get; set; }

        [XmlElement("FunctionKeyCaption")]
        public string? FunctionKeyCaption { get; set; }

        [XmlElement("RadioName")]
        public string? RadioName { get; set; }

        [XmlElement("AuxAntSelected")]
        public string? AuxAntSelected { get; set; }

        [XmlElement("AuxAntSelectedName")]
        public string? AuxAntSelectedName { get; set; }


        [XmlIgnore]
        public bool? IsConnected { get; set; }

        [XmlElement("IsConnected")]
        public string? IsConnectedSerialize
        {
            get { return BoolToString(IsConnected); }
            set => IsConnected = ParseBool(value);
        }

        private static bool? ParseBool(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return value.Equals("True", StringComparison.OrdinalIgnoreCase);
        }

        private static string? BoolToString(bool? value)
        {
            return value.HasValue && value.Value ? "True" : "False";
        }
    }
}
