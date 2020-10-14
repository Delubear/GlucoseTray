namespace Dexcom.Fetch.Models
{
    /// <summary>
    /// Class that maps to the JSON received from DexCom queries.
    /// </summary>
    internal class DexcomResult
    {
        public string ST { get; set; }
        public string DT { get; set; }
        public int Trend { get; set; }
        public double Value { get; set; }
        public string WT { get; set; }
    }
}
