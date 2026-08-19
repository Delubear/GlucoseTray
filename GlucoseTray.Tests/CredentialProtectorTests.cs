using GlucoseTray;

namespace GlucoseTray.Tests;

public class CredentialProtectorTests
{
    private readonly DpapiCredentialProtector _protector = new();

    [Test]
    public void ShouldRoundTripAProtectedValue()
    {
        var original = "super-secret-password";

        var protectedValue = _protector.Protect(original);
        var unprotectedValue = _protector.Unprotect(protectedValue);

        Assert.That(unprotectedValue, Is.EqualTo(original));
    }

    [Test]
    public void ShouldMarkProtectedValuesWithPrefix()
    {
        var protectedValue = _protector.Protect("token");

        Assert.That(_protector.IsProtected(protectedValue), Is.True);
        Assert.That(protectedValue, Does.StartWith("ENC:"));
    }

    [Test]
    public void ShouldNotDoubleProtectAnAlreadyProtectedValue()
    {
        var protectedOnce = _protector.Protect("token");
        var protectedTwice = _protector.Protect(protectedOnce);

        Assert.That(protectedTwice, Is.EqualTo(protectedOnce));
    }

    [Test]
    public void ShouldReturnPlaintextUnchangedWhenUnprotectingUnprotectedValue()
    {
        const string plaintext = "not-encrypted";

        Assert.That(_protector.Unprotect(plaintext), Is.EqualTo(plaintext));
        Assert.That(_protector.IsProtected(plaintext), Is.False);
    }

    [Test]
    public void ShouldTreatEmptyValuesAsPassthrough()
    {
        Assert.That(_protector.Protect(string.Empty), Is.EqualTo(string.Empty));
        Assert.That(_protector.Unprotect(string.Empty), Is.EqualTo(string.Empty));
    }
}
