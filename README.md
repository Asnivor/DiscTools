# DiscTools
Class libraries for .NET2.0 and .NET4.5.2 that currently allow you to interrogate disc images (based on valid .cue or .ccd files) for the following systems:

* PSX
* Sega Saturn
* PC-Engine CD
* PC-FX
* SegaCD
* Philips CD-i
* NeoGeo CD (Currently Only Internal ISO Format Supported)
* Dreamcast

[![Build status](https://ci.appveyor.com/api/projects/status/58juhmjpih7mw266/branch/master?svg=true)](https://ci.appveyor.com/project/Asnivor/disctools/branch/master)

See the [Releases](https://github.com/Asnivor/DiscTools/releases) page for the latest successful auto-build release.

## Usage (C#):

* Add a reference to the DiscTools.dll

#### Quick Scan (may not detect all discs)
* var disc = DiscInspector.ScanDisc(@"\path\to\cue_or_ccd_file", false);

#### Intensive Scan (may take longer)
* var disc = DiscInspector.ScanDisc(@"\path\to\cue_or_ccd_file");

## Usage (VB):

* Add a reference to the DiscTools.dll

#### Quick Scan (may not detect all discs)
* Dim disc = DiscInspector.ScanDisc("\path\to\cue_or_ccd_file", False)

#### Intensive Scan (may take longer)
* Dim disc = DiscInspector.ScanDisc("\path\to\cue_or_ccd_file")



The 'disc' object can now be inspected to get any relevant data that has been discovered:

![](Images/ss-data.PNG?raw=true)

At the very least the library *should* be able to detect the system that the disc is for. If you are lucky it will also return lots more data.

## Attribution
This library uses code from BizHawk multi-system emulator - https://github.com/TASVideos/BizHawk

The .NET 2.0 version also uses the LinqBridge assembly by Atif Aziz - https://www.nuget.org/packages/LinqBridge/1.3.0
