using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using DiscTools.Inspection.Statics.SonyMethods;
using DiscTools.Objects;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        /// <summary>
        /// We will assume that PSP is always a valid ISO format (for now)
        /// </summary>
        /// <returns></returns>
        public bool ScanISOPSP()
        {
            bool isPSP = false;

            // check app ident
            if (discI.Data._ISOData.ApplicationIdentifier == "PSP GAME")
                isPSP = true;

            // try and get data from RESERVED volume descriptor field
            string[] reserved = discI.Data._ISOData.Reserved.Split('|');
            if (reserved.Length > 1)
            {
                discI.Data.SerialNumber = reserved[0];
                discI.Data.ManufacturerID = reserved[1];
            }

            // check for existance of PSP_GAME folder
            ISO.ISODirectoryNode cnf = discI.Data._ISOData.ISOFiles.Where(a => a.Key == "PSP_GAME").FirstOrDefault().Value as ISO.ISODirectoryNode;
            if (cnf == null)
                return false;
            isPSP = true;

            // check for UMD_DATA.bin
            var umd = discI.Data._ISOData.ISOFiles.Where(a => a.Key == "UMD_DATA.BIN").FirstOrDefault();
            if (umd.Key.Contains("UMD_DATA.BIN"))
            {
                ifn = umd.Value;
                CurrentLBA = Convert.ToInt32(ifn.Offset);
                currSector = di.ReadData(CurrentLBA, 2048);
                string umdStr = Encoding.Default.GetString(currSector);
                string[] umdArr = umdStr.Split('|');
                if (umdArr.Length > 1)
                {
                    discI.Data.SerialNumber = umdArr[0];
                    discI.Data.ManufacturerID = umdArr[1];
                }
            }

            // attempt to parse PARAM.SFO
            var param = cnf.Children.Where(a => a.Key == "PARAM.SFO").FirstOrDefault();
            if (param.Key.Contains("PARAM.SFO"))
            {
                ifn = param.Value;
                CurrentLBA = Convert.ToInt32(ifn.Offset);
                currSector = di.ReadData(CurrentLBA, 2048);
                SFO sfo = new SFO(currSector);
                PSPData.ParsePSPData(discI, sfo);
            }

            return isPSP;
        }
        
        public bool GetPSPData()
        {
            return false;
            /*
            byte[] data = di.ReadData(CurrentLBA, 2048);
            byte[] data32 = data.ToList().Take(2048).ToArray();

            string sS = System.Text.Encoding.Default.GetString(data32);

            return GetPSPData(sS);
            */
        }

        public bool GetPSPData(string lbaString)
        {

            /*
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
                    discI.Data.AreaCodes = Statics.Sony.GetPSPRegion(three);
                    discI.Data.PeripheralCodes = four;
                }

                return true;
                
            }

            */

            return false;
        }
    }
}
