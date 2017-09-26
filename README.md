# DiscTools
Class libraries for .NET2.0 and .NET4.5.2 that allow you to interogate a PSX or Saturn disc image (based on code from the BizHawk team)

## Usage (C#):

* Add a reference to the DiscTools.dll
* string serial = DiscSN.SerialNumber.GetPSXSerial(@"\path\to\cue_or_ccd_file");

For safety, you should probably wrap the above in a try/catch block to handle any exceptions that may occur (the library hasnt been fully tested).

## Usage (VB):

* Add a reference to the DiscTools.dll
* Dim serial As String = DiscSN.SerialNumber.GetPSXSerial("\path\to\cue_or_ccd_file")

For safety, you should probably wrap the above in a try/catch block to handle any exceptions that may occur (the library hasnt been fully tested).

## Attribution
This library uses code from BizHawk multi-system emulator - https://github.com/TASVideos/BizHawk

The .NET 2.0 version also uses the LinqBridge assembly by Atif Aziz - https://www.nuget.org/packages/LinqBridge/1.3.0
