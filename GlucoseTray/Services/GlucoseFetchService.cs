using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using GlucoseTray.Enums;
using GlucoseTray.Extensions;
using GlucoseTray.Interfaces;
using GlucoseTray.Models;

namespace GlucoseTray.Services
{
    public class GlucoseFetchService
    {
        private readonly string baseDexcomUrl = "https://share1.dexcom.com";
        private readonly string nightscoutRequestUrl = $"{Constants.NightscoutUrl}/api/v1/entries/sgv?count=1";

        public GlucoseFetchResult GetLatestReading(ILogService logger)
        {
            var fetchResult = new GlucoseFetchResult();

            try
            {
                if(Constants.FetchMethod == FetchMethod.DexcomShare)
                    GetFetchResultFromDexcom(fetchResult);
                else if(Constants.FetchMethod == FetchMethod.NightscoutApi)
                    GetFetchResultFromNightscout(fetchResult);
                else
                    throw new InvalidOperationException("Fetch Method either not specified or invalid specification.");
            }
            catch (Exception e)
            {
                logger.Log(e);

                fetchResult.Value = 0;
                fetchResult.Time = DateTime.Now;
                fetchResult.TrendIcon = "4".GetTrendArrowFromDexcom();
            }

            return fetchResult;
        }

        private GlucoseFetchResult GetFetchResultFromNightscout(GlucoseFetchResult fetchResult)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{nightscoutRequestUrl}" + (!string.IsNullOrWhiteSpace(Constants.AccessToken) ? $"&token={Constants.AccessToken}" : "")),
                Method = HttpMethod.Get,
            };

            var response = new HttpClient().SendAsync(request).Result;
            var content = response.Content.ReadAsStringAsync().Result.Split('\t');
            Regex rgx = new Regex("[^a-zA-Z]");
            var time = content[0].ParseNightscoutDate();
            var trendString = rgx.Replace(content[3], "");
            var val = int.Parse(content[2]);

            fetchResult.Value = val;
            fetchResult.Time = time;
            fetchResult.TrendIcon = trendString.GetTrendArrowFromNightscout();
            return fetchResult;
        }

        private GlucoseFetchResult GetFetchResultFromDexcom(GlucoseFetchResult fetchResult)
        {
            // Get Session Id
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseDexcomUrl}/ShareWebServices/Services/General/LoginPublisherAccountByName"),
                Method = HttpMethod.Post,
                Content = new StringContent("{\"accountName\":\"" + Constants.DexcomUsername + "\"," +
                                                 "\"applicationId\":\"d8665ade-9673-4e27-9ff6-92db4ce13d13\"," +
                                                 "\"password\":\"" + Constants.DexcomPassword + "\"}", Encoding.UTF8, "application/json")
            };

            var client = new HttpClient();
            var response = client.SendAsync(request).Result;
            var sessionId = response.Content.ReadAsStringAsync().Result.Replace("\"", "");

            request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseDexcomUrl}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1"),
                Method = HttpMethod.Post
            };
            var result = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result.RemoveUnnecessaryCharacters().Split(',');

            // Get raw data seperated
            var unixTime = result[1].Split('e')[1];
            var trend = result[2].Split(':')[1];
            var stringVal = result[3].Split(':')[1];

            // Convert raw to correct data
            DateTime localTime = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(unixTime))
                localTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(unixTime)).LocalDateTime;
            int.TryParse(stringVal, out int val);
            var trendIcon = trend.GetTrendArrowFromDexcom();

            fetchResult.Value = val;
            fetchResult.Time = localTime;
            fetchResult.TrendIcon = trendIcon;
            return fetchResult;
        }
    }
}
