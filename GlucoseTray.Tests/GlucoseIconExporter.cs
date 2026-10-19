using GlucoseTray.Display;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace GlucoseTray.Tests;

/// <summary>
/// Exports auto-drawn icons into GlucoseTray/Icons so they can be hand-edited as overrides.
/// Set GLUCOSETRAY_EXPORT_ICON to e.g. "12'7@16;144@20".
/// </summary>
[Explicit("Writes override images into GlucoseTray/Icons. Run manually.")]
public class GlucoseIconExporter
{
    [Test]
    public void ExportIconForEditing()
    {
        var request = Environment.GetEnvironmentVariable("GLUCOSETRAY_EXPORT_ICON");
        if (string.IsNullOrWhiteSpace(request))
            Assert.Ignore("Set GLUCOSETRAY_EXPORT_ICON, e.g. \"12'7@16\".");

        var outputDir = Path.GetFullPath(Path.Combine(RepoRoot(), "GlucoseTray", "Icons"));
        using var rasterizer = new GlucoseTextRasterizer();
        foreach (var item in request!.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var (text, size) = item.Split('@') is [var t, var s] ? (t, int.Parse(s)) : throw new FormatException($"Expected text@size, got '{item}'.");
            using var bitmap = rasterizer.Render(text, size);
            var path = Path.Combine(outputDir, ImageGlucoseIconRenderer.OverrideFileName(text, size));
            bitmap.Save(path, ImageFormat.Png);
            TestContext.Out.WriteLine($"Exported {path}");
        }
    }

    /// <summary>
    /// Regenerates every pre-saved 16x16 icon. Overwrites hand edits, so re-apply those afterwards.
    /// </summary>
    [Test]
    public void GeneratePreSavedIcons()
    {
        if (Environment.GetEnvironmentVariable("GLUCOSETRAY_GENERATE_ICONS") != "1")
            Assert.Ignore("Set GLUCOSETRAY_GENERATE_ICONS=1 to regenerate GlucoseTray/Icons.");

        const int size = 16;
        var outputDir = Path.GetFullPath(Path.Combine(RepoRoot(), "GlucoseTray", "Icons"));
        foreach (var old in Directory.GetFiles(outputDir, "*.png"))
            File.Delete(old);

        using var rasterizer = new GlucoseTextRasterizer();
        foreach (var text in Readings())
        {
            using var bitmap = rasterizer.Render(text, size);
            bitmap.Save(Path.Combine(outputDir, ImageGlucoseIconRenderer.OverrideFileName(text, size)), ImageFormat.Png);
        }
    }

    private static IEnumerable<string> Readings()
    {
        yield return "NUL";
        yield return "DAN";
        for (var mg = 1; mg <= 600; mg++)
            yield return mg.ToString();
        for (var tenths = 10; tenths <= 333; tenths++)
            yield return $"{tenths / 10}'{tenths % 10}";
    }

    private static string RepoRoot([CallerFilePath] string thisFile = "") =>
        Path.Combine(Path.GetDirectoryName(thisFile)!, "..");
}
