using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlucoseTray
{
    public class AppContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon;
        private bool IsCriticalLow;

        private GlucoseFetchResult FetchResult;
        private readonly Font fontToUse = new Font("Trebuchet MS", 10, FontStyle.Regular, GraphicsUnit.Pixel);

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
                    System.IO.File.AppendAllText(@"c:\TEMP\TrayError.txt", DateTime.Now.ToString() + e.Message + e.Message + e.InnerException + e.StackTrace + Environment.NewLine + Environment.NewLine);
                    trayIcon.Visible = false;
                    trayIcon?.Dispose();
                    Environment.Exit(0);
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        private void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            trayIcon?.Dispose();
            Application.Exit();
        }

        private void CreateTextIcon(string str)
        {
            Brush brushToUse = SetColor();

            if (IsCriticalLow)
                str = "DAN";
            else if (str == "0")
                str = "NUL";

            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(bitmapText);

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -2, 0);
            var hIcon = bitmapText.GetHicon();
            var myIcon = Icon.FromHandle(hIcon);
            trayIcon.Icon = myIcon;

            DestroyIcon(myIcon.Handle);
            bitmapText.Dispose();
            g.Dispose();
            myIcon.Dispose();
        }

        private Brush SetColor()
        {
            IsCriticalLow = false;
            switch (FetchResult.Value)
            {
                case int n when n < Constants.HighBg && n > Constants.LowBg:
                    return new SolidBrush(Color.White);
                case int n when n >= Constants.HighBg && n < Constants.DangerHighBg:
                    return new SolidBrush(Color.Yellow);
                case int n when n > Constants.DangerHighBg:
                    return new SolidBrush(Color.Red);
                case int n when n <= Constants.LowBg && n > Constants.DangerLowBg:
                    return new SolidBrush(Color.Yellow);
                case int n when n <= Constants.DangerLowBg && n > Constants.CriticalLowBg:
                    return new SolidBrush(Color.Red);
                case int n when n <= Constants.CriticalLowBg && n > 0:
                    IsCriticalLow = true;
                    return new SolidBrush(Color.Red);
                default:
                    return new SolidBrush(Color.White);
            }
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
            trayIcon.ShowBalloonTip(2000, "Glucose", $"{FetchResult.Value}   {FetchResult.Time.ToLongTimeString()}    {GetTrendArrow(FetchResult.Trend)}", ToolTipIcon.Info);
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
