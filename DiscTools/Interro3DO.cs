using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISO3DO()
        {
            // not implemented
            return false;
        }
        
        public bool Get3DOData()
        {
            currSector = di.ReadData(CurrentLBA, 2048);

            string sS = System.Text.Encoding.Default.GetString(currSector);

            return Get3DOData(sS);
        }

        public bool Get3DOData(string lbaString)
        {
            if (lbaString.ToLower().Contains("iamaduckiamaduck"))
            {
                return true;
            }

            return false;
        }
    }
}
