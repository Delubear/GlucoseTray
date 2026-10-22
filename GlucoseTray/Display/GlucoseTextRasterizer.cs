using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace GlucoseTray.Display;

/// <summary>
/// Draws a reading as white-on-transparent at an exact pixel size, tuned for legibility at tray sizes.
/// </summary>
public sealed class GlucoseTextRasterizer : IDisposable
{
    private static readonly string[] FontCandidates = ["Bahnschrift SemiBold Condensed", "Bahnschrift", "Segoe UI"];
    private const float MaxStretch = 1.6f;
    private const float EmSize = 100f;
    private const float CharGap = 8f;

    private readonly FontFamily _family;
    private readonly FontStyle _style;
    private readonly RectangleF _digitBounds;
    private readonly Lock _gate = new();

    public GlucoseTextRasterizer()
    {
        using var installed = new InstalledFontCollection();
        var names = installed.Families.Select(f => f.Name).ToHashSet();
        var fontName = FontCandidates.FirstOrDefault(names.Contains);
        _family = fontName is null ? new FontFamily(GenericFontFamilies.SansSerif) : new FontFamily(fontName);
        _style = fontName?.Contains("SemiBold") == true ? FontStyle.Regular : FontStyle.Bold;
        using var zero = FontChar('0');
        _digitBounds = zero.GetBounds();
    }

    /// <param name="text">Display text; an apostrophe is drawn as a decimal point.</param>
    public Bitmap Render(string text, int size)
    {
        var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        lock (_gate)
        {
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var dot = text.IndexOf('.');
            if (dot < 0)
            {
                if (!TryDrawEqualBars(g, text, new Rectangle(0, 0, size, size)))
                {
                    using var path = BuildText(text);
                    DrawFitted(g, path, new RectangleF(0, 0, size, size), centered: true);
                }
                return bitmap;
            }

            // Decimals: the whole number stays full height, and the tenths digit is drawn small and
            // squarish in the top-right corner, overhanging a solid pixel-aligned dot like a superscript.
            // This leaves the whole number far more width than splitting the icon into equal-height halves.
            using var whole = BuildText(text[..dot]);
            using var fraction = BuildText(text[(dot + 1)..]);
            var dotSize = Math.Max(2, size / 8);
            var fractionHeight = (int)Math.Round(size * 0.55);
            var fractionWidth = (int)Math.Round(size * 0.3);
            var fractionLeft = size - fractionWidth;
            var dotLeft = fractionLeft; // the dot tucks under the tenths digit so the whole number keeps the width
            var wholeWidth = fractionLeft - 1;

            if (!TryDrawEqualBars(g, text[..dot], new Rectangle(0, 0, wholeWidth, size)))
                DrawFitted(g, whole, new RectangleF(0, 0, wholeWidth, size), centered: false);
            g.FillRectangle(Brushes.White, dotLeft, size - dotSize, dotSize, dotSize);
            if (!TryDrawEqualBars(g, text[(dot + 1)..], new Rectangle(fractionLeft, 0, fractionWidth, fractionHeight)))
                DrawFitted(g, fraction, new RectangleF(fractionLeft, 0, fractionWidth, fractionHeight), centered: false);
        }
        return bitmap;
    }

    public void Dispose() => _family.Dispose();

    private static void DrawFitted(Graphics g, GraphicsPath path, RectangleF box, bool centered)
    {
        var bounds = path.GetBounds();
        if (bounds.Width <= 0 || bounds.Height <= 0 || box.Width <= 0)
            return;
        var scaleY = box.Height / bounds.Height;
        var scaleX = centered ? Math.Min(box.Width / bounds.Width, scaleY * MaxStretch) : box.Width / bounds.Width;
        var offsetX = box.X + (box.Width - bounds.Width * scaleX) / 2;

        using var matrix = new Matrix();
        matrix.Translate(-bounds.X, -bounds.Y);
        matrix.Scale(scaleX, scaleY, MatrixOrder.Append);
        matrix.Translate(offsetX, box.Y, MatrixOrder.Append);
        path.Transform(matrix);
        // Snap edges to whole pixels so straight bars (like a run of 1s) render without soft columns.
        var points = path.PathPoints.Select(p => new PointF(MathF.Round(p.X), MathF.Round(p.Y))).ToArray();
        using var snapped = new GraphicsPath(points, path.PathTypes, FillMode.Winding);
        g.FillPath(Brushes.White, snapped);
    }

    // Runs of 1s are laid out directly in whole pixels so every bar gets the same width.
    private static bool TryDrawEqualBars(Graphics g, string text, Rectangle box)
    {
        if (text.Length == 0 || text.Any(c => c != '1')) return false;
        var count = text.Length;
        var gap = count > 1 ? Math.Max(1, box.Width / 8) : 0;
        var barWidth = Math.Min(Math.Max(1, box.Height / 4), (box.Width - gap * (count - 1)) / count);
        if (barWidth < 1) return false;
        var left = box.X + (box.Width - (barWidth * count + gap * (count - 1))) / 2;
        for (var i = 0; i < count; i++)
            g.FillRectangle(Brushes.White, left + i * (barWidth + gap), box.Y, barWidth, box.Height);
        return true;
    }

    private GraphicsPath BuildText(string text)
    {
        var result = new GraphicsPath(FillMode.Winding);
        var cursor = 0f;
        foreach (var c in text)
        {
            using var charPath = c switch { '4' => SquareFour(), '1' => BarOne(), _ => FontChar(c) };
            var b = charPath.GetBounds();
            if (b.Height <= 0)
                continue;
            using var m = new Matrix();
            // Normalize every glyph to the font's digit height so the custom 4 lines up with font glyphs.
            var s = _digitBounds.Height / b.Height;
            m.Translate(-b.X, -b.Y);
            m.Scale(s, s, MatrixOrder.Append);
            m.Translate(cursor, _digitBounds.Y, MatrixOrder.Append);
            charPath.Transform(m);
            cursor = charPath.GetBounds().Right + CharGap;
            result.AddPath(charPath, false);
        }
        return result;
    }

    private GraphicsPath FontChar(char c)
    {
        // Some fonts build glyphs from overlapping contours; Winding fill keeps the overlaps solid.
        var p = new GraphicsPath(FillMode.Winding);
        p.AddString(c.ToString(), _family, (int)_style, EmSize, PointF.Empty, StringFormat.GenericTypographic);
        return p;
    }

    // A diagonal 4 smears into noise at tray size, so use an open-top 4 built only from straight bars.
    private GraphicsPath SquareFour()
    {
        var w = _digitBounds.Width;
        var h = _digitBounds.Height;
        var t = w * 0.3f;
        var barBottom = h * 0.66f;
        var p = new GraphicsPath(FillMode.Winding);
        p.AddRectangle(new RectangleF(0, 0, t, barBottom));
        p.AddRectangle(new RectangleF(0, barBottom - t, w, t));
        p.AddRectangle(new RectangleF(w - t, 0, t, h));
        return p;
    }

    // A plain bar 1 saves the columns the font spends on the flag.
    private GraphicsPath BarOne()
    {
        var p = new GraphicsPath(FillMode.Winding);
        p.AddRectangle(new RectangleF(0, 0, _digitBounds.Width * 0.3f, _digitBounds.Height));
        return p;
    }
}
