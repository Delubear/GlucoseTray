# Hand-tuned tray icon overrides

Tray icons are drawn automatically. If one reading looks bad at one size, you can replace just that image:

1. Export the auto-drawn version as a starting point (writes into this folder):
   `$env:GLUCOSETRAY_EXPORT_ICON="12'7@16"; dotnet test GlucoseTray.Tests --filter "Name=ExportIconForEditing"`
   (`@` is followed by the pixel size: 16, 20, 24, 28, 32, 40 or 48. Separate several with `;`.)
2. Edit the PNG pixel by pixel. Keep it **white on transparent** and the same size; the app colors it.
3. Rebuild. Any `{size}_{reading}.png` here (decimal point written as `_`, e.g. `16_12_7.png`) is used instead of the drawn one.

Delete the file to go back to the automatic version.
