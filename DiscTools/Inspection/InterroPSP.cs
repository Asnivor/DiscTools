using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISOPSP()
        {
            if (discI.Data.ISOData.ApplicationIdentifier == "PSP GAME")
            {
                // get PARAM.SFO
                ISO.ISODirectoryNode cnf = discI.Data.ISOData.ISOFiles.Where(a => a.Key == "PSP_GAME").FirstOrDefault().Value as ISO.ISODirectoryNode;

                var param = cnf.Children.Where(a => a.Key == "PARAM.SFO").FirstOrDefault();              

                if (param.Key.Contains("PARAM.SFO"))
                {
                    ifn = param.Value;
                    CurrentLBA = Convert.ToInt32(ifn.Offset);
                    currSector = di.ReadData(CurrentLBA, 2048);
                    discI = Statics.SonyLookup.ParsePSPParam(discI, currSector);
                }

                // ISO data
                string ss = discI.Data.ISOData.Reserved;
                string[] sArr = ss.Split('|');

                if (sArr.Length > 1)
                {
                    discI.Data.SerialNumber = sArr[0];
                    discI.Data.ManufacturerID = sArr[1];
                    discI.Data.AreaCodes = Statics.SonyLookup.GetPSPRegion(sArr[2].Replace("  ", " ").Split(' ').First());
                }

                return GetPSPData();
            }

            return false;
        }
        
        public bool GetPSPData()
        {
            byte[] data = di.ReadData(CurrentLBA, 2048);
            byte[] data32 = data.ToList().Take(2048).ToArray();

            string sS = System.Text.Encoding.Default.GetString(data32);

            return GetPSPData(sS);
        }

        public bool GetPSPData(string lbaString)
        {
            // volume descriptor parsing
            if (lbaString.Contains("|"))
            {

                string[] array = lbaString.Split('|');
                if (array.Length > 3)
                {
                    string serial = array[0];
                    string two = array[1];
                    string three = array[2];
                    string four = array[3];

                    discI.Data.SerialNumber = serial;
                    discI.Data.MediaInfo = two;
                    discI.Data.AreaCodes = Statics.SonyLookup.GetPSPRegion(three);
                    discI.Data.PeripheralCodes = four;
                }

                return true;
                
            }

            

            return false;
        }
    }
}
