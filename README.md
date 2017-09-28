# DiscTools
Class libraries for .NET2.0 and .NET4.5.2 that currently allow you to interrogate disc images (based on valid .cue or .ccd files) for the following systems:

* PSX
* Sega Saturn
* PC-Engine CD
* PC-FX
* SegaCD
* Philips CD-i
* NeoGeo CD
* Dreamcast

[![Build status](https://ci.appveyor.com/api/projects/status/58juhmjpih7mw266/branch/master?svg=true)](https://ci.appveyor.com/project/Asnivor/disctools/branch/master)

See the [Releases](https://github.com/Asnivor/DiscTools/releases) page for the latest successful auto-build release.

## Limitations
* DiscTools will only accept a valid *.cue or *.ccd input file
* Cue files with multiple *.bin or *.iso tracks **are** supported
* Referenced audio tracks that have been converted into a different format (*.mp3, *.ogg, *.ape etc..) are **not** currently supported
* If an error is encountered the DiscInspector methods will either return a `null` object, or a DiscInspector object that contains a `DetectedDiscType` property set to 'UnknownFormat'

## Setup
* Copy all *.dll files from the release archive you are using into your project
* Reference DiscTools.dll

## Methods

### Disc Inspector

#### ScanDisc()
* Performs an intensive scan in order to identify the `DetectedDiscType`. For some system discs (NeoGeoCD & Dreamcast) that may have been written in a non-ISO format this has a better chance of identification (although it **may** take longer than expected)
* Returns a `DiscInspector` object that contains a `DetectedDiscType` enum along with a `DiscTypeString` string
* Within this object is also a `DiscData` object that contains information (if any) that has been gleaned from interrogating the disc

```c#
// C# Usage
var disc = DiscInspector.ScanDisc(@"\path\to\cue_or_ccd_file");
```

```vb
' VB Usage
Dim disc = DiscInspector.ScanDisc("\path\to\cue_or_ccd_file")
```

#### QuickScanDisc()
* Performs a quick scan in order to identify the `DetectedDiscType`. For some system discs (NeoGeoCD & Dreamcast) that may have been written in a non-ISO format there is a good chance that they will not be detected properly
* Returns a `DiscInspector` object that contains a `DetectedDiscType` enum along with a `DiscTypeString` string
* Within this object is also a `DiscData` object that contains information (if any) that has been gleaned from interrogating the disc

```c#
// C# Usage
var disc = DiscInspector.QuickScanDisc(@"\path\to\cue_or_ccd_file");
```

```vb
// VB Usage
Dim disc = DiscInspector.QuickScanDisc("\path\to\cue_or_ccd_file")
```



## Attribution
This library uses code from BizHawk multi-system emulator - https://github.com/TASVideos/BizHawk

The .NET 2.0 version also uses the LinqBridge assembly by Atif Aziz - https://www.nuget.org/packages/LinqBridge/1.3.0
