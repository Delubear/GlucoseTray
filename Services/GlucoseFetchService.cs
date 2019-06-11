using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using GlucoseTray.Models;

namespace GlucoseTray.Services
{
    public class GlucoseFetchService
    {
        private readonly string _request = Constants.NightscoutUrl + "/api/v1/entries/sgv?count=1";

        public GlucoseFetchResult GetLatestReading()
        {
            var client = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                response = client.GetAsync(_request).Result;
                var content = response.Content.ReadAsStringAsync().Result.Split('\t');

                Regex rgx = new Regex("[^a-zA-Z]");
                var time = ParseDate(content[0]);

                var trendString = rgx.Replace(content[3], "");
                var val = int.Parse(content[2]);

                return new GlucoseFetchResult()
                {
                    Value = val,
                    Trend = trendString,
                    Time = time,
                    TrendIcon = GetTrendArrow(trendString)
                };
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText(Constants.ErrorLogPath, DateTime.Now.ToString() + e.Message + e.Message + e.InnerException + e.StackTrace + Environment.NewLine + Environment.NewLine);

                return new GlucoseFetchResult()
                {
                    Value = 0,
                    Trend = "Flat",
                    Time = DateTime.Now,
                    TrendIcon = GetTrendArrow("Flat")
                };
            }
            finally
            {
                client.Dispose();
                response.Dispose();
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

        private string GetTrendArrow(string trend)
        {
            if (string.Equals(trend, "Flat", StringComparison.OrdinalIgnoreCase))
                return "→";
            else if (string.Equals(trend, "FortyFiveDown", StringComparison.OrdinalIgnoreCase))
                return "↘";
            else if (string.Equals(trend, "FortyFiveUp", StringComparison.OrdinalIgnoreCase))
                return "↗";
            else if (string.Equals(trend, "SingleDown", StringComparison.OrdinalIgnoreCase))
                return "↓";
            else if (string.Equals(trend, "SingleUp", StringComparison.OrdinalIgnoreCase))
                return "↑";
            else if (string.Equals(trend, "DoubleDown", StringComparison.OrdinalIgnoreCase))
                return "⮇";
            else if (string.Equals(trend, "DoubleUp", StringComparison.OrdinalIgnoreCase))
                return "⮅";
            else
                return "";
        }
    }
}
