using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOPlaydia()
        {
            if (discI.Data.ISOData.SystemIdentifier.Contains("ASAHI-CDV"))
            {
                return true;
            }

            return false;
        }
        
        public bool GetPlaydiaData()
        {
            currSector = di.ReadData(CurrentLBA, 2048);

            string sS = System.Text.Encoding.Default.GetString(currSector);

            return GetPlaydiaData(sS);
        }

        public bool GetPlaydiaData(string lbaString)
        {
            if (lbaString.ToLower().Contains("asahi-cdv"))
                return true;

            return false;
        }
    }
}
