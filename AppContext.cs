using System;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlucoseTray
{
    public class AppContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private readonly int HighBg = int.Parse(ConfigurationManager.AppSettings["HighBg"]);
        private readonly int LowBg = int.Parse(ConfigurationManager.AppSettings["LowBg"]);
        private GlucoseFetchResult FetchResult;

        public AppContext()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Exit", Exit)
            }),
                Visible = true
            };

            trayIcon.Click += ShowBalloon;

            // Pull new data every 5 minutes
            //var startTimeSpan = TimeSpan.Zero;
            //var periodTimeSpan = TimeSpan.FromMinutes(5);
            //var timer = new System.Threading.Timer((_) => CreateIcon(), null, startTimeSpan, periodTimeSpan);

            // trying this since the above stopped updating after a few hours at work.
            while (true)
            {
                try
                {
                    CreateIcon();
                    Task.Delay(60000);
                }
                catch (Exception e)
                {
                    System.IO.File.AppendAllText(@"c:\TEMP\TrayError.txt", e.Message + e.Message + e.InnerException + e.StackTrace);
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void CreateTextIcon(string str)
        {
            Font fontToUse = new Font("Trebuchet MS", 10, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brushToUse = SetColor();

            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(bitmapText);

            IntPtr hIcon;

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -2, 0);
            hIcon = bitmapText.GetHicon();
            trayIcon.Icon = Icon.FromHandle(hIcon);
            //DestroyIcon(hIcon.ToInt32);
        }

        private Brush SetColor()
        {
            if (HighBg <= FetchResult.Value)
                return new SolidBrush(Color.Yellow);
            else if (LowBg >= FetchResult.Value)
                return new SolidBrush(Color.Red);
            else
                return new SolidBrush(Color.White);
        }

        private void CreateIcon()
        {
            var service = new GlucoseFetch();
            FetchResult = service.GetLatestReading();
            trayIcon.Text = $"{FetchResult.Time.ToLongTimeString()}  {GetTrendArrow(FetchResult.Trend)}";
            CreateTextIcon(FetchResult.Value.ToString());
        }

        private void ShowBalloon(object sender, EventArgs e)
        {
            trayIcon.ShowBalloonTip(2000, "Glucose", $"{FetchResult.Time.ToLongTimeString()}    {GetTrendArrow(FetchResult.Trend)}", ToolTipIcon.Info);
        }

        private string GetTrendArrow(string trend)
        {
            if (string.Equals(trend, "Flat", StringComparison.OrdinalIgnoreCase))
                return "→";
            else if (string.Equals(trend, "FortyFiveDown", StringComparison.OrdinalIgnoreCase))
                return "↘";
            else if (string.Equals(trend, "FortyFiveUp", StringComparison.OrdinalIgnoreCase))
                return "↗";
            else if (string.Equals(trend, "SingleDown", StringComparison.OrdinalIgnoreCase))
                return "↓";
            else if (string.Equals(trend, "SingleUp", StringComparison.OrdinalIgnoreCase))
                return "↑";
            else if (string.Equals(trend, "DoubleDown", StringComparison.OrdinalIgnoreCase))
                return "⮇";
            else if (string.Equals(trend, "DoubleUp", StringComparison.OrdinalIgnoreCase))
                return "⮅";
            else
                return "";
        }
    }
}
