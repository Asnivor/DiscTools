using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiscTools
{
    public partial class DiscInspector
    {
        public static DiscInspector ScanSegaCD(string cuePath)
        {
            var DISC = new DiscInspector();
            DISC.CuePath = cuePath;
            DISC.IntensiveScanning = true;
            DISC.InitProcess();

            if (DISC.disc == null || DISC.DetectedDiscType == DetectedDiscType.UnknownFormat || DISC.DetectedDiscType == DetectedDiscType.UnknownCDFS)
            {
                string newCue = CueHandler.ParseCue(cuePath);
                DISC.CuePath = newCue;
                DISC.InitProcess();
                if (System.IO.File.Exists(newCue))
                    System.IO.File.Delete(newCue);
                DISC.CuePath = cuePath;
            }

            if (DISC.isIso == true)
            {
                // take only the first volume descriptor (all the discs Ive seen so far that have multiple - anything after the first is null values)
                var vs = DISC.iso.VolumeDescriptors.Where(a => a != null).ToArray().First();

                // translate the vd
                DISC.Data.ISOData = PopulateISOData(vs);
                DISC.Data.ISOData.ISOFiles = DISC.iso.Root.Children;
            }

            bool segacdTest = DISC.StringAt("SEGADISCSYSTEM", 0) || DISC.StringAt("SEGADISCSYSTEM", 16);
            if (segacdTest)
            {
                if (DISC.GetMegaCDInfo())
                {
                    DISC.DetectedDiscType = DetectedDiscType.SegaCD;
                    DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                    return DISC;
                }
            }
            return DISC;
        }


        public bool GetMegaCDInfo()
        {
            List<string> header = new List<string>();
            
            if (isIso)
            {
                // read from LBA0 on megaCD
                byte[] d1 = di.ReadData(0, 2048);

                for (int i = 0; i < 100; i++)
                {
                    string l = System.Text.Encoding.ASCII.GetString(d1.ToList().Skip(i * 16).Take(16).ToArray());
                    header.Add(l);
                }

                Data.ManufacturerID = header[16].Trim();
                Data.GameTitle = (header[18] + header[19]).Trim();
                Data.SerialNumber = header[24].Trim();
                Data.AreaCodes = header[31].Trim();
                Data.PeripheralCodes = header[25].Trim();
                Data.InternalDate = header[5].Trim();
                Data.DeviceInformation = header[0].Trim();
                Data.OtherData = header[17].Trim();

                return true;
            }
            
            
            // read 2048 bytes of data from lba 0 (as MegaCD info is in the header)
            byte[] d = di.ReadData(0, 2048);

            for (int i = 0; i < 100; i++)
            {
                string l = System.Text.Encoding.ASCII.GetString(d.ToList().Skip(i * 16).Take(16).ToArray());
                header.Add(l);
            }

            if (header[16].Trim().Contains("SEGA"))
            {
                Data.ManufacturerID = header[16].Trim();
                Data.GameTitle = (header[18] + header[19]).Trim();
                Data.SerialNumber = header[24].Trim();
                Data.AreaCodes = header[31].Trim();
                Data.PeripheralCodes = header[25].Trim();
                Data.InternalDate = header[5].Trim();
                Data.DeviceInformation = header[0].Trim();
                Data.OtherData = header[17].Trim();

                return true;
            }
            else
            {
                d = di.ReadData(16, 2048);
                header.Clear();
                for (int i = 0; i < 100; i++)
                {
                    string l = System.Text.Encoding.ASCII.GetString(d.ToList().Skip(i * 16).Take(16).ToArray());
                    header.Add(l);
                }

                if (header[16].Trim().Contains("SEGA"))
                {
                    Data.ManufacturerID = header[16].Trim();
                    Data.GameTitle = (header[18] + header[19]).Trim();
                    Data.SerialNumber = header[24].Trim();
                    Data.AreaCodes = header[31].Trim();
                    Data.PeripheralCodes = header[25].Trim();
                    Data.InternalDate = header[5].Trim();
                    Data.DeviceInformation = header[0].Trim();
                    Data.OtherData = header[17].Trim();

                    return true;
                }

                return false;
            }           
        }
    }
}
