using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GlucoseTray.Enums;

namespace GlucoseTray.Read.LibreLinkUp;

public interface ILibreLinkUpClient
{
    Task<GlucoseReading> GetLatestGlucoseAsync(AppSettings settings);
}

/// <summary>
/// Read-only client for the unofficial LibreLinkUp API. Protocol reference:
/// https://github.com/timoschlueter/nightscout-librelink-up
/// Tokens are kept in memory and shared between refreshes, not written to settings.
/// </summary>
public sealed class LibreLinkUpClient(IHttpClientFactory httpClientFactory, ILibreLinkUpCredentialStore credentials) : ILibreLinkUpClient
{
    public const string HttpClientName = "LibreLinkUp";
    private static readonly Dictionary<string, string> Hosts = new(StringComparer.OrdinalIgnoreCase)
    {
        ["AE"] = "api-ae.libreview.io", ["AP"] = "api-ap.libreview.io",
        ["AU"] = "api-au.libreview.io", ["CA"] = "api-ca.libreview.io",
        ["DE"] = "api-de.libreview.io", ["EU"] = "api-eu.libreview.io",
        ["EU2"] = "api-eu2.libreview.io", ["FR"] = "api-fr.libreview.io",
        ["JP"] = "api-jp.libreview.io", ["US"] = "api-us.libreview.io",
        ["LA"] = "api-la.libreview.io", ["RU"] = "api.libreview.ru",
        ["CN"] = "api-cn.myfreestyle.cn",
    };

    private SessionIdentity? _identity;
    private string? _host;
    private string? _token;
    private string? _accountId;
    private long _expires;

    public async Task<GlucoseReading> GetLatestGlucoseAsync(AppSettings settings)
    {
        try { return await ReadAsync(settings); }
        catch (GlucoseReadException) { throw; }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new GlucoseReadException("LibreLinkUp sign-in failed. Check your follower credentials and sign in to the official LibreLinkUp app.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            throw new GlucoseReadException($"LibreLinkUp request was refused or failed (HTTP {(int)ex.StatusCode.Value}). Check the official app and your internet connection.");
        }
        catch (Exception)
        {
            throw new GlucoseReadException("LibreLinkUp could not update. Check your internet connection and sign in to the official LibreLinkUp app to complete any pending setup.");
        }
    }

    private async Task<GlucoseReading> ReadAsync(AppSettings settings)
    {
        var identity = new SessionIdentity(settings.LibreLinkUpUsername.Trim(), credentials.Revision,
            settings.LibreLinkUpRegion.Trim().ToUpperInvariant(), settings.LibreLinkUpApiVersion.Trim());
        var password = credentials.GetPassword(identity.Username);
        if (string.IsNullOrWhiteSpace(identity.Username) || string.IsNullOrEmpty(password))
        {
            ClearSession();
            throw new GlucoseReadException("LibreLinkUp is signed out. Right-click the tray icon and choose LibreLinkUp sign in.", false);
        }
        if (!Hosts.ContainsKey(identity.Region))
            throw new GlucoseReadException("Unsupported LibreLinkUpRegion. See the README for supported region codes.", false);
        if (!Version.TryParse(identity.Version, out _))
            throw new GlucoseReadException("LibreLinkUpApiVersion must be an app version such as 4.16.0.", false);

        if (_identity != identity)
        {
            ClearSession();
            _identity = identity;
        }

        using var client = httpClientFactory.CreateClient(HttpClientName);
        for (var attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                if (_token is null || _expires <= DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 30)
                    await LoginAsync(client, identity, password);

                using var response = await SendAsync(client, identity, HttpMethod.Get, _host!, "/llu/connections", authenticated: true);
                var connections = RequireData(response);
                var measurement = SelectMeasurement(connections, settings.LibreLinkUpPatientId);
                return measurement;
            }
            catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                ClearSession();
                if (attempt == 1)
                    throw;
            }
        }
        throw new InvalidOperationException("LibreLinkUp authentication failed. Sign in to the official LibreLinkUp app to check your account.");
    }

    private async Task LoginAsync(HttpClient client, SessionIdentity identity, string password)
    {
        ClearSession();
        var region = identity.Region;
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (visited.Add(region) && visited.Count <= 3)
        {
            if (!Hosts.TryGetValue(region, out var host))
                throw new InvalidOperationException("LibreLinkUp returned an unsupported region. Check your account in the official app.");
            var body = JsonSerializer.Serialize(new { email = identity.Username, password });
            using var response = await SendAsync(client, identity, HttpMethod.Post, host, "/llu/auth/login", body);
            var data = RequireData(response);
            if (data.TryGetProperty("redirect", out var redirect) && redirect.ValueKind == JsonValueKind.True)
            {
                region = data.GetProperty("region").GetString() ?? string.Empty;
                continue;
            }

            var ticket = data.GetProperty("authTicket");
            var token = ticket.GetProperty("token").GetString();
            var userId = data.GetProperty("user").GetProperty("id").GetString();
            var expires = ticket.GetProperty("expires").GetInt64();
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(userId) || expires <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                throw new InvalidOperationException("LibreLinkUp did not return a valid session. Check your account in the official app.");
            _host = host;
            _token = token;
            _accountId = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(userId))).ToLowerInvariant();
            _expires = expires;
            return;
        }
        throw new InvalidOperationException("LibreLinkUp region redirect limit reached. Check LibreLinkUpRegion.");
    }

    private async Task<JsonDocument> SendAsync(HttpClient client, SessionIdentity identity, HttpMethod method,
        string host, string path, string? body = null, bool authenticated = false)
    {
        using var request = new HttpRequestMessage(method, $"https://{host}{path}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        // Abbott's gateway rejects .NET requests with no User-Agent before they reach the API.
        request.Headers.UserAgent.ParseAdd("GlucoseTray/1.0");
        request.Headers.Add("product", "llu.ios");
        request.Headers.Add("version", identity.Version);
        if (authenticated)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            request.Headers.Add("account-id", _accountId);
        }
        if (body is not null)
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"LibreLinkUp request failed (HTTP {(int)response.StatusCode}). Check the official app and internet connection.", null, response.StatusCode);
        // Do not include response bodies in errors: they may contain credentials or health data.
        try
        {
            return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("LibreLinkUp returned an unreadable response.");
        }
    }

    private static JsonElement RequireData(JsonDocument response)
    {
        var root = response.RootElement;
        if (!root.TryGetProperty("status", out var status) || !status.TryGetInt32(out var code))
            throw new InvalidOperationException("LibreLinkUp returned an invalid response.");
        if (code == 2)
            throw new HttpRequestException("LibreLinkUp authentication was rejected. Check your credentials in the official app.", null, HttpStatusCode.Unauthorized);
        if (code != 0)
            throw new GlucoseReadException($"LibreLinkUp returned status {code}. Open the official LibreLinkUp app and complete any pending consent, email verification, or terms acceptance.");
        if (!root.TryGetProperty("data", out var data) || data.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array))
            throw new InvalidOperationException("LibreLinkUp returned no data.");
        return data;
    }

    private static GlucoseReading SelectMeasurement(JsonElement connections, string patientId)
    {
        if (connections.ValueKind != JsonValueKind.Array || connections.GetArrayLength() == 0)
            throw new GlucoseReadException("No LibreLinkUp connections found. Accept your sharing invitation in the official LibreLinkUp app first.", false);
        patientId = patientId.Trim();
        if (patientId.Length == 0 && connections.GetArrayLength() != 1)
            throw new GlucoseReadException("Multiple LibreLinkUp connections found. Set LibreLinkUpPatientId to select one; no reading was selected automatically.", false);
        foreach (var connection in connections.EnumerateArray())
        {
            if (connection.ValueKind != JsonValueKind.Object ||
                !connection.TryGetProperty("patientId", out var id) || id.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(id.GetString()))
                throw new GlucoseReadException("LibreLinkUp returned a connection without a patient ID. Check sharing in the official app.", false);
            if (patientId.Length == 0 || id.GetString() == patientId)
            {
                try
                {
                    if (!connection.TryGetProperty("glucoseMeasurement", out var measurement) || measurement.ValueKind != JsonValueKind.Object)
                        throw new InvalidOperationException("No measurement.");
                    var reading = MapMeasurement(measurement);
                    reading.SubjectId = id.GetString();
                    return reading;
                }
                catch (Exception)
                {
                    throw new GlucoseReadException("LibreLinkUp has no usable reading for the selected patient. Check the official app and sensor connection.", subjectId: id.GetString());
                }
            }
        }
        throw new GlucoseReadException("LibreLinkUpPatientId does not match a shared connection.", false);
    }

    private static GlucoseReading MapMeasurement(JsonElement measurement)
    {
        // ValueInMgPerDl is always mg/dL, regardless of the account's display units.
        // FactoryTimestamp is UTC; Timestamp is the patient's local time and is deliberately not used.
        if (!measurement.TryGetProperty("ValueInMgPerDl", out var value) || value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out var mg) || mg <= 0)
            throw new InvalidOperationException("LibreLinkUp returned no usable glucose value.");
        if (!measurement.TryGetProperty("FactoryTimestamp", out var timestamp) || timestamp.ValueKind != JsonValueKind.String ||
            !DateTimeOffset.TryParseExact(timestamp.GetString(), ["M/d/yyyy h:mm:ss tt", "M/d/yyyy H:mm:ss", "yyyy-MM-dd'T'HH:mm:ss'Z'", "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF'Z'"],
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var utc))
            throw new InvalidOperationException("LibreLinkUp returned no usable UTC measurement time; the reading was rejected.");
        if (utc > DateTimeOffset.UtcNow.AddMinutes(5))
            throw new InvalidOperationException("LibreLinkUp returned a measurement time in the future; check your PC clock.");
        var trend = measurement.TryGetProperty("TrendArrow", out var arrow) && arrow.ValueKind == JsonValueKind.Number && arrow.TryGetInt32(out var number) ? number : 0;
        return new GlucoseReading
        {
            MgValue = mg,
            MmolValue = mg / 18.0182f,
            TimestampUtc = utc.UtcDateTime,
            Trend = trend switch
            {
                1 => Trend.SingleDown, 2 => Trend.FortyFiveDown, 3 => Trend.Flat,
                4 => Trend.FortyFiveUp, 5 => Trend.SingleUp, _ => Trend.Unknown,
            },
        };
    }

    private void ClearSession()
    {
        _host = null;
        _token = null;
        _accountId = null;
        _expires = 0;
    }

    private sealed record SessionIdentity(string Username, long CredentialRevision, string Region, string Version);
}
