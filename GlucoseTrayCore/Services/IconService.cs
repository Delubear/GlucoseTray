using Dexcom.Fetch.Enums;
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
        private readonly ILogger _logger;
        private readonly float _standardOffset = -3f;
        private readonly int _defaultFontSize = 10;
        private readonly int _smallerFontSize = 9;
        private Font _fontToUse;
        private bool _useDefaultFontSize = true;

        public IconService(ILogger logger) => _logger = logger;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        public void DestroyMyIcon(IntPtr handle) => DestroyIcon(handle);

        internal Brush SetColor(double val) => val switch
        {
            double n when n < Constants.HighBg && n > Constants.LowBg => new SolidBrush(Color.White),
            double n when n >= Constants.HighBg && n < Constants.DangerHighBg => new SolidBrush(Color.Yellow),
            double n when n >= Constants.DangerHighBg => new SolidBrush(Color.Red),
            double n when n <= Constants.LowBg && n > Constants.DangerLowBg => new SolidBrush(Color.Yellow),
            double n when n <= Constants.DangerLowBg && n > Constants.CriticalLowBg => new SolidBrush(Color.Red),
            double n when n <= Constants.CriticalLowBg && n > 0 => new SolidBrush(Color.Red),
            _ => new SolidBrush(Color.White),
        };

        internal void CreateTextIcon(GlucoseFetchResult fetchResult, bool isCriticalLow, NotifyIcon trayIcon)
        {
            var result = fetchResult.GetFormattedStringValue().Replace('.', '\''); // Use ' instead of . since it is narrower and allows a better display of a two digit number + decimal place.

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

            var xOffset = CalculateXPosition(fetchResult);
            var fontSize = _useDefaultFontSize ? _defaultFontSize : _smallerFontSize;
            _fontToUse = new Font("Roboto", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

            var bitmapText = new Bitmap(16, 16);
            var g = Graphics.FromImage(bitmapText);
            g.Clear(Color.Transparent);
            g.DrawString(result, _fontToUse, SetColor(fetchResult.Value), xOffset, 0f);
            var hIcon = bitmapText.GetHicon();
            var myIcon = Icon.FromHandle(hIcon);
            trayIcon.Icon = myIcon;

            DestroyMyIcon(myIcon.Handle);
            bitmapText.Dispose();
            g.Dispose();
            myIcon.Dispose();
        }

        private float CalculateXPosition(GlucoseFetchResult result)
        {
            _useDefaultFontSize = true;
            var value = result.Value;
            if (result.UnitDisplayType == GlucoseUnitType.MG) // Non MMOL display, use our standard offset.
                return _standardOffset;
            if (value > 9.9 && value < 20) // MMOL display with 3 digits, use a greater offset to fit display.
                return -5f;
            if (value > 19.9) // MMOL with 3 digits over 20.  Since 2 is a lot wider than 1, this requires also changing the font size from 10 to 9.
            {
                _useDefaultFontSize = false;
                return _standardOffset;
            }
            return _standardOffset; // MMOL display with only two digits, use our standard offset.
        }
    }
}