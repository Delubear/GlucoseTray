﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GlucoseTray.Services;

public class IconService
{
    private readonly ILogger<IconService> _logger;
    private readonly IOptionsMonitor<GlucoseTraySettings> _options;
    private readonly float _standardOffset = -10f;
    private readonly int _defaultFontSize = 40;
    private readonly int _smallerFontSize = 38;

    public IconService(ILogger<IconService> logger, IOptionsMonitor<GlucoseTraySettings> options)
    {
        _logger = logger;
        _options = options;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool DestroyIcon(IntPtr handle);

    public static void DestroyMyIcon(IntPtr handle) => DestroyIcon(handle);

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

        var fontSize = CalculateFontSize(result);
        var font = new Font("Roboto", fontSize, isStale ? FontStyle.Strikeout : FontStyle.Regular, GraphicsUnit.Pixel);

        var bitmapText = new Bitmap(64, 64);
        var g = Graphics.FromImage(bitmapText);
        g.Clear(Color.Transparent);
        g.DrawString(glucoseValue, font, SetColor(_options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG ? result.MgValue : result.MmolValue), _standardOffset, 0f);
        var hIcon = bitmapText.GetHicon();
        var myIcon = Icon.FromHandle(hIcon);
        trayIcon.Icon = myIcon;

        DestroyMyIcon(myIcon.Handle);
        bitmapText.Dispose();
        g.Dispose();
        myIcon.Dispose();
    }

    private int CalculateFontSize(GlucoseResult result)
    {
        var value = _options.CurrentValue.GlucoseUnit == GlucoseUnitType.MG ? result.MgValue : result.MmolValue;
        if (_options.CurrentValue.GlucoseUnit == GlucoseUnitType.MMOL && value > 9.9) // Need to use smaller font size to accommodate 3 numbers + a decimal point
            return _smallerFontSize;
        return _defaultFontSize;
    }
}