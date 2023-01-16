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
        readonly ILogger<IconService> _logger;
        readonly IOptionsMonitor<GlucoseTraySettings> _options;
        readonly float _standardOffset = -3f;
        readonly int _defaultFontSize = 10;
        readonly int _smallerFontSize = 9;
        Font _fontToUse;
        bool _useDefaultFontSize = true;

        public IconService(ILogger<IconService> logger, IOptionsMonitor<GlucoseTraySettings> options)
        {
            _logger = logger;
            _options = options;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

        public void DestroyMyIcon(IntPtr handle) => DestroyIcon(handle);

        public Brush SetColor(double val) => val switch
        {
            double n when n < _options.CurrentValue.WarningHighBg && n > _options.CurrentValue.WarningLowBg => new SolidBrush(Color.White),
            double n when n >= _options.CurrentValue.WarningHighBg && n < _options.CurrentValue.HighBg => new SolidBrush(Color.Yellow),
            double n when n >= _options.CurrentValue.HighBg => new SolidBrush(Color.Red),
            double n when n <= _options.CurrentValue.WarningLowBg && n > _options.CurrentValue.LowBg => new SolidBrush(Color.Yellow),
            double n when n <= _options.CurrentValue.LowBg && n > _options.CurrentValue.CriticalLowBg => new SolidBrush(Color.Red),
            double n when n <= _options.CurrentValue.CriticalLowBg && n > 0 => new SolidBrush(Color.Red),
            _ => new SolidBrush(Color.White),
        };

        string glucoseValue = string.Empty;
        bool isStale = false;
        public void CreateTextIcon(GlucoseResult result, NotifyIcon trayIcon)
        {
            glucoseValue = result.GetFormattedStringValue(_options.CurrentValue.GlucoseUnit).Replace('.', '\''); // Use ' instead of . since it is narrower and allows a better display of a two digit number + decimal place.
            isStale = result.IsStale(_options.CurrentValue.StaleResultsThreshold);

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

            float xOffset = CalculateXPosition(result);
            int fontSize = _useDefaultFontSize ? _defaultFontSize : _smallerFontSize;
            _fontToUse = new Font("Roboto", fontSize, isStale ? FontStyle.Strikeout : FontStyle.Regular, GraphicsUnit.Pixel);

            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(bitmapText);
            g.Clear(Color.Transparent);
            g.DrawString(glucoseValue, _fontToUse, SetColor(_options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG ? result.MgValue : result.MmolValue), xOffset, 0f);
            IntPtr hIcon = bitmapText.GetHicon();
            Icon myIcon = Icon.FromHandle(hIcon);
            trayIcon.Icon = myIcon;

            DestroyMyIcon(myIcon.Handle);
            bitmapText.Dispose();
            g.Dispose();
            myIcon.Dispose();
        }

        float CalculateXPosition(GlucoseResult result)
        {
            _useDefaultFontSize = true;
            double value = _options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG ? result.MgValue : result.MmolValue;

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