using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace GlucoseTray
{
    public class GlucoseFetch
    {
        private readonly string _request = Constants.NightscoutUrl + "/api/v1/entries/sgv?count=1";

        public GlucoseFetchResult GetLatestReading()
        {
            try
            {
                var client = new HttpClient();
                var response = client.GetAsync(_request).Result;
                var content = response.Content.ReadAsStringAsync().Result.Split('\t');

                Regex rgx = new Regex("[^a-zA-Z]");
                var time = ParseDate(content[0]);

                var trendString = rgx.Replace(content[3], "");
                var val = int.Parse(content[2]);

                client.Dispose();
                response.Dispose();

                return new GlucoseFetchResult()
                {
                    Value = val,
                    Trend = trendString,
                    Time = time
                };
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText(@"c:\TEMP\TrayError.txt", DateTime.Now.ToString() + e.Message + e.Message + e.InnerException + e.StackTrace + Environment.NewLine + Environment.NewLine);

                return new GlucoseFetchResult()
                {
                    Value = 0,
                    Trend = "Flat",
                    Time = DateTime.Now
                };
            }
        }

        private DateTime ParseDate(string date)
        {
            Regex rgx = new Regex("[^0-9]");

            var ymd = date.Split('T')[0].Split('-');
            var hms = date.Split('T')[1].Split('.')[0].Split(':');

            var year = int.Parse(rgx.Replace(ymd[0], ""));
            var month = int.Parse(rgx.Replace(ymd[1], ""));
            var day = int.Parse(rgx.Replace(ymd[2], ""));

            var hour = int.Parse(rgx.Replace(hms[0], ""));
            var minute = int.Parse(rgx.Replace(hms[1], ""));
            var second = int.Parse(rgx.Replace(hms[2], ""));

            var dateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

            return dateTime.ToLocalTime();
        }
    }

    public class GlucoseFetchResult
    {
        public int Value { get; set; }
        public string Trend { get; set; }
        public DateTime Time { get; set; }
    }
}
