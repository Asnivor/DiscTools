# DiscTools
Class libraries for .NET2.0 and .NET4.5.2 that allow you to interrogate a PSX disc image (based on code from the BizHawk team)

[![Build status](https://ci.appveyor.com/api/projects/status/58juhmjpih7mw266/branch/master?svg=true)](https://ci.appveyor.com/project/Asnivor/disctools/branch/master)

See the [Releases](https://github.com/Asnivor/DiscTools/releases) page for the latest successful auto-build release.

## Usage (C#):

* Add a reference to the DiscTools.dll
* string serial = DiscTools.SerialNumber.GetPSXSerial(@"\path\to\cue_or_ccd_file");

For safety, you should probably wrap the above in a try/catch block to handle any exceptions that may occur (the library hasnt been fully tested).

## Usage (VB):

* Add a reference to the DiscTools.dll
* Dim serial As String = DiscTools.SerialNumber.GetPSXSerial("\path\to\cue_or_ccd_file")

For safety, you should probably wrap the above in a try/catch block to handle any exceptions that may occur (the library hasnt been fully tested).

## Attribution
This library uses code from BizHawk multi-system emulator - https://github.com/TASVideos/BizHawk

The .NET 2.0 version also uses the LinqBridge assembly by Atif Aziz - https://www.nuget.org/packages/LinqBridge/1.3.0
