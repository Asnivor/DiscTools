using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOSaturn()
        {
            if (discI.Data._ISOData.SystemIdentifier.Contains("SEGA SEGASATURN"))
            {
                CurrentLBA = 0;
                return GetSaturnData();
            }

            return false;
        }
        
        public bool GetSaturnData()
        {
            currSector = di.ReadData(CurrentLBA, 2048);

            string sS = System.Text.Encoding.Default.GetString(currSector);

            return GetSaturnData(sS);
        }

        public bool GetSaturnData(string lbaString)
        {
            if (!System.Text.Encoding.Default.GetString(currSector.ToList().Skip(16).Take(16).ToArray()).Trim().Contains("SEGA"))
                return false;

            // read the info
            discI.Data.ManufacturerID = System.Text.Encoding.Default.GetString(currSector.ToList().Skip(16).Take(16).ToArray()).Trim();

            /* These appear on the same 'line' but the offset appears to change. Will just try splitting by whitespace
            Data.SerialNumber = System.Text.Encoding.Default.GetString(d.ToList().Skip(32).Take(10).ToArray()).Trim();
            Data.Version = System.Text.Encoding.Default.GetString(d.ToList().Skip(42).Take(7).ToArray()).Trim();
            */

            string serialAndVer = System.Text.Encoding.Default.GetString(currSector.ToList().Skip(32).Take(16).ToArray()).Trim();

            // split by V
            string[] arr1 = serialAndVer.Split('V');
            discI.Data.SerialNumber = arr1[0].Trim();
            discI.Data.Version = ("V" + arr1[1]).Trim();

            discI.Data.InternalDate = System.Text.Encoding.Default.GetString(currSector.ToList().Skip(48).Take(8).ToArray()).Trim();
            discI.Data.DeviceInformation = System.Text.Encoding.Default.GetString(currSector.ToList().Skip(56).Take(8).ToArray()).Trim();
            discI.Data.AreaCodes = System.Text.Encoding.Default.GetString(currSector.ToList().Skip(64).Take(16).ToArray()).Trim();
            discI.Data.PeripheralCodes = System.Text.Encoding.Default.GetString(currSector.ToList().Skip(80).Take(8).ToArray()).Trim(); // saturn docs show this area as 10 bytes, but it looks like its actually 8
            discI.Data.GameTitle = System.Text.Encoding.Default.GetString(currSector.ToList().Skip(86).Take(120).ToArray()).Trim();

            return true;
        }
    }
}
