using System.Drawing.Imaging;

namespace GlucoseTray.Display;

public interface IGlucoseIconRenderer
{
    Bitmap Render(string text, Color color, bool isStale, int size);
}

/// <summary>
/// Draws glucose text with hand-made pixel glyphs instead of a font, so every pixel lands exactly
/// on the tray icon grid and stays crisp at any size.
/// </summary>
public class GlucoseIconRenderer : IGlucoseIconRenderer
{
    private const int GlyphHeight = 7;
    private const int StaleAlpha = 140;

    private static readonly Dictionary<char, string[]> Glyphs = new()
    {
        ['0'] = [".##.", "#..#", "#..#", "#..#", "#..#", "#..#", ".##."],
        ['1'] = [".#.", "##.", ".#.", ".#.", ".#.", ".#.", "###"],
        ['2'] = [".##.", "#..#", "...#", "..#.", ".#..", "#...", "####"],
        ['3'] = ["###.", "...#", "...#", ".##.", "...#", "...#", "###."],
        ['4'] = ["#..#", "#..#", "#..#", "####", "...#", "...#", "...#"],
        ['5'] = ["####", "#...", "###.", "...#", "...#", "#..#", ".##."],
        ['6'] = [".##.", "#...", "#...", "###.", "#..#", "#..#", ".##."],
        ['7'] = ["####", "...#", "..#.", "..#.", ".#..", ".#..", ".#.."],
        ['8'] = [".##.", "#..#", "#..#", ".##.", "#..#", "#..#", ".##."],
        ['9'] = [".##.", "#..#", "#..#", ".###", "...#", "...#", ".##."],
        ['.'] = [".", ".", ".", ".", ".", ".", "#"],
        ['D'] = ["###.", "#..#", "#..#", "#..#", "#..#", "#..#", "###."],
        ['A'] = [".##.", "#..#", "#..#", "####", "#..#", "#..#", "#..#"],
        ['N'] = ["#..#", "##.#", "##.#", "#.##", "#.##", "#..#", "#..#"],
        ['U'] = ["#..#", "#..#", "#..#", "#..#", "#..#", "#..#", ".##."],
        ['L'] = ["#...", "#...", "#...", "#...", "#...", "#...", "####"],
    };

    public Bitmap Render(string text, Color color, bool isStale, int size)
    {
        var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        var glyphs = text.Where(Glyphs.ContainsKey).Select(c => Glyphs[c]).ToList();
        if (glyphs.Count == 0)
            return bitmap;

        var gap = Math.Max(1, size / 16);
        var textHeight = isStale ? size - gap * 2 : size;
        var inkColor = isStale ? Color.FromArgb(StaleAlpha, color) : color;

        var baseWidth = glyphs.Sum(g => g[0].Length);
        var availableWidth = size - gap * (glyphs.Count - 1);
        var scale = (double)availableWidth / baseWidth;

        var cumulativeBase = 0;
        var x = 0;
        foreach (var glyph in glyphs)
        {
            var glyphBaseWidth = glyph[0].Length;
            var start = (int)Math.Round(cumulativeBase * scale);
            cumulativeBase += glyphBaseWidth;
            var width = Math.Max(1, (int)Math.Round(cumulativeBase * scale) - start);

            DrawGlyph(bitmap, glyph, x, width, textHeight, inkColor);
            x += width + gap;
        }

        if (isStale)
            DrawStaleMarker(bitmap, size, gap, color);

        return bitmap;
    }

    private static void DrawGlyph(Bitmap bitmap, string[] glyph, int left, int width, int height, Color color)
    {
        var glyphWidth = glyph[0].Length;
        for (var y = 0; y < height; y++)
        {
            var row = glyph[y * GlyphHeight / height];
            for (var dx = 0; dx < width; dx++)
            {
                var px = left + dx;
                if (px < bitmap.Width && row[dx * glyphWidth / width] == '#')
                    bitmap.SetPixel(px, y, color);
            }
        }
    }

    private static void DrawStaleMarker(Bitmap bitmap, int size, int thickness, Color color)
    {
        for (var y = size - thickness; y < size; y++)
            for (var x = 0; x < size; x += 2)
                bitmap.SetPixel(x, y, color);
    }
}
