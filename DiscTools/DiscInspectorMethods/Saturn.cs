using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiscTools
{
    public partial class DiscInspector
    {
        public static DiscInspector ScanSaturn(string cuePath)
        {
            var DISC = new DiscInspector();
            DISC.CuePath = cuePath;
            DISC.IntensiveScanning = true;
            DISC.InitProcess();

            if (DISC.isIso == true)
            {
                // take only the first volume descriptor (all the discs Ive seen so far that have multiple - anything after the first is null values)
                var vs = DISC.iso.VolumeDescriptors.Where(a => a != null).ToArray().First();

                // translate the vd
                DISC.Data.ISOData = PopulateISOData(vs);
                DISC.Data.ISOData.ISOFiles = DISC.iso.Root.Children;

                if (DISC.Data.ISOData.SystemIdentifier.Contains("SEGA SEGASATURN"))
                {
                    if (DISC.GetSaturnInfo())
                    {
                        DISC.DetectedDiscType = DetectedDiscType.SegaSaturn;
                        DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                        return DISC;
                    }
                    /*
                    var ipBin = Data.ISOData.ISOFiles.Where(a => a.Key.Contains("IP.BIN")).FirstOrDefault();
                    ifn = ipBin.Value;
                    CurrentLBA = Convert.ToInt32(ifn.Offset);
                    if (GetDreamcastInfo())
                        return DetectedDiscType.DreamCast;
                        */
                }
            }

            bool satTest = DISC.StringAt("SEGA SEGASATURN", 0);
            if (satTest)
            {
                if (DISC.GetSaturnInfo())
                {
                    DISC.DetectedDiscType = DetectedDiscType.SegaSaturn;
                    DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                    return DISC;
                }                   
            }
            return DISC;
        }


        public bool GetSaturnInfo()
        {
            // read 2048 bytes of data from lba 0 (as saturn info is in the header)
            byte[] d = di.ReadData(0, 2048);

            string temp = System.Text.Encoding.Default.GetString(d.ToList().Skip(0).Take(1024).ToArray());

            // read the info
            Data.ManufacturerID = System.Text.Encoding.Default.GetString(d.ToList().Skip(16).Take(16).ToArray()).Trim();

            /* These appear on the same 'line' but the offset appears to change. Will just try splitting by whitespace
            Data.SerialNumber = System.Text.Encoding.Default.GetString(d.ToList().Skip(32).Take(10).ToArray()).Trim();
            Data.Version = System.Text.Encoding.Default.GetString(d.ToList().Skip(42).Take(7).ToArray()).Trim();
            */

            string serialAndVer = System.Text.Encoding.Default.GetString(d.ToList().Skip(32).Take(16).ToArray()).Trim();

            // split by V
            string[] arr1 = serialAndVer.Split('V');
            Data.SerialNumber = arr1[0];
            Data.Version = "V" + arr1[1];

            Data.InternalDate = System.Text.Encoding.Default.GetString(d.ToList().Skip(48).Take(8).ToArray()).Trim();
            Data.DeviceInformation = System.Text.Encoding.Default.GetString(d.ToList().Skip(56).Take(8).ToArray()).Trim();
            Data.AreaCodes = System.Text.Encoding.Default.GetString(d.ToList().Skip(64).Take(16).ToArray()).Trim();
            Data.PeripheralCodes = System.Text.Encoding.Default.GetString(d.ToList().Skip(80).Take(8).ToArray()).Trim(); // saturn docs show this area as 10 bytes, but it looks like its actually 8
            Data.GameTitle = System.Text.Encoding.Default.GetString(d.ToList().Skip(86).Take(120).ToArray()).Trim();

            return true;
        }
    }
}
