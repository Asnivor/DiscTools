using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISONeoGeoCD()
        {
            var absTxt = discI.Data._ISOData.ISOFiles.Where(a => a.Key.Contains("ABS.TXT")).ToList();
            if (absTxt.Count == 0)
                return false;

            CurrentLBA = Convert.ToInt32(absTxt.First().Value.Offset);

            if (GetNeoGeoCDData())
                return true;

            return false;
        }
        
        public bool GetNeoGeoCDData()
        {
            currSector = di.ReadData(CurrentLBA, 2048);

            string sS = System.Text.Encoding.Default.GetString(currSector);

            return GetNeoGeoCDData(sS);
        }

        public bool GetNeoGeoCDData(string lbaString)
        {
            if (lbaString.ToLower().Contains("abstracted by snk"))
                return true;

            return false;
        }
    }
}
