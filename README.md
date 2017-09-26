# DiscTools
Class libraries for .NET2.0 and .NET4.5.2 that allow you to interrogate PSX, Sega Saturn, PCE-CD and PC-FX disc images (based on modified code from the BizHawk team)

[![Build status](https://ci.appveyor.com/api/projects/status/58juhmjpih7mw266/branch/master?svg=true)](https://ci.appveyor.com/project/Asnivor/disctools/branch/master)

See the [Releases](https://github.com/Asnivor/DiscTools/releases) page for the latest successful auto-build release.

## Usage (C#):

* Add a reference to the DiscTools.dll
* var disc = new DiscInspector(@"\path\to\cue_or_ccd_file");

## Usage (VB):

* Add a reference to the DiscTools.dll
* Dim disc = New DiscInspector("\path\to\cue_or_ccd_file")

For safety, you should probably wrap the above in a try/catch block to handle any exceptions that may occur (the library hasnt been fully tested).

The 'disc' object can now be inspected to get any relevant data that has been discovered:

![](Images/ss-data.PNG?raw=true)

Saturn images have the most data to return. There is limited data for the other types.

## Attribution
This library uses code from BizHawk multi-system emulator - https://github.com/TASVideos/BizHawk

The .NET 2.0 version also uses the LinqBridge assembly by Atif Aziz - https://www.nuget.org/packages/LinqBridge/1.3.0