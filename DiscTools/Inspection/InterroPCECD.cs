using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOPCECD()
        {
            // not implemented
            return false;
        }
        
        public bool GetPCECDData()
        {
            currSector = di.ReadData(CurrentLBA, 2048);

            string sS = System.Text.Encoding.Default.GetString(currSector);

            return GetPCECDData(sS);
        }

        public bool GetPCECDData(string lbaString)
        {
            if (lbaString.ToLower().Contains("pc engine") && !lbaString.ToLower().Contains("pc-fx"))
            {
                byte[] newData = System.Text.Encoding.ASCII.GetBytes(lbaString);

                byte[] dataSm1 = newData.Skip(0).ToArray();
                string t1 = System.Text.Encoding.Default.GetString(dataSm1).Replace('\0', ' ').Trim();

                // get game name
                byte[] dataSm = newData.Skip(106).Take(48).ToArray();
                string t = System.Text.Encoding.Default.GetString(dataSm).Replace('\0', ' ').Trim().Split(new string[] { "  " }, StringSplitOptions.None).FirstOrDefault();
                discI.Data.GameTitle = t;

                return true;
            }

            return false;
        }
    }
}
