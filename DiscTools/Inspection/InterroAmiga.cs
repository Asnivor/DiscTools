using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOAmiga()
        {
            if (discI.Data._ISOData.SystemIdentifier.Contains("CDTV") || discI.Data._ISOData.SystemIdentifier.Contains("AMIGA"))
            {
                // is it CDTV or CD32?
                foreach (var child in discI.Data._ISOData.ISOFiles)
                {
                    if (child.Key.ToLower().Contains("cd32"))
                    {
                        DiscSubType = DetectedDiscType.AmigaCD32;
                    }
                }

                DiscSubType = DetectedDiscType.AmigaCDTV;
                return true;
            }

            return false;
        }
        
        public bool GetAmigaData()
        {
            currSector = di.ReadData(CurrentLBA, 2048);

            string sS = System.Text.Encoding.Default.GetString(currSector);            

            return GetAmigaData(sS);
        }

        public bool GetAmigaData(string lbaString)
        {
            if (lbaString.ToLower().Contains("amiga"))
            {
                DiscSubType = DetectedDiscType.AmigaCDTV;

                if (lbaString.ToLower().Contains("cd32"))
                    DiscSubType = DetectedDiscType.AmigaCD32;

                return true;
            }

            return false;
        }
    }
}
