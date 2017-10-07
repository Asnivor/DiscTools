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
                int ind = lbaString.IndexOf("PC Engine");
                string d = lbaString.Substring(lbaString.IndexOf("PC Engine"));

                byte[] newData = System.Text.Encoding.ASCII.GetBytes(d);

                byte[] dataSm1 = newData.Skip(74).Take(16).ToArray();
                string t1 = System.Text.Encoding.Default.GetString(dataSm1).Replace('\0', ' ').Trim();

                // get game name
                if (t1.Trim() != "")
                    discI.Data.GameTitle = t1;

                return true;
            }

            return false;
        }
    }
}
