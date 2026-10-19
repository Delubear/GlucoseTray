using GlucoseTray.Display;
using System.Drawing;

namespace GlucoseTray.Tests;

public class GlucoseIconRendererTests
{
    private readonly GlucoseIconRenderer _renderer = new();

    [TestCase("55", 16)]
    [TestCase("123", 16)]
    [TestCase("12'5", 16)]
    [TestCase("DAN", 16)]
    [TestCase("NUL", 24)]
    [TestCase("300", 32)]
    public void ShouldFillTheIconWidthAndHeight(string text, int size)
    {
        using var bitmap = _renderer.Render(text, Color.White, false, size);

        var (minX, maxX, minY, maxY) = InkBounds(bitmap);
        Assert.Multiple(() =>
        {
            Assert.That(maxX - minX + 1, Is.GreaterThanOrEqualTo(size - 1));
            Assert.That(maxY - minY + 1, Is.EqualTo(size));
        });
    }

    [Test]
    public void ShouldOnlyUseFullyOpaquePixelsForFreshReadings()
    {
        using var bitmap = _renderer.Render("123", Color.Red, false, 16);

        var alphas = AllPixels(bitmap).Select(p => p.A).Distinct();
        Assert.That(alphas, Is.SubsetOf(new byte[] { 0, 255 }));
    }

    [Test]
    public void ShouldDrawStaleMarkerAlongTheBottom()
    {
        using var bitmap = _renderer.Render("123", Color.White, true, 16);

        Assert.That(bitmap.GetPixel(0, 15).A, Is.EqualTo(255));
    }

    [Test]
    public void ShouldDimStaleDigits()
    {
        using var bitmap = _renderer.Render("123", Color.White, true, 16);

        var digitAlphas = Enumerable.Range(0, 16).SelectMany(x => Enumerable.Range(0, 14).Select(y => bitmap.GetPixel(x, y).A)).Where(a => a > 0);
        Assert.That(digitAlphas, Is.All.LessThan(255));
    }

    private static IEnumerable<Color> AllPixels(Bitmap bitmap) =>
        Enumerable.Range(0, bitmap.Width).SelectMany(x => Enumerable.Range(0, bitmap.Height).Select(y => bitmap.GetPixel(x, y)));

    private static (int MinX, int MaxX, int MinY, int MaxY) InkBounds(Bitmap bitmap)
    {
        var ink = Enumerable.Range(0, bitmap.Width)
            .SelectMany(x => Enumerable.Range(0, bitmap.Height).Select(y => (x, y)))
            .Where(p => bitmap.GetPixel(p.x, p.y).A > 0)
            .ToList();
        return (ink.Min(p => p.x), ink.Max(p => p.x), ink.Min(p => p.y), ink.Max(p => p.y));
    }
}
