namespace Dexcom.Fetch.Models
{
    public class NightScoutResult
    {
        public string _id { get; set; }
        public double sgv { get; set; }
        public long date { get; set; }
        public string dateString { get; set; }
        public string direction { get; set; }
        public string device { get; set; }
        public string type { get; set; }
        public long utcOffset { get; set; }
        public string sysTime { get; set; }
    }
}
