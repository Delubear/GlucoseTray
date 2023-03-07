using GlucoseTray.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GlucoseTray.Extensions;
using GlucoseTray.Models;

namespace GlucoseTray.Services
{
    public class IconService
    {
        private readonly ILogger<IconService> _logger;
        private readonly IOptionsMonitor<GlucoseTraySettings> _options;
        private readonly float _standardOffset = -3f;
        private readonly int _defaultFontSize = 10;
        private readonly int _smallerFontSize = 9;
        private Font _fontToUse;
        private bool _useDefaultFontSize = true;

        public IconService(ILogger<IconService> logger, IOptionsMonitor<GlucoseTraySettings> options)
        {
            _logger = logger;
            _options = options;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        public void DestroyMyIcon(IntPtr handle) => DestroyIcon(handle);

        public Brush SetColor(double val)
        {
            if (_options.CurrentValue.IsDarkMode)
            {
                return val switch
                {
                    double n when n < _options.CurrentValue.WarningHighBg && n > _options.CurrentValue.WarningLowBg => new SolidBrush(Color.White),
                    double n when n >= _options.CurrentValue.WarningHighBg && n < _options.CurrentValue.HighBg => new SolidBrush(Color.Yellow),
                    double n when n >= _options.CurrentValue.HighBg => new SolidBrush(Color.Red),
                    double n when n <= _options.CurrentValue.WarningLowBg && n > _options.CurrentValue.LowBg => new SolidBrush(Color.Yellow),
                    double n when n <= _options.CurrentValue.LowBg && n > _options.CurrentValue.CriticalLowBg => new SolidBrush(Color.Red),
                    double n when n <= _options.CurrentValue.CriticalLowBg && n > 0 => new SolidBrush(Color.Red),
                    _ => new SolidBrush(Color.White),
                };
            }
            else
            {
                // Still having issues when using against a "light" background. Main issue appears to be when we use "DrawString" it is adding a black border/shadow here around the generated image numbers.
                // Need to investigate if it's possible to have a double-wide icon, feasibility of multiple icons, or some other method of making this clearer.
                return val switch
                {
                    double n when n < _options.CurrentValue.WarningHighBg && n > _options.CurrentValue.WarningLowBg => new SolidBrush(Color.Black),
                    double n when n >= _options.CurrentValue.WarningHighBg && n < _options.CurrentValue.HighBg => new SolidBrush(Color.DarkGoldenrod),
                    double n when n >= _options.CurrentValue.HighBg => new SolidBrush(Color.Red),
                    double n when n <= _options.CurrentValue.WarningLowBg && n > _options.CurrentValue.LowBg => new SolidBrush(Color.DarkGoldenrod),
                    double n when n <= _options.CurrentValue.LowBg && n > _options.CurrentValue.CriticalLowBg => new SolidBrush(Color.Red),
                    double n when n <= _options.CurrentValue.CriticalLowBg && n > 0 => new SolidBrush(Color.Red),
                    _ => new SolidBrush(Color.Black),
                };
            }
        }

        public void CreateTextIcon(GlucoseResult result, NotifyIcon trayIcon)
        {
            var glucoseValue = result.GetFormattedStringValue(_options.CurrentValue.GlucoseUnit).Replace('.', '\''); // Use ' instead of . since it is narrower and allows a better display of a two digit number + decimal place.

            var isStale = result.IsStale(_options.CurrentValue.StaleResultsThreshold);

            if (glucoseValue == "0")
            {
                _logger.LogWarning("Empty glucose result received.");
                glucoseValue = "NUL";
            }
            else if (result.IsCriticalLow)
            {
                _logger.LogInformation("Critical low glucose read.");
                glucoseValue = "DAN";
            }

            var xOffset = CalculateXPosition(result);
            var fontSize = _useDefaultFontSize ? _defaultFontSize : _smallerFontSize;
            _fontToUse = new Font("Roboto", fontSize, isStale ? FontStyle.Strikeout : FontStyle.Regular, GraphicsUnit.Pixel);

            var bitmapText = new Bitmap(16, 16);
            var g = Graphics.FromImage(bitmapText);
            g.Clear(Color.Transparent);
            g.DrawString(glucoseValue, _fontToUse, SetColor(_options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG ? result.MgValue : result.MmolValue), xOffset, 0f);
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
            var value = _options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG ? result.MgValue : result.MmolValue;
            if (_options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG) // Non MMOL display, use our standard offset.
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