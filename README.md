# GlucoseTray
Tray Icon for displaying current BG information in taskbar.

Update the config file with your nightscout url and run the application.  Ensure you go into your taskbar icon settings and ensure it's set to always display the GlucoseTray app.

Always verify the time of the last reading by hovering over the tray icon or clicking it.  Should it crash, the icon may stay in the taskbar but stop updating.

Always check with your DexCom reader before making any treatment decisions.


Step-by-step Instructions:
Open GlucoseTray.exe.config in your text editor and set your desired values there.  Most important is to set the NightscoutUrl value to be your base Nightscout site url.

HighBg displays yellow.
DangerousHighBg displays red.
LowBg displays yellow.
DangerousLowBg displays red.
CriticalLowBg displays as "DAN" for DANGER
Normal blood glucose displays as white.

Features:
-Color coded glucose numbers set to your ranges.
-See latest glucose reading in taskbar.  Also get time of reading and trend on hover or double-click.
-Context Menu option to open up your Nightscout url in browser.
