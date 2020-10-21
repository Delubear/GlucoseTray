# GlucoseTray
Tray Icon for displaying current BG information in taskbar.

Featured at: https://www.deebee.it/?p=18480&lang=en

Update the config file with your Dexcom credentials and run the application.  Ensure you go into your taskbar icon settings and ensure it's set to always display the GlucoseTray app.

Always verify the time of the last reading by hovering over the tray icon or clicking it.  Should it crash, the icon may stay in the taskbar but stop updating.

Always check with your DexCom reader before making any treatment decisions.


<strong>Step-by-step Instructions:</strong> <br>
Open appsettings.json in your text editor and set your desired values there.  Most important settings are to configure if you are using Nightscout or Dexcom as data source and to enter your site url (NightScout) or credentials (Dexcom).

HighBg displays yellow. <br>
DangerousHighBg displays red. <br>
LowBg displays yellow. <br>
DangerousLowBg displays red. <br>
CriticalLowBg displays as "DAN" for DANGER. <br>
Normal blood glucose displays as white. <br>
Out-of-date readings are shown with a strikethrough effect. <br>

<strong>Features:</strong> <br>
-Color coded glucose numbers set to your ranges. <br>
-See latest glucose reading in taskbar.  Also get time of reading and trend on hover or double-click. <br>
-Context Menu option to open up your Nightscout url in browser. <br>
-Option to start application on system startup. <br>

![alt text](https://raw.githubusercontent.com/Delubear/GlucoseTray/master/2019-05-03_16-18-24.png)


If missing configuration file, get a new copy at https://github.com/Delubear/GlucoseTray
