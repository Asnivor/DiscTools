using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOFMTowns()
        {
            if (discI.Data._ISOData.DataPreparerIdentifier.StartsWith("FUJITSU") && discI.Data._ISOData.SystemIdentifier.StartsWith("HM"))
            {

                return true;

                // store lba for SYSTEM.CNF
                var cnf = discI.Data._ISOData.ISOFiles.Where(a => a.Key.Contains("SYSTEM.CNF")).FirstOrDefault();
                if (cnf.Key.Contains("SYSTEM.CNF"))
                {
                    ifn = cnf.Value;
                    CurrentLBA = Convert.ToInt32(ifn.Offset);
                }
                else
                {
                    // assume LBA 23
                    CurrentLBA = 23;
                }

                return GetFMTownsData();
            }

            return false;
        }
        
        public bool GetFMTownsData()
        {
            byte[] data = di.GetPSXSerialNumber(CurrentLBA);
            byte[] data32 = data.ToList().Take(200).ToArray();

            string sS = System.Text.Encoding.Default.GetString(data32);

            return GetFMTownsData(sS);
        }

        public bool GetFMTownsData(string lbaString)
        {
            // not yet implemented
            return false;
        }
    }
}
