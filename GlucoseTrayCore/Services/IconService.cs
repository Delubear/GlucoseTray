using Dexcom.Fetch.Extensions;
using Dexcom.Fetch.Models;
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
            return val switch
            {
                double n when n < Constants.HighBg && n > Constants.LowBg => new SolidBrush(Color.White),
                double n when n >= Constants.HighBg && n < Constants.DangerHighBg => new SolidBrush(Color.Yellow),
                double n when n >= Constants.DangerHighBg => new SolidBrush(Color.Red),
                double n when n <= Constants.LowBg && n > Constants.DangerLowBg => new SolidBrush(Color.Yellow),
                double n when n <= Constants.DangerLowBg && n > Constants.CriticalLowBg => new SolidBrush(Color.Red),
                double n when n <= Constants.CriticalLowBg && n > 0 => new SolidBrush(Color.Red),
                _ => new SolidBrush(Color.White),
            };
        }

        internal void CreateTextIcon(GlucoseFetchResult fetchResult, bool isCriticalLow, NotifyIcon trayIcon)
        {
            var result = fetchResult.GetFormattedStringValue();

            if (result == "0")
            {
                _logger.LogWarning("Empty glucose result received.");
                result = "NUL";
            }
            else if (isCriticalLow)
            {
                _logger.LogInformation("Critical low glucose read.");
                result = "DAN";
            }

            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(bitmapText);
            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(result, fontToUse, SetColor(fetchResult.Value), -2, 0);
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