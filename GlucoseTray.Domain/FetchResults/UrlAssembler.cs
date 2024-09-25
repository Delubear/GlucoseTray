using GlucoseTray.Domain.Enums;

namespace GlucoseTray.Domain.FetchResults;

public class UrlAssembler(ISettingsProxy options)
{
    private readonly ISettingsProxy _options = options;

    public string BuildNightscoutUrl()
    {
        var url = $"{_options.NightscoutUrl?.TrimEnd('/')}/api/v1/entries/sgv?count=1";
        url += !string.IsNullOrWhiteSpace(_options.AccessToken) ? $"&token={_options.AccessToken}" : string.Empty;
        return url;
    }

    public string GetDexComServer() => _options.DexcomServer switch
    {
        DexcomServerLocation.DexcomShare1 => "share1.dexcom.com",
        DexcomServerLocation.DexcomShare2 => "share2.dexcom.com",
        DexcomServerLocation.DexcomInternational => "shareous1.dexcom.com",
        _ => "share1.dexcom.com",
    };

    public string BuildDexComAccountIdUrl()
    {
        var host = GetDexComServer();
        var url = $"https://{host}/ShareWebServices/Services/General/AuthenticatePublisherAccount";
        return url;
    }

    public string BuildDexComSessionUrl()
    {
        var host = GetDexComServer();
        var url = $"https://{host}/ShareWebServices/Services/General/LoginPublisherAccountById";
        return url;
    }

    public string BuildDexComGlucoseValueUrl(string sessionId)
    {
        var host = GetDexComServer();
        var url = $"https://{host}/ShareWebServices/Services/Publisher/ReadPublisherLatestGlucoseValues?sessionId={sessionId}&minutes=1440&maxCount=1";
        return url;
    }
}
