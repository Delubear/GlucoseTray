using System.Text.Json;

namespace GlucoseTray;

public interface ICredentialMigrator
{
    bool ProtectFile(string filePath);
}

public class CredentialMigrator(ICredentialProtector protector) : ICredentialMigrator
{
    /// <summary>
    /// Encrypts any plaintext credentials in the settings file in place.
    /// Returns true if the file was modified.
    /// </summary>
    public bool ProtectFile(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(filePath), AppSettingsJson.SerializerOptions);
        if (settings is null)
            return false;

        var changed = false;
        if (!string.IsNullOrEmpty(settings.DexcomPassword) && !protector.IsProtected(settings.DexcomPassword))
        {
            settings.DexcomPassword = protector.Protect(settings.DexcomPassword);
            changed = true;
        }
        if (!string.IsNullOrEmpty(settings.NightscoutToken) && !protector.IsProtected(settings.NightscoutToken))
        {
            settings.NightscoutToken = protector.Protect(settings.NightscoutToken);
            changed = true;
        }

        if (changed)
            File.WriteAllText(filePath, JsonSerializer.Serialize(settings, AppSettingsJson.SerializerOptions));

        return changed;
    }
}
