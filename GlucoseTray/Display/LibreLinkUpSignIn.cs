using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GlucoseTray.Display;

public interface ILibreLinkUpSignIn
{
    void ShowDialog();
    void SignOut();
}

public sealed class LibreLinkUpSignIn(ILibreLinkUpCredentialStore store, IOptionsMonitor<AppSettings> options, IConfiguration configuration) : ILibreLinkUpSignIn
{
    public void ShowDialog()
    {
        using var form = new Form
        {
            Text = "GlucoseTray — LibreLinkUp sign in", ClientSize = new Size(460, 245),
            FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false,
            StartPosition = FormStartPosition.CenterScreen,
        };
        var email = new TextBox { Left = 24, Top = 45, Width = 410, Text = options.CurrentValue.LibreLinkUpUsername, MaxLength = 254 };
        var password = new TextBox { Left = 24, Top = 103, Width = 410, UseSystemPasswordChar = true, MaxLength = 1280 };
        var save = new Button { Text = "Save and connect", Left = 266, Top = 197, Width = 168 };
        var cancel = new Button { Text = "Cancel", Left = 158, Top = 197, Width = 96, DialogResult = DialogResult.Cancel };
        form.Controls.AddRange([
            new Label { Text = "LibreLinkUp follower email", Left = 24, Top = 23, Width = 410 }, email,
            new Label { Text = "Password", Left = 24, Top = 81, Width = 410 }, password,
            new Label { Text = "Your password is saved in Windows Credential Manager.\r\nIt is never written to appsettings.json.", Left = 24, Top = 143, Width = 410, Height = 45 },
            save, cancel,
        ]);
        form.AcceptButton = save;
        form.CancelButton = cancel;
        save.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(email.Text) || string.IsNullOrEmpty(password.Text))
            {
                MessageBox.Show(form, "Enter your LibreLinkUp follower email and password.", "LibreLinkUp sign in");
                return;
            }
            try
            {
                store.SavePassword(email.Text.Trim(), password.Text);
                LibreLinkUpAccountSettings.SetAccount(AppSettings.FileName, email.Text.Trim());
                (configuration as IConfigurationRoot)?.Reload();
                password.Clear();
                form.DialogResult = DialogResult.OK;
            }
            catch
            {
                MessageBox.Show(form, "Could not save the account. Check access to Windows Credential Manager and the settings folder.", "LibreLinkUp sign in", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        };
        try { form.ShowDialog(); }
        finally { password.Clear(); }
    }

    public void SignOut()
    {
        try
        {
            store.DeletePassword(options.CurrentValue.LibreLinkUpUsername);
            LibreLinkUpAccountSettings.SetAccount(AppSettings.FileName, string.Empty);
            (configuration as IConfigurationRoot)?.Reload();
        }
        catch
        {
            MessageBox.Show("Could not complete sign-out. Check Windows Credential Manager and the settings folder.", "LibreLinkUp sign out", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
