using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GlucoseTray.Display;
using GlucoseTray.Enums;

namespace GlucoseTray.Tests;

[Apartment(ApartmentState.STA)]
[NonParallelizable]
public class NativeDisplayTests
{
    [TestCase(IconTextColor.Green, false, false, 91, 223, 171)]
    [TestCase(IconTextColor.DarkGreen, false, false, 46, 139, 87)]
    [TestCase(IconTextColor.Yellow, false, false, 255, 207, 86)]
    [TestCase(IconTextColor.Red, false, false, 255, 106, 117)]
    [TestCase(IconTextColor.Green, true, false, 164, 174, 190)]
    public void NativeIconShouldRetainItsStatusColorAfterSourceBitmapDisposal(IconTextColor color,
        bool stale, bool offline, int red, int green, int blue)
    {
        using var icon = TrayIconRenderer.CreateIcon(new GlucoseDisplay
        {
            DisplayValue = "8.1", Color = color, IsStale = stale,
            TimestampUtc = DateTime.UtcNow,
        }, 16);
        using var bitmap = icon.ToBitmap();
        Assert.That(bitmap.Size, Is.EqualTo(new System.Drawing.Size(16, 16)));
        var coloredPixels = 0;
        for (var y = 0; y < bitmap.Height; y++)
        for (var x = 0; x < bitmap.Width; x++)
        {
            var pixel = bitmap.GetPixel(x, y);
            if (pixel.A > 200 && Math.Abs(pixel.R - red) <= 2 &&
                Math.Abs(pixel.G - green) <= 2 && Math.Abs(pixel.B - blue) <= 2) coloredPixels++;
        }
        Assert.That(coloredPixels, Is.GreaterThan(5), "The real HICON must retain the status color.");
    }

    [Test]
    public void RepeatedIconRefreshShouldNotLeakNativeHandles()
    {
        var previous = SynchronizationContext.Current;
        try
        {
            using var icon = new NotificationIcon();
            icon.RefreshIcon(Reading());
            using var process = Process.GetCurrentProcess();
            var gdiBefore = GetGuiResources(process.Handle, 0);
            var userBefore = GetGuiResources(process.Handle, 1);
            for (var i = 0; i < 250; i++) icon.RefreshIcon(Reading());
            Assert.That(GetGuiResources(process.Handle, 0), Is.LessThanOrEqualTo(gdiBefore + 5));
            Assert.That(GetGuiResources(process.Handle, 1), Is.LessThanOrEqualTo(userBefore + 5));
        }
        finally { SynchronizationContext.SetSynchronizationContext(previous); }
    }

    [Test]
    public void BackgroundRefreshShouldReachTheRealUiThread()
    {
        var previous = SynchronizationContext.Current;
        try
        {
            using var icon = new NotificationIcon();
            Task.Run(() => icon.RefreshIcon(Reading())).GetAwaiter().GetResult();
            var native = (NotifyIcon)typeof(NotificationIcon).GetField("_trayIcon", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(icon)!;
            var deadline = Stopwatch.StartNew();
            while (native.Icon is null && deadline.Elapsed < TimeSpan.FromSeconds(3)) Application.DoEvents();
            Assert.That(native.Icon, Is.Not.Null);
            Assert.That(native.Text, Does.Contain("8.1"));
            using var bitmap = native.Icon!.ToBitmap();
            Assert.That(bitmap.Width, Is.GreaterThanOrEqualTo(16));
        }
        finally { SynchronizationContext.SetSynchronizationContext(previous); }
    }

    private static GlucoseDisplay Reading() => new()
    {
        DisplayValue = "8.1", Color = IconTextColor.Green, TimestampUtc = DateTime.UtcNow,
    };

    [DllImport("user32.dll")]
    private static extern uint GetGuiResources(nint process, uint flags);
}
