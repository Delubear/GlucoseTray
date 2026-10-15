using System.Text.Json;
using System.Text.Json.Nodes;

namespace GlucoseTray;

public static class LibreLinkUpAccountSettings
{
    public static void SetAccount(string path, string username)
    {
        var root = Read(path);
        root["LibreLinkUpUsername"] = username.Trim();
        root.Remove("LibreLinkUpPassword");
        Write(path, root);
    }

    public static bool MigrateLegacyPassword(string path, ILibreLinkUpCredentialStore store, ICredentialProtector protector)
    {
        if (!File.Exists(path)) return false;
        var root = Read(path);
        if (!root.TryGetPropertyValue("LibreLinkUpPassword", out var property)) return false;
        var password = property?.GetValue<string>();
        if (!string.IsNullOrEmpty(password))
        {
            var username = root["LibreLinkUpUsername"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("A legacy LibreLinkUp password needs its follower email before it can be migrated to Windows Credential Manager.");
            // A failed vault write must leave the original recoverable settings intact.
            if (string.IsNullOrEmpty(store.GetPassword(username)))
                store.SavePassword(username, protector.Unprotect(password));
        }
        root.Remove("LibreLinkUpPassword");
        Write(path, root);
        return true;
    }

    private static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))?.AsObject()
        ?? throw new InvalidOperationException("The settings file is empty.");

    private static void Write(string path, JsonObject root) => File.WriteAllText(path, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
}
