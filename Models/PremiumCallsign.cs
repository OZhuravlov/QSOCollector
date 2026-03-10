using FileHelpers;

namespace QSOCollector.Models
{
    [DelimitedRecord(",")]
    public class PremiumCallsign
    {
        public string Callsign { get; set; }
        public decimal Donated_amount_usd { get; set; }
        public string? Club { get; set; }
    }
}
