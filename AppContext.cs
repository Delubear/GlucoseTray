using System;
using System.Diagnostics;
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
        private IconService _winService;
        
        public AppContext()
        {
            _winService = new IconService();

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                ContextMenu = new ContextMenu(new MenuItem[] 
                {
                    new MenuItem("Nightscout", (obj,e) => Process.Start(Constants.NightscoutUrl)),
                    new MenuItem("Exit", new EventHandler(Exit))
                }),
                Visible = true
            };

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
                    System.IO.File.AppendAllText(Constants.ErrorLogPath, DateTime.Now.ToString() + e.Message + e.Message + e.InnerException + e.StackTrace + Environment.NewLine + Environment.NewLine);
                    trayIcon.Visible = false;
                    trayIcon?.Dispose();
                    Environment.Exit(0);
                }
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
            _winService.CreateTextIcon(FetchResult.Value, IsCriticalLow, trayIcon);
        }

        private void ShowBalloon(object sender, EventArgs e)
        {
            trayIcon.ShowBalloonTip(2000, "Glucose", $"{FetchResult.Value}   {FetchResult.Time.ToLongTimeString()}    {FetchResult.TrendIcon}", ToolTipIcon.Info);
        }
    }
}
