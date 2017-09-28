using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiscTools
{
    public partial class DiscInspector
    {
        public static DiscInspector ScanPSX(string cuePath)
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
            }

            // try lba 23 first
            DISC.CurrentLBA = 23;
            if (DISC.GetPSXInfo())
            {
                DISC.DetectedDiscType = DetectedDiscType.SonyPSX;
                DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                return DISC;
            }

            // it wasnt at lba 23 - check ISO
            if (DISC.isIso == true)
            {
                
                ISO.ISONode ifn = null;

                /* PSX */
                if (DISC.Data.ISOData.ApplicationIdentifier == "PLAYSTATION")
                {
                    // store lba for SYSTEM.CNF
                    var cnf = DISC.Data.ISOData.ISOFiles.Where(a => a.Key.Contains("SYSTEM.CNF")).FirstOrDefault();
                    if (cnf.Key.Contains("SYSTEM.CNF"))
                    {
                        ifn = cnf.Value;
                        DISC.CurrentLBA = Convert.ToInt32(ifn.Offset);
                    }
                    else
                    {
                        // assume LBA 23
                        DISC.CurrentLBA = 23;
                    }

                    if (DISC.GetPSXInfo())
                    {
                        DISC.DetectedDiscType = DetectedDiscType.SonyPSX;
                        DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                        return DISC;
                    }
                    
                }
            }


            var tocItems = DISC.disc.TOC.TOCItems.Where(a => a.Exists == true && a.IsData == true).ToList();
            foreach (var toc in tocItems)
            {
                DISC.CurrentLBA = toc.LBA;
                if (DISC.GetPSXInfo())
                {
                    DISC.DetectedDiscType = DetectedDiscType.SonyPSX;
                    DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                    return DISC;
                }
            }

            return DISC;
        }

        public bool GetPSXInfo()
        {
            byte[] data = di.GetPSXSerialNumber(CurrentLBA);
            byte[] data32 = data.ToList().Take(200).ToArray();

            string sS = System.Text.Encoding.Default.GetString(data32);

            if (!sS.Contains("cdrom:"))
                return false;

            // get the actual serial number from the returned string
            string[] arr = sS.Split(new string[] { "cdrom:" }, StringSplitOptions.None);
            string[] arr2 = arr[1].Split(new string[] { ";1" }, StringSplitOptions.None);
            string serial = arr2[0].Replace("_", "-").Replace(".", "");
            if (serial.Contains("\\"))
                serial = serial.Split('\\').Last();
            else
                serial = serial.TrimStart('\\').TrimStart('\\');

            // try and remove any nonsense after the serial
            string[] sarr2 = serial.Split('\r');
            if (sarr2.Length > 1)
                serial = sarr2.First();

            Data.SerialNumber = serial;

            return true;

        }
    }
}
