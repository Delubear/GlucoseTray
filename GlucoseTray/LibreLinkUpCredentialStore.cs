using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GlucoseTray;

public interface ILibreLinkUpCredentialStore
{
    long Revision { get; }
    string? GetPassword(string username);
    void SavePassword(string username, string password);
    void DeletePassword(string username);
}

/// <summary>Current-user Windows Credential Manager storage; never writes passwords to a file.</summary>
public sealed class WindowsLibreLinkUpCredentialStore : ILibreLinkUpCredentialStore
{
    private const uint Generic = 1;
    private const int NotFound = 1168;
    private long _revision;
    public long Revision => Interlocked.Read(ref _revision);
    private static string Target(string username) => "GlucoseTray/LibreLinkUp/" + username.Trim().ToLowerInvariant();

    public string? GetPassword(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        if (!CredRead(Target(username), Generic, 0, out var pointer))
        {
            var error = Marshal.GetLastWin32Error();
            if (error == NotFound) return null;
            throw new Win32Exception(error, "Unable to read LibreLinkUp credentials from Windows Credential Manager.");
        }
        try
        {
            var credential = Marshal.PtrToStructure<Credential>(pointer);
            if (credential.BlobSize > 2560 || credential.BlobSize % 2 != 0)
                throw new InvalidOperationException("Windows Credential Manager returned an invalid credential.");
            return Marshal.PtrToStringUni(credential.Blob, (int)credential.BlobSize / 2);
        }
        finally { CredFree(pointer); }
    }

    public void SavePassword(string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrEmpty(password);
        if (password.Length > 1280)
            throw new ArgumentException("The password exceeds the Windows Credential Manager size limit.");
        var blob = Marshal.StringToCoTaskMemUni(password);
        try
        {
            var credential = new Credential
            {
                Type = Generic, TargetName = Target(username), UserName = username.Trim(),
                Blob = blob, BlobSize = checked((uint)password.Length * 2), Persist = 2,
            };
            if (!CredWrite(ref credential, 0))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to save LibreLinkUp credentials in Windows Credential Manager.");
            Interlocked.Increment(ref _revision);
        }
        finally { Marshal.ZeroFreeCoTaskMemUnicode(blob); }
    }

    public void DeletePassword(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return;
        if (!CredDelete(Target(username), Generic, 0) && Marshal.GetLastWin32Error() != NotFound)
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to remove LibreLinkUp credentials from Windows Credential Manager.");
        Interlocked.Increment(ref _revision);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct Credential
    {
        public uint Flags;
        public uint Type;
        public string TargetName;
        public string? Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint BlobSize;
        public IntPtr Blob;
        public uint Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        public string? TargetAlias;
        public string UserName;
    }

    [DllImport("advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CredRead(string target, uint type, uint flags, out IntPtr credential);
    [DllImport("advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CredWrite(ref Credential credential, uint flags);
    [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CredDelete(string target, uint type, uint flags);
    [DllImport("advapi32.dll")]
    private static extern void CredFree(IntPtr credential);
}
