namespace QSOCollector.Parsers
{
    public static class FreqConverter
    {
        private const double freqMhzMult = 100000;

        public static double? toMhz(int? freqInHz)
        {
            if (freqInHz == null)
            {
                return null;
            }
            return freqInHz / freqMhzMult;
        }

        public static int? fromMhz(double? freqInMhz)
        {
            if (freqInMhz == null) {
                return null;
            }
            return (int)(freqInMhz * freqMhzMult);
        }
    }
}