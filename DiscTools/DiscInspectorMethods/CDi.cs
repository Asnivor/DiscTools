using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiscTools
{
    public partial class DiscInspector
    {
        public static DiscInspector ScanCDi(string cuePath)
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

            if (DISC.GetCDiInfo())
            {
                DISC.DetectedDiscType = DetectedDiscType.PhilipsCDi;
                DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
            }
            
            return DISC;
        }

        public bool GetCDiInfo()
        {
            // CDi header data appears to be on LBA16 (through trial and erro) and has no pointer in the TOC. 
            // Because I am not 100% sure of this we will start at LBA0 and run through to LBA30
            for (int i = 0; i < 31; i++)
            {
                byte[] data = di.ReadData(i, 2048);
                string result = System.Text.Encoding.ASCII.GetString(data);
                if (result.ToLower().Contains("cd-rtos"))
                {
                    Data.ManufacturerID = System.Text.Encoding.Default.GetString(data.ToList().Skip(1).Take(4).ToArray());
                    Data.OtherData = System.Text.Encoding.Default.GetString(data.ToList().Skip(8).Take(16).ToArray()).Trim();
                    int start = 190;
                    int block = 128;

                    List<string> header = new List<string>();
                    for (int a = 0; a < 10; a++)
                    {
                        string test = System.Text.Encoding.Default.GetString(data.ToList().Skip(190 + (a * block)).Take(block).ToArray());
                        header.Add(test);
                    }

                    Data.DeviceInformation = header[3].Trim();
                    Data.GameTitle = header[0].Trim();
                    Data.Publisher = header[1].Trim();
                    Data.Developer = header[2].Trim();

                    Data.InternalDate = TruncateLongString(header[5], 12);

                    return true;
                }
            }
            return false;
        }
    }
}
