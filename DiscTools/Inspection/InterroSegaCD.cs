using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOSegaCD()
        {
            if (discI.Data._ISOData.SystemIdentifier.Contains("MEGA_CD"))
            {
                if (StringAt("SEGADISCSYSTEM", 0))
                {
                    CurrentLBA = 0;
                    return GetSegaCDData();
                }

                if (StringAt("SEGADISCSYSTEM", 16))
                {
                    CurrentLBA = 16;
                    return GetSegaCDData();
                }
            }

            return false;
        }
        
        public bool GetSegaCDData()
        {
            currSector = di.ReadData(CurrentLBA, 2048);

            string sS = System.Text.Encoding.Default.GetString(currSector);

            return GetSegaCDData(sS);
        }

        public bool GetSegaCDData(string lbaString)
        {
            List<string> header = new List<string>();

            for (int i = 0; i < 100; i++)
            {
                string l = System.Text.Encoding.ASCII.GetString(currSector.ToList().Skip(i * 16).Take(16).ToArray());
                header.Add(l);
            }

            if (!header[16].Trim().Contains("SEGA"))
                return false;

            discI.Data.ManufacturerID = header[16].Trim();
            discI.Data.GameTitle = (header[18] + header[19]).Trim();
            discI.Data.SerialNumber = header[24].Trim();
            discI.Data.AreaCodes = header[31].Trim();
            discI.Data.PeripheralCodes = header[25].Trim();
            discI.Data.InternalDate = header[5].Trim();
            discI.Data.DeviceInformation = header[0].Trim();
            discI.Data.OtherData = header[17].Trim();

            return true;
        }
    }
}
