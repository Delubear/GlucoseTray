using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GlucoseTray.Services;

/// <summary>
/// Used example from here: https://tekeye.uk/visual_studio/encrypt-decrypt-c-sharp-string
/// </summary>
public static class StringEncryptionService
{
    // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be 32 bytes long.
    // Using a 16 character string here gives us 32 bytes when converted to a byte array.
    private const string initVector = "pemgail9uzpgzl88";

    // This constant is used to determine the keysize of the encryption algorithm
    private const int keysize = 256;

    public static string EncryptString(string plainText, string passPhrase)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var password = new PasswordDeriveBytes(passPhrase, null);
        byte[] keyBytes = password.GetBytes(keysize / 8);
        var symmetricKey = Aes.Create();
        symmetricKey.Mode = CipherMode.CBC;
        ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
        var memoryStream = new MemoryStream();
        var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();
        byte[] cipherTextBytes = memoryStream.ToArray();
        memoryStream.Close();
        cryptoStream.Close();
        return Convert.ToBase64String(cipherTextBytes);
    }

    public static string DecryptString(string cipherText, string passPhrase)
    {
        byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
        var password = new PasswordDeriveBytes(passPhrase, null);
        byte[] keyBytes = password.GetBytes(keysize / 8);
        var symmetricKey = Aes.Create();
        symmetricKey.Mode = CipherMode.CBC;
        ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        var memoryStream = new MemoryStream(cipherTextBytes);
        var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        byte[] plainTextBytes = new byte[cipherTextBytes.Length];

        // Amended for Dotnet 6.0 and above which now longer guarentees that you will get all the bytes you asked for in the first read, so we need to keep reading until the end.
        int decryptedByteCount = 0;
        byte[] tempBytes = new byte[cipherTextBytes.Length];
        do
        {
            int bytesRead = cryptoStream.Read(tempBytes, 0, tempBytes.Length);
            if (bytesRead == 0)
                break;

            Array.Copy(tempBytes, 0, plainTextBytes, decryptedByteCount, bytesRead);
            decryptedByteCount += bytesRead;
        }
        while (true);

        memoryStream.Close();
        cryptoStream.Close();
        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
    }

    public static bool IsEncrypted(string cipherText, string passPhrase)
    {
        var isEncrypted = true;
        try
        {
            var result = DecryptString(cipherText, passPhrase);
        }
        catch
        {
            isEncrypted = false;
        }
        return isEncrypted;
    }
}
