using System.Collections.Concurrent;
using System.Drawing.Imaging;

namespace GlucoseTray.Display;

/// <summary>
/// Draws readings on the fly, but prefers a hand-tuned override image (GlucoseTray/Icons/{size}_{text}.png)
/// when one exists for the exact size, so individual values that look poor can be fixed by hand.
/// Images are white on transparent and tinted to the reading's color.
/// </summary>
public class ImageGlucoseIconRenderer(GlucoseTextRasterizer rasterizer) : IGlucoseIconRenderer
{
	private const string ResourcePrefix = "GlucoseIcons.";
	private const int StaleAlpha = 140;

	private readonly ConcurrentDictionary<string, Bitmap?> _overrides = new();

	public Bitmap Render(string text, Color color, bool isStale, int size)
	{
		var bitmap = LoadOverride(text, size) is { } image
			? new Bitmap(image)
			: rasterizer.Render(text, size);

		Tint(bitmap, color, isStale ? StaleAlpha : 255);
		if (isStale)
			DrawStaleMarker(bitmap, color);

		return bitmap;
	}

	public static string OverrideFileName(string text, int size) => $"{size}_{text.Replace('\'', '_').Replace('.', '_')}.png";

	private Bitmap? LoadOverride(string text, int size)
	{
		var name = ResourcePrefix + Path.GetFileNameWithoutExtension(OverrideFileName(text, size));
		var image = _overrides.GetOrAdd(name, static n =>
		{
			using var stream = typeof(ImageGlucoseIconRenderer).Assembly.GetManifestResourceStream(n);
			return stream is null ? null : new Bitmap(stream);
		});
		return image?.Size == new Size(size, size) ? image : null;
	}

	private static void Tint(Bitmap bitmap, Color color, int maxAlpha)
	{
		for (var x = 0; x < bitmap.Width; x++)
			for (var y = 0; y < bitmap.Height; y++)
			{
				var alpha = bitmap.GetPixel(x, y).A * maxAlpha / 255;
				bitmap.SetPixel(x, y, Color.FromArgb(alpha, color));
			}
	}

	private static void DrawStaleMarker(Bitmap bitmap, Color color)
	{
		var y = bitmap.Height - 1;
		for (var x = 0; x < bitmap.Width; x += 2)
			bitmap.SetPixel(x, y, color);
	}
}
