# DiscTools
Class libraries for .NET2.0 and .NET4.5.2 that currently allow you to interrogate disc images (based on valid .cue, .ccd or .iso files) for the following systems:

* PSX
* PS2
* PSP
* Sega Saturn
* PC-Engine CD
* PC-FX
* SegaCD
* Philips CD-i
* NeoGeo CD
* Dreamcast
* Panasonic 3DO
* Amiga CDTV / CD32
* Bandai Playdia
* Gamecube
* Wii
* FM Towns

*There is also currently experimental support for detecting DreamCast DiscJuggler (.cdi) files. This may be very slow as the disc is not being 'mounted' in the traditional sense.*

[![Build status](https://ci.appveyor.com/api/projects/status/58juhmjpih7mw266/branch/master?svg=true)](https://ci.appveyor.com/project/Asnivor/disctools/branch/master)

See the [Releases](https://github.com/Asnivor/DiscTools/releases) page for the latest successful auto-build release.

## Limitations
* DiscTools will only accept a valid *.cue, *.ccd or *.iso input file
* Cue files with multiple *.bin or *.iso tracks **are** supported
* Referenced audio tracks that have been converted into a different format (*.mp3, *.ogg, *.ape etc..) are **not** currently supported, but DiscTools should generate a cuesheet on the fly that will attempt to load the BINARY tracks
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
var disc = DiscInspector.ScanDisc(@"\path\to\cue_or_ccd_or_iso_file");
```

```vb
# VB Usage
Dim disc = DiscInspector.ScanDisc("\path\to\cue_or_ccd_or_iso_file")
```

#### ScanDiscQuick()
* Performs a quick scan in order to identify the `DetectedDiscType`. For some system discs (NeoGeoCD & Dreamcast) that may have been written in a non-ISO format there is a good chance that they will not be detected properly
* Returns a `DiscInspector` object that contains a `DetectedDiscType` enum along with a `DiscTypeString` string
* Within this object is also a `DiscData` object that contains information (if any) that has been gleaned from interrogating the disc

```c#
// C# Usage
var disc = DiscInspector.ScanDiscQuick(@"\path\to\cue_or_ccd_or_iso_file");
```

```vb
# VB Usage
Dim disc = DiscInspector.ScanDiscQuick("\path\to\cue_or_ccd_or_iso_file")
```

#### System Specific Scans
* If you want to interrogate a disc for a certain system only (i.e you know ahead of time what the disc format is), then the methods below can be used
* In most cases this **will** be quicker than the all-out system detection options
* Again, all of these methods return a `DiscInspector` object

```c#
// C#
var psxDisc = DiscInspector.ScanPSX(@"path\to\cue_or_ccd_or_iso");
var ps2Disc = DiscInspector.ScanPS2(@"path\to\cue_or_ccd_or_iso");
var pspDisc = DiscInspector.ScanPSP(@"path\to\cue_or_ccd_or_iso");
var saturnDisc = DiscInspector.ScanSaturn(@"path\to\cue_or_ccd_or_iso");
var pcecdDisc = DiscInspector.ScanPCECD(@"path\to\cue_or_ccd_or_iso");
var pcfxDisc = DiscInspector.ScanPCFX(@"path\to\cue_or_ccd_or_iso");
var segaCDDisc = DiscInspector.ScanSegaCD(@"path\to\cue_or_ccd_or_iso");
var philipsCDiDisc = DiscInspector.ScanCDi(@"path\to\cue_or_ccd_or_iso");
var neogeoDisc = DiscInspector.ScanNeoGeoCD(@"path\to\cue_or_ccd_or_iso");
var dreamcastDisc = DiscInspector.ScanDreamcast(@"path\to\cue_or_ccd_or_iso");
var philips3DODisc = DiscInspector.Scan3DO(@"path\to\cue_or_ccd_or_iso");
var cdtvDisc = DiscInspector.ScanAmigaCDTV(@"path\to\cue_or_ccd_or_iso");
var cd32Disc = DiscInspector.ScanAmigaCD32(@"path\to\cue_or_ccd_or_iso");
var playdiaDisc = DiscInspector.ScanPlaydia(@"path\to\cue_or_ccd_or_iso");
var gcDisc = DiscInspector.ScanGamecube(@"path\to\cue_or_ccd_or_iso");
var wiiDisc = DiscInspector.ScanWii(@"path\to\cue_or_ccd_or_iso");
var townsDisc = DiscInspector.ScanTowns(@"path\to\cue_or_ccd_or_iso");
```

```vb
# VB
Dim psxDisc = DiscInspector.ScanPSX("path\to\cue_or_ccd_or_iso")
Dim ps2Disc = DiscInspector.ScanPS2(@"path\to\cue_or_ccd_or_iso")
Dim pspDisc = DiscInspector.ScanPSP(@"path\to\cue_or_ccd_or_iso")
Dim saturnDisc = DiscInspector.ScanSaturn("path\to\cue_or_ccd_or_iso")
Dim pcecdDisc = DiscInspector.ScanPCECD("path\to\cue_or_ccd_or_iso")
Dim pcfxDisc = DiscInspector.ScanPCFX("path\to\cue_or_ccd_or_iso")
Dim segaCDDisc = DiscInspector.ScanSegaCD("path\to\cue_or_ccd_or_iso")
Dim philipsCDiDisc = DiscInspector.ScanCDi("path\to\cue_or_ccd_or_iso")
Dim neogeoDisc = DiscInspector.ScanNeoGeoCD("path\to\cue_or_ccd_or_iso")
Dim dreamcastDisc = DiscInspector.ScanDreamcast("path\to\cue_or_ccd_or_iso")
Dim philips3DODisc = DiscInspector.Scan3DO(@"path\to\cue_or_ccd_or_iso")
Dim cdtvDisc = DiscInspector.ScanAmigaCDTV(@"path\to\cue_or_ccd_or_iso")
Dim cd32Disc = DiscInspector.ScanAmigaCD32(@"path\to\cue_or_ccd_or_iso")
Dim playdiaDisc = DiscInspector.ScanPlaydia(@"path\to\cue_or_ccd_or_iso")
Dim gcDisc = DiscInspector.ScanGamecube(@"path\to\cue_or_ccd_or_iso")
Dim wiiDisc = DiscInspector.ScanWii(@"path\to\cue_or_ccd_or_iso")
Dim townsDisc = DiscInspector.ScanTowns(@"path\to\cue_or_ccd_or_iso")
```


## Attribution
This library uses code from BizHawk multi-system emulator - https://github.com/TASVideos/BizHawk

The .NET 2.0 version also uses the LinqBridge assembly by Atif Aziz - https://www.nuget.org/packages/LinqBridge/1.3.0
