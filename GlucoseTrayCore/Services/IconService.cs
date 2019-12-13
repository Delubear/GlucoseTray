using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GlucoseTrayCore.Services
{
    public class IconService
    {
        private readonly Font fontToUse = new Font("Trebuchet MS", 10, FontStyle.Regular, GraphicsUnit.Pixel);
        private readonly ILogger _logger;

        public IconService(ILogger logger) => _logger = logger;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        public void DestroyMyIcon(IntPtr handle) => DestroyIcon(handle);

        internal Brush SetColor(double val)
        {
            switch (val)
            {
                case double n when n < Constants.HighBg && n > Constants.LowBg:
                    return new SolidBrush(Color.White);

                case double n when n >= Constants.HighBg && n < Constants.DangerHighBg:
                    return new SolidBrush(Color.Yellow);

                case double n when n >= Constants.DangerHighBg:
                    return new SolidBrush(Color.Red);

                case double n when n <= Constants.LowBg && n > Constants.DangerLowBg:
                    return new SolidBrush(Color.Yellow);

                case double n when n <= Constants.DangerLowBg && n > Constants.CriticalLowBg:
                    return new SolidBrush(Color.Red);

                case double n when n <= Constants.CriticalLowBg && n > 0:
                    return new SolidBrush(Color.Red);

                default:
                    return new SolidBrush(Color.White);
            }
        }

        internal void CreateTextIcon(double val, bool isCriticalLow, NotifyIcon trayIcon)
        {
            var str = val.ToString();
            if (str.Contains(".")) // Round MMOL decimal values to one decimal place.
                str = val.ToString("0.0");
            Brush brushToUse = SetColor(val);

            if (isCriticalLow)
            {
                _logger.LogInformation("Critical low glucose read.");
                str = "DAN";
            }
            else if (str == "0")
            {
                _logger.LogWarning("Empty glucose result received.");
                str = "NUL";
            }

            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(bitmapText);

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -2, 0);
            var hIcon = bitmapText.GetHicon();
            var myIcon = Icon.FromHandle(hIcon);
            trayIcon.Icon = myIcon;

            DestroyMyIcon(myIcon.Handle);
            bitmapText.Dispose();
            g.Dispose();
            myIcon.Dispose();
        }
    }
}