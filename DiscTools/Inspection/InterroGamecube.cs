using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOGamecube()
        {
            // no implementation

            return false;
        }
        
        public bool GetGamecubeData()
        {
            byte[] data = di.ReadData(CurrentLBA, 2048);
            currSector = data;
            string res = System.Text.Encoding.Default.GetString(data);

            return GetGamecubeData(res);
        }

        public bool GetGamecubeData(string lbaString)
        {
            // most of this stuff gleaned from:  https://github.com/sleepy9090/GameCubeIsoAnalyzer

            // console identification
            string consoleId = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(0).Take(1).ToArray());
            switch (consoleId)
            {
                case "D":   // Emulated/Ported/Promotional
                    discI.DetectedDiscType = DetectedDiscType.Gamecube;
                    discI.Data.DeviceInformation = "Emulated/Ported/Promotional";
                    break;
                case "G":   // Gamecube
                    discI.DetectedDiscType = DetectedDiscType.Gamecube;
                    discI.Data.DeviceInformation = "Gamecube";
                    break;
                case "U":   // GBA-Player Boot CD
                    discI.DetectedDiscType = DetectedDiscType.Gamecube;
                    discI.Data.DeviceInformation = "GBA-Player Boot CD";
                    break;
                default:
                    return false;
            }

            // game name
            string gName = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(32).Take(992).ToArray()).Trim().TrimEnd('\0');
            discI.Data.GameTitle = gName;

            // game code
            string gc = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(1).Take(2).ToArray());
            discI.Data.MediaID = gc;

            // country code
            string cc = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(3).Take(1).ToArray());
            switch (cc)
            {
                case "E":
                    discI.Data.AreaCodes = "E - USA/NTSC";
                    break;
                case "P":
                    discI.Data.AreaCodes = "P - Europe/PAL";
                    break;
                case "J":
                    discI.Data.AreaCodes = "J - Japan/NTSC";
                    break;
                case "U":
                    discI.Data.AreaCodes = "U - Europe/PAL (LoZ Oot (MQ))?";
                    break;
            }

            // maker code
            string makerHex = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(4).Take(2).ToArray());
            string vendor = "";

            switch (makerHex)
            {
                case "01":
                    vendor = "Nintendo";
                    break;
                case "08":
                    vendor = "Capcom";
                    break;
                case "41":
                    vendor = "Ubisoft";
                    break;
                case "4F":
                    vendor = "Eidos";
                    break;
                case "51":
                    vendor = "Acclaim";
                    break;
                case "52":
                    vendor = "Activision";
                    break;
                case "5D":
                    vendor = "Midway";
                    break;
                case "5G":
                    vendor = "Hudson";
                    break;
                case "64":
                    vendor = "Lucas Arts";
                    break;
                case "69":
                    vendor = "Electronic Arts";
                    break;
                case "6S":
                    vendor = "TDK Mediactive";
                    break;
                case "8P":
                    vendor = "Sega";
                    break;
                case "A4":
                    vendor = "Mirage Studios";
                    break;
                case "AF":
                    vendor = "Namco";
                    break;
                case "B2":
                    vendor = "Bandai";
                    break;
                case "DA":
                    vendor = "Tomy";
                    break;
                case "EM":
                    vendor = "Konami";
                    break;
                default:
                    vendor = "Unknown";
                    break;
            }

            discI.Data.ManufacturerID = vendor;

            // disc id
            string discId = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(6).Take(1).ToArray());
            discI.Data.SerialNumber = discId;

            // version
            string ver = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(7).Take(1).ToArray());
            discI.Data.Version = ver;

            // dvdMagic
            string dvdMag = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(28).Take(4).ToArray());
            discI.Data.OtherData = dvdMag;
            
            return true;
        }
    }
}
