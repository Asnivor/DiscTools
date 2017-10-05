using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOPCFX()
        {
            // not implemented
            return false;
        }
        
        public bool GetPCFXData()
        {
            currSector = di.ReadData(CurrentLBA, 2048);

            string sS = System.Text.Encoding.Default.GetString(currSector);

            return GetPCFXData(sS);
        }

        public bool GetPCFXData(string lbaString)
        {
            if (lbaString.ToLower().Contains("pc-fx"))
            {
                byte[] newData = System.Text.Encoding.ASCII.GetBytes(lbaString);

                if (lbaString.ToLower().StartsWith("pc-fx:hu_cd"))
                {
                    // disc format does not have a gametitle                                
                }
                else
                {
                    // game title should exist
                    byte[] dataSm = newData.Skip(106).Take(48).ToArray();
                    string t = System.Text.Encoding.Default.GetString(dataSm).Replace('\0', ' ').Trim().Split(new string[] { "  " }, StringSplitOptions.None).FirstOrDefault();
                    discI.Data.GameTitle = t;
                }

                return true;
            }

            return false;
        }
    }
}
