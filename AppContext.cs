using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlucoseTray.Models;
using GlucoseTray.Services;

namespace GlucoseTray
{
    public class AppContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon;
        private bool IsCriticalLow;

        private GlucoseFetchResult FetchResult;
        private readonly IconService _iconService;

        public AppContext()
        {
            RunStartupChecks();

            _iconService = new IconService();

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                ContextMenu = new ContextMenu(new MenuItem[]{}),
                Visible = true
            };

            if(!string.IsNullOrWhiteSpace(Constants.NightscoutUrl))
                trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Nightscout", (obj, e) => Process.Start(Constants.NightscoutUrl)));
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(Exit)));

            trayIcon.DoubleClick += ShowBalloon;

            while (true)
            {
                try
                {
                    Application.DoEvents();
                    CreateIcon();
                    Task.Delay(5000).Wait();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"ERROR: {e}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    File.AppendAllText(Constants.ErrorLogPath, DateTime.Now.ToString() + e.Message + e.Message + e.InnerException + e.StackTrace + Environment.NewLine + Environment.NewLine);
                    trayIcon.Visible = false;
                    trayIcon?.Dispose();
                    Environment.Exit(0);
                }
            }
        }

        private void RunStartupChecks()
        {
            try
            {
                if (!File.Exists(Constants.ErrorLogPath))
                    File.Create(Constants.ErrorLogPath);
            }
            catch
            {
                MessageBox.Show("ERROR: Log path unable to be created.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            trayIcon?.Dispose();
            Application.ExitThread();
            Application.Exit();
        }

        private void CreateIcon()
        {
            IsCriticalLow = false;
            var service = new GlucoseFetchService();
            FetchResult = service.GetLatestReading();
            trayIcon.Text = $"{FetchResult.Value}   {FetchResult.Time.ToLongTimeString()}  {FetchResult.TrendIcon}";
            if (FetchResult.Value <= Constants.CriticalLowBg)
                IsCriticalLow = true;
            _iconService.CreateTextIcon(FetchResult.Value, IsCriticalLow, trayIcon);
        }

        private void ShowBalloon(object sender, EventArgs e)
        {
            trayIcon.ShowBalloonTip(2000, "Glucose", $"{FetchResult.Value}   {FetchResult.Time.ToLongTimeString()}    {FetchResult.TrendIcon}", ToolTipIcon.Info);
        }
    }
}
