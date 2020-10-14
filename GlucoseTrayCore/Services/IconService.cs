using GlucoseTrayCore.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GlucoseTrayCore.Extensions;
using GlucoseTrayCore.Models;

namespace GlucoseTrayCore.Services
{
    public class IconService
    {
        private readonly ILogger<IconService> _logger;
        private readonly GlucoseTraySettings _options;
        private readonly float _standardOffset = -3f;
        private readonly int _defaultFontSize = 10;
        private readonly int _smallerFontSize = 9;
        private Font _fontToUse;
        private bool _useDefaultFontSize = true;

        public IconService(ILogger<IconService> logger, IOptions<GlucoseTraySettings> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        public void DestroyMyIcon(IntPtr handle) => DestroyIcon(handle);

        internal Brush SetColor(double val) => val switch
        {
            double n when n < _options.HighBg && n > _options.LowBg => new SolidBrush(Color.White),
            double n when n >= _options.HighBg && n < _options.DangerHighBg => new SolidBrush(Color.Yellow),
            double n when n >= _options.DangerHighBg => new SolidBrush(Color.Red),
            double n when n <= _options.LowBg && n > _options.DangerLowBg => new SolidBrush(Color.Yellow),
            double n when n <= _options.DangerLowBg && n > _options.CriticalLowBg => new SolidBrush(Color.Red),
            double n when n <= _options.CriticalLowBg && n > 0 => new SolidBrush(Color.Red),
            _ => new SolidBrush(Color.White),
        };

        internal void CreateTextIcon(GlucoseResult fetchResult, bool isCriticalLow, NotifyIcon trayIcon)
        {
            var result = fetchResult.GetFormattedStringValue(_options.GlucoseUnit).Replace('.', '\''); // Use ' instead of . since it is narrower and allows a better display of a two digit number + decimal place.

            var isStale = fetchResult.IsStale(_options.StaleResultsThreshold);

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
            _fontToUse = new Font("Roboto", fontSize, isStale ? FontStyle.Strikeout : FontStyle.Regular, GraphicsUnit.Pixel);

            var bitmapText = new Bitmap(16, 16);
            var g = Graphics.FromImage(bitmapText);
            g.Clear(Color.Transparent);
            g.DrawString(result, _fontToUse, SetColor(_options.GlucoseUnit == GlucoseUnitType.MG ? fetchResult.MgValue : fetchResult.MmolValue), xOffset, 0f);
            var hIcon = bitmapText.GetHicon();
            var myIcon = Icon.FromHandle(hIcon);
            trayIcon.Icon = myIcon;

            DestroyMyIcon(myIcon.Handle);
            bitmapText.Dispose();
            g.Dispose();
            myIcon.Dispose();
        }

        private float CalculateXPosition(GlucoseResult result)
        {
            _useDefaultFontSize = true;
            var value = _options.GlucoseUnit == GlucoseUnitType.MG ? result.MgValue : result.MmolValue;
            if (_options.GlucoseUnit == GlucoseUnitType.MG) // Non MMOL display, use our standard offset.
                return _standardOffset;
            if (value > 9.9) // MMOL with 3 digits over 20. This requires also changing the font size from 10 to 9.
            {
                _useDefaultFontSize = false;
                return _standardOffset;
            }
            return _standardOffset; // MMOL display with only two digits, use our standard offset.
        }
    }
}