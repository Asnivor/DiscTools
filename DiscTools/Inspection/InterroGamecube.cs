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
            // dvdMagic
            //string dvdMag = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(28).Take(4).ToArray());
            string dvdMag = getHexStringFromByteArray(Encoding.Default.GetBytes(lbaString).Skip(28).Take(4).ToArray());

            if (dvdMag != "C2339F3D")
                return false;

            discI.Data.OtherData = dvdMag;
            discI.DetectedDiscType = DetectedDiscType.Gamecube;

            string consoleId = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(0).Take(1).ToArray());
            discI.Data.DeviceInformation = Statics.NintendoLookup.GetDiscId(consoleId);

            // game name
            string gName = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(32).Take(992).ToArray()).Trim().TrimEnd('\0');
            discI.Data.GameTitle = gName;

            // game code
            string gc = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(1).Take(2).ToArray());
            discI.Data.MediaID = gc;

            // country code
            string cc = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(3).Take(1).ToArray());
            discI.Data.AreaCodes = Statics.NintendoLookup.GetRegion(cc);

            // maker code
            string makerHex = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(4).Take(2).ToArray());
            
            discI.Data.Publisher = Statics.NintendoLookup.GetMaker(makerHex);

            // disc id
            string discId = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(6).Take(1).ToArray());
            discI.Data.SerialNumber = discId;

            // version
            string ver = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(7).Take(1).ToArray());
            discI.Data.Version = ver;

            
            
            return true;
        }
    }
}
