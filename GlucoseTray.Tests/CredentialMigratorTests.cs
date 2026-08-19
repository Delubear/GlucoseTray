using System.Text.Json;
using System.Text.Json.Serialization;

namespace GlucoseTray.Tests;

public class CredentialMigratorTests
{
    private string _filePath = null!;

    [SetUp]
    public void SetUp() => _filePath = Path.Combine(Path.GetTempPath(), $"glucosetray-{Guid.NewGuid():N}.json");

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    private void WriteSettings(AppSettings settings) => File.WriteAllText(_filePath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } }));

    private AppSettings ReadSettings() => JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_filePath), new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } })!;

    [Test]
    public void ShouldEncryptPlaintextCredentials()
    {
        WriteSettings(new AppSettings { DexcomPassword = "pw", NightscoutToken = "tok" });
        var migrator = new CredentialMigrator(new PrefixProtector());

        var changed = migrator.ProtectFile(_filePath);

        var result = ReadSettings();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(changed, Is.True);
            Assert.That(result.DexcomPassword, Is.EqualTo("ENC:pw"));
            Assert.That(result.NightscoutToken, Is.EqualTo("ENC:tok"));
        }
    }

    [Test]
    public void ShouldNotRewriteWhenCredentialsAlreadyProtected()
    {
        WriteSettings(new AppSettings { DexcomPassword = "ENC:pw", NightscoutToken = "ENC:tok" });
        var migrator = new CredentialMigrator(new PrefixProtector());

        var changed = migrator.ProtectFile(_filePath);

        Assert.That(changed, Is.False);
    }

    [Test]
    public void ShouldLeaveEmptyCredentialsUntouched()
    {
        WriteSettings(new AppSettings { DexcomPassword = string.Empty, NightscoutToken = string.Empty });
        var migrator = new CredentialMigrator(new PrefixProtector());

        var changed = migrator.ProtectFile(_filePath);

        Assert.That(changed, Is.False);
    }

    [Test]
    public void ShouldOnlyEncryptTheUnprotectedCredential()
    {
        WriteSettings(new AppSettings { DexcomPassword = "pw", NightscoutToken = "ENC:tok" });
        var migrator = new CredentialMigrator(new PrefixProtector());

        var changed = migrator.ProtectFile(_filePath);

        var result = ReadSettings();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(changed, Is.True);
            Assert.That(result.DexcomPassword, Is.EqualTo("ENC:pw"));
            Assert.That(result.NightscoutToken, Is.EqualTo("ENC:tok"));
        }
    }

    private sealed class PrefixProtector : ICredentialProtector
    {
        private const string Prefix = "ENC:";
        public bool IsProtected(string? value) => value?.StartsWith(Prefix, StringComparison.Ordinal) == true;
        public string Protect(string? plaintext) => string.IsNullOrEmpty(plaintext) || IsProtected(plaintext) ? plaintext ?? string.Empty : Prefix + plaintext;
        public string Unprotect(string? storedValue) => IsProtected(storedValue) ? storedValue![Prefix.Length..] : storedValue ?? string.Empty;
    }
}
