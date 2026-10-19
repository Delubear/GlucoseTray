using GlucoseTray.Display;
using System.Drawing;

namespace GlucoseTray.Tests;

public class ImageGlucoseIconRendererTests
{
	private static readonly GlucoseTextRasterizer Rasterizer = new();
	private readonly ImageGlucoseIconRenderer _renderer = new(Rasterizer);

	[TestCase("123", 16)]
	[TestCase("12.5", 20)]
	[TestCase("DAN", 24)]
	[TestCase("NUL", 32)]
	[TestCase("5.5", 16)]
	[TestCase("144", 28)]
	[TestCase("999", 48)]
	public void ShouldRenderReadingsInTheRequestedColor(string text, int size)
	{
		using var bitmap = _renderer.Render(text, Color.Red, false, size);

		var ink = Pixels(bitmap).Where(p => p.A > 0).ToList();
		Assert.Multiple(() =>
		{
			Assert.That(bitmap.Size, Is.EqualTo(new Size(size, size)));
			Assert.That(ink, Is.Not.Empty);
			Assert.That(ink.All(p => p.R == 255 && p.G == 0 && p.B == 0), Is.True);
		});
	}

	[Test]
	public void ShouldDimStaleReadings()
	{
		using var bitmap = _renderer.Render("123", Color.White, true, 16);

		var digitAlphas = Pixels(bitmap, maxY: 14).Select(p => p.A);
		Assert.That(digitAlphas.Max(), Is.LessThan(255));
	}

	[Test]
	public void ShouldNameOverridesWithUnderscoreForTheDecimalPoint() =>
		Assert.That(ImageGlucoseIconRenderer.OverrideFileName("12.7", 16), Is.EqualTo("16_12_7.png"));

	private static IEnumerable<Color> Pixels(Bitmap bitmap, int? maxY = null) =>
		Enumerable.Range(0, bitmap.Width).SelectMany(x => Enumerable.Range(0, maxY ?? bitmap.Height).Select(y => bitmap.GetPixel(x, y)));
}
