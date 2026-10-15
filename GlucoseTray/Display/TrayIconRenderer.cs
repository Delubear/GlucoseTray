using GlucoseTray.Enums;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace GlucoseTray.Display;

/// <summary>Fits the glyph outlines to the native tray size rather than shrinking a padded 64px label.</summary>
public static class TrayIconRenderer
{
    public static Bitmap Render(GlucoseDisplay display, int size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 16);
        var bitmap = new Bitmap(size, size);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        if (string.IsNullOrEmpty(display.DisplayValue)) return bitmap;

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var glyphs = new GraphicsPath();
        using var family = new FontFamily("Arial");
        using var format = (StringFormat)StringFormat.GenericTypographic.Clone();
        glyphs.AddString(display.DisplayValue, family, (int)FontStyle.Bold, 32, PointF.Empty, format);
        var bounds = glyphs.GetBounds();
        if (bounds.Width <= 0 || bounds.Height <= 0) return bitmap;

        // Use nearly all the slot, with an explicit one-pixel border to avoid clipped antialiasing.
        var inset = size / 16f;
        var width = size - 2 * inset;
        var height = size - 2 * inset;
        // Avoid stretching short values too wide; longer values are condensed to fit.
        var scaleY = height / bounds.Height;
        var scaleX = Math.Min(scaleY, width / bounds.Width);
        var left = (size - bounds.Width * scaleX) / 2;
        using var transform = new Matrix(scaleX, 0, 0, scaleY,
            left - bounds.Left * scaleX, inset - bounds.Top * scaleY);
        glyphs.Transform(transform);
        var unavailable = display.TimestampUtc == DateTime.MinValue || display.IsStale;
        using var brush = new SolidBrush(unavailable ? Color.FromArgb(164, 174, 190) : ToColor(display.Color));
        // A slight expansion keeps condensed strokes visible at 16px without ClearType fringes.
        using var stroke = new Pen(brush, size / 40f) { LineJoin = LineJoin.Round };
        graphics.DrawPath(stroke, glyphs);
        graphics.FillPath(brush, glyphs);
        if (display.IsStale)
        {
            using var pen = new Pen(brush, Math.Max(1, size / 16f));
            graphics.DrawLine(pen, inset, size / 2f, size - inset, size / 2f);
        }
        return bitmap;
    }

    public static Icon CreateIcon(GlucoseDisplay display, int size)
    {
        using var bitmap = Render(display, size);
        var handle = bitmap.GetHicon();
        try
        {
            using var borrowed = Icon.FromHandle(handle);
            return (Icon)borrowed.Clone();
        }
        finally { DestroyIcon(handle); }
    }

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(nint handle);

    private static Color ToColor(IconTextColor color) => color switch
    {
        IconTextColor.White => Color.White,
        IconTextColor.Black => Color.Black,
        IconTextColor.Green => Color.FromArgb(91, 223, 171),
        IconTextColor.DarkGreen => Color.SeaGreen,
        IconTextColor.Yellow => Color.FromArgb(255, 207, 86),
        IconTextColor.Gold => Color.DarkGoldenrod,
        IconTextColor.Red => Color.FromArgb(255, 106, 117),
        _ => Color.Black,
    };
}
