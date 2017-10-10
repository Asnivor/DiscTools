using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOWii()
        {
            // no implementation

            return false;
        }
        
        public bool GetWiiData()
        {
            byte[] data = di.ReadData(CurrentLBA, 2048);
            currSector = data;
            string res = System.Text.Encoding.Default.GetString(data);

            return GetWiiData(res);
        }

        public bool GetWiiData(string lbaString)
        {
            // most of this stuff gleaned from:  http://wiibrew.org/wiki/Wii_Disc

            // console identification
            // dvdMagic
            string dvdMag = getHexStringFromByteArray(Encoding.Default.GetBytes(lbaString).Skip(24).Take(4).ToArray());

            if (dvdMag != "5D1C9EA3")
                return false;

            discI.Data.OtherData = dvdMag;
            discI.DetectedDiscType = DetectedDiscType.Wii;

            string consoleId = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(0).Take(1).ToArray());
            discI.Data.DeviceInformation = Statics.Nintendo.GetDiscId(consoleId);

            // game name
            string gName = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(32).Take(992).ToArray()).Trim().TrimEnd('\0');
            discI.Data.GameTitle = gName;

            // game code
            string gc = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(1).Take(2).ToArray());
            discI.Data.MediaID = gc;

            // country code
            string cc = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(3).Take(1).ToArray());
            discI.Data.AreaCodes = Statics.Nintendo.GetRegion(cc);

            // maker code
            string makerHex = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(4).Take(2).ToArray());
            
            discI.Data.Publisher = Statics.Nintendo.GetMaker(makerHex);

            // disc id
            string discId = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(6).Take(1).ToArray());
            discI.Data.SerialNumber = discId;

            // disc number
            string discNum = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(7).Take(1).ToArray());
            discI.Data.MediaInfo = "Disc " + discNum;

            // version
            string ver = Encoding.Default.GetString(Encoding.Default.GetBytes(lbaString).Skip(8).Take(1).ToArray());
            discI.Data.Version = ver;

            
            
            return true;
        }
    }
}
