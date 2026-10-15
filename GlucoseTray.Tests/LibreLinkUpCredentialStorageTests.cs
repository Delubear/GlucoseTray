using System.Text.Json;
using System.Text.Json.Nodes;
using NSubstitute;

namespace GlucoseTray.Tests;

public class LibreLinkUpCredentialStorageTests
{
    private string _path = null!;
    [SetUp]
    public void SetUp() => _path = Path.Combine(Path.GetTempPath(), $"glucose-vault-{Guid.NewGuid():N}.json");
    [TearDown]
    public void TearDown() { if (File.Exists(_path)) File.Delete(_path); }

    [Test]
    public void NewSettingsMustNotContainPasswordField()
    {
        Assert.That(JsonSerializer.Serialize(new AppSettings()), Does.Not.Contain("LibreLinkUpPassword"));
    }

    [TestCase("synthetic-password")]
    [TestCase("ENC:synthetic-password")]
    public void LegacyPasswordShouldMoveToVaultAndBeRemovedFromSettings(string stored)
    {
        File.WriteAllText(_path, new JsonObject { ["LibreLinkUpUsername"] = "test@example.invalid", ["LibreLinkUpPassword"] = stored, ["UnrelatedOption"] = 42 }.ToJsonString());
        var vault = Substitute.For<ILibreLinkUpCredentialStore>();
        var protector = Substitute.For<ICredentialProtector>();
        protector.Unprotect(stored).Returns("synthetic-password");

        Assert.That(LibreLinkUpAccountSettings.MigrateLegacyPassword(_path, vault, protector), Is.True);

        vault.Received(1).SavePassword("test@example.invalid", "synthetic-password");
        var after = File.ReadAllText(_path);
        Assert.That(after, Does.Not.Contain("synthetic-password").And.Not.Contain("LibreLinkUpPassword"));
        Assert.That(JsonNode.Parse(after)!["UnrelatedOption"]!.GetValue<int>(), Is.EqualTo(42));
        Assert.That(LibreLinkUpAccountSettings.MigrateLegacyPassword(_path, vault, protector), Is.False);
    }

    [Test]
    public void FailedVaultWriteMustLeaveLegacySettingsIntact()
    {
        const string original = """{"LibreLinkUpUsername":"test@example.invalid","LibreLinkUpPassword":"synthetic-password"}""";
        File.WriteAllText(_path, original);
        var vault = Substitute.For<ILibreLinkUpCredentialStore>();
        vault.When(v => v.SavePassword(Arg.Any<string>(), Arg.Any<string>())).Do(_ => throw new InvalidOperationException("Synthetic vault failure"));
        var protector = Substitute.For<ICredentialProtector>();
        protector.Unprotect("synthetic-password").Returns("synthetic-password");
        Assert.Throws<InvalidOperationException>(() => LibreLinkUpAccountSettings.MigrateLegacyPassword(_path, vault, protector));
        Assert.That(File.ReadAllText(_path), Is.EqualTo(original));
    }

    [Test]
    public void EmptyLegacyPasswordShouldBeRemovedWithoutVaultWrite()
    {
        File.WriteAllText(_path, """{"LibreLinkUpPassword":""}""");
        var vault = Substitute.For<ILibreLinkUpCredentialStore>();
        LibreLinkUpAccountSettings.MigrateLegacyPassword(_path, vault, Substitute.For<ICredentialProtector>());
        vault.DidNotReceive().SavePassword(Arg.Any<string>(), Arg.Any<string>());
        Assert.That(File.ReadAllText(_path), Does.Not.Contain("LibreLinkUpPassword"));
    }

    [Test]
    public void MigrationMustNotOverwriteNewerCredentialAlreadyInVault()
    {
        File.WriteAllText(_path, """{"LibreLinkUpUsername":"test@example.invalid","LibreLinkUpPassword":"old-synthetic-password"}""");
        var vault = Substitute.For<ILibreLinkUpCredentialStore>();
        vault.GetPassword("test@example.invalid").Returns("new-synthetic-password");
        LibreLinkUpAccountSettings.MigrateLegacyPassword(_path, vault, Substitute.For<ICredentialProtector>());
        vault.DidNotReceive().SavePassword(Arg.Any<string>(), Arg.Any<string>());
        Assert.That(File.ReadAllText(_path), Does.Not.Contain("LibreLinkUpPassword"));
    }

    [Test]
    public void SavingAccountMustPreserveSettingsWithoutWritingPassword()
    {
        File.WriteAllText(_path, """{"HighMmolThreshold":12.5,"LibreLinkUpRegion":"EU"}""");
        LibreLinkUpAccountSettings.SetAccount(_path, "test@example.invalid");
        var settings = JsonNode.Parse(File.ReadAllText(_path))!;
        Assert.That(settings["HighMmolThreshold"]!.GetValue<double>(), Is.EqualTo(12.5));
        Assert.That(settings["LibreLinkUpUsername"]!.GetValue<string>(), Is.EqualTo("test@example.invalid"));
        Assert.That(File.ReadAllText(_path), Does.Not.Contain("LibreLinkUpPassword"));
    }

    [Test]
    public void WindowsCredentialManagerShouldRoundTripOverwriteAndDeleteSyntheticCredential()
    {
        var username = $"glucosetray-test-{Guid.NewGuid():N}@example.invalid";
        var vault = new WindowsLibreLinkUpCredentialStore();
        try
        {
            Assert.That(vault.GetPassword(username), Is.Null);
            vault.SavePassword(username, "Synthetic ünicode password 🔒");
            Assert.That(vault.GetPassword(username), Is.EqualTo("Synthetic ünicode password 🔒"));
            vault.SavePassword(username, "updated synthetic password");
            Assert.That(vault.GetPassword(username), Is.EqualTo("updated synthetic password"));
            vault.DeletePassword(username);
            Assert.That(vault.GetPassword(username), Is.Null);
            Assert.That(vault.Revision, Is.EqualTo(3));
        }
        finally { vault.DeletePassword(username); }
    }
}
