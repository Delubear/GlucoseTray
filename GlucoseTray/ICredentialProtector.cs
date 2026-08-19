using System.Security.Cryptography;
using System.Text;

namespace GlucoseTray;

public interface ICredentialProtector
{
    bool IsProtected(string? value);
    string Protect(string? plaintext);
    string Unprotect(string? storedValue);
}

public class DpapiCredentialProtector : ICredentialProtector
{
    private const string Prefix = "ENC:";

    public bool IsProtected(string? value) => value?.StartsWith(Prefix, StringComparison.Ordinal) == true;

    public string Protect(string? plaintext)
    {
        if (string.IsNullOrEmpty(plaintext) || IsProtected(plaintext))
            return plaintext ?? string.Empty;

        var bytes = Encoding.UTF8.GetBytes(plaintext);
        var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        return Prefix + Convert.ToBase64String(encrypted);
    }

    public string Unprotect(string? storedValue)
    {
        if (string.IsNullOrEmpty(storedValue) || !IsProtected(storedValue))
            return storedValue ?? string.Empty;

        var encrypted = Convert.FromBase64String(storedValue[Prefix.Length..]);
        var bytes = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(bytes);
    }
}
