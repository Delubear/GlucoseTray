# GlucoseTray

Tray icon for displaying your current blood glucose (BG) information in the Windows taskbar.

Always verify the time of the last reading by hovering over the tray icon or double-clicking it. If the app crashes, the icon may stay in the taskbar but stop updating.

> **Always check with your Dexcom reader before making any treatment decisions.**

## Getting Started

1. Download and run one of the following:
   - **GlucoseTray.exe** — no .NET install required.
   - **GlucoseTray-Slim.exe** — requires the appropriate .NET runtime installed.
2. On first run, the program creates an `appsettings.json` file alongside the `.exe`.
3. Right-click the new taskbar icon and click **Settings**, or open `appsettings.json` in a text editor.
4. Configure the program settings and see the updates immediately on save.

Your Dexcom password and Nightscout token are automatically encrypted at rest in `appsettings.json` (Windows DPAPI, current-user scope). If you edit a credential while the app is running, it is re-encrypted on save.

## Color Coding

| Reading | Display |
| --- | --- |
| Critically high glucose | Red |
| High glucose | Yellow |
| Low glucose | Yellow |
| Critically low glucose | `DAN` (for DANGER), red |
| Normal glucose | Black (white in dark mode) |
| Out-of-date reading | Strikethrough effect |

## Features

- Color-coded glucose numbers set to your ranges.
- See the latest glucose reading in the taskbar, plus the reading time and trend on hover or double-click.
- Option to start the application on system startup.

![GlucoseTray taskbar icon](https://raw.githubusercontent.com/Delubear/GlucoseTray/master/2019-05-03_16-18-24.png)

## Setup Guide

See the [Setup Guide wiki](https://github.com/Delubear/GlucoseTray/wiki/Setup-Guide) for detailed configuration instructions.
