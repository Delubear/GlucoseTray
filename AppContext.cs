using System;
using System.Configuration;
using System.Drawing;
using System.Runtime.InteropServices;
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

        IntPtr hIcon;
        Font fontToUse = new Font("Trebuchet MS", 10, FontStyle.Regular, GraphicsUnit.Pixel);

        public AppContext()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                ContextMenu = new ContextMenu(new MenuItem[] { new MenuItem("Exit", Exit) }),
                Visible = true
            };

            trayIcon.Click += ShowBalloon;

            while (true)
            {
                try
                {
                    CreateIcon();
                    Task.Delay(5000).Wait();
                }
                catch (Exception e)
                {
                    trayIcon?.Dispose();
                    System.IO.File.AppendAllText(@"c:\TEMP\TrayError.txt", DateTime.Now.ToString() + e.Message + e.Message + e.InnerException + e.StackTrace + Environment.NewLine + Environment.NewLine);
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            if(trayIcon != null)
                trayIcon.Visible = false;
            Application.Exit();
        }

        private void CreateTextIcon(string str)
        {
            Brush brushToUse = SetColor();
            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(bitmapText);

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -2, 0);
            hIcon = bitmapText.GetHicon();
            trayIcon.Icon = Icon.FromHandle(hIcon);
            bitmapText.Dispose();
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
