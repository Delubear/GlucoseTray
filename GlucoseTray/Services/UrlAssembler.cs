using GlucoseTray.Enums;
using Microsoft.Extensions.Options;

namespace GlucoseTray.Services
{
    public class UrlAssembler
    {
        readonly IOptionsMonitor<GlucoseTraySettings> _options;

        public UrlAssembler(IOptionsMonitor<GlucoseTraySettings> options)
        {
            _options = options;
        }

        public string BuildNightscoutUrl()
        {
            string url = $"{_options.CurrentValue.NightscoutUrl?.TrimEnd('/')}/api/v1/entries/sgv?count=1";
            url += !string.IsNullOrWhiteSpace(_options.CurrentValue.AccessToken) ? $"&token={_options.CurrentValue.AccessToken}" : string.Empty;
            return url;
        }

        public string GetDexComServer() => _options.CurrentValue.DexcomServer switch
        {
            DexcomServerLocation.DexcomShare1 => "share1.dexcom.com",
            DexcomServerLocation.DexcomShare2 => "share2.dexcom.com",
            DexcomServerLocation.DexcomInternational => "shareous1.dexcom.com",
            _ => "share1.dexcom.com",
        };

        public string BuildDexComAccountIdUrl()
        {
            string host = GetDexComServer();
            string url = $"https://{host}/ShareWebServices/Services/General/AuthenticatePublisherAccount";
            return url;
        }

        public string BuildDexComSessionUrl()
        {
            string host = GetDexComServer();
            string url = $"https://{host}/ShareWebServices/Services/General/LoginPublisherAccountById";
            return url;
        }

        public string BuildDexComGlucoseValueUrl(string sessionId)
        {
            string host = GetDexComServer();
            string url = $"https://{host}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1";
            return url;
        }
    }
}
