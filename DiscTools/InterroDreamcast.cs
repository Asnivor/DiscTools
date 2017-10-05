using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        public bool ScanISODreamcast()
        {
            if (discI.Data.ISOData.SystemIdentifier.Contains("SEGAKATANA"))
            {
                // store lba for IP.BIN
                var cnf = discI.Data.ISOData.ISOFiles.Where(a => a.Key.Contains("IP.BIN")).FirstOrDefault();
                if (cnf.Key.Contains("IP.BIN"))
                {
                    ifn = cnf.Value;
                    CurrentLBA = Convert.ToInt32(ifn.Offset);
                }

                return GetDreamcastData();
            }

            return false;
        }
        
        public bool GetDreamcastData()
        {
            byte[] data = di.ReadData(CurrentLBA, 2048);
            currSector = data;
            string res = System.Text.Encoding.Default.GetString(data);

            return GetDreamcastData(res);
        }

        public bool GetDreamcastData(string lbaString)
        {
            if (!lbaString.ToLower().Contains("segakatana"))
                return false;

            int ind = lbaString.ToLower().IndexOf("segakatana");
            string d = lbaString.Substring(lbaString.ToLower().IndexOf("segakatana"));

            List<string> header = new List<string>();

            byte[] dat = System.Text.Encoding.Default.GetBytes(d);

            for (int i = 0; i < 20; i++)
            {
                string lookup = System.Text.Encoding.Default.GetString(dat.Skip((i * 16) - 5).Take(16).ToArray());
                header.Add(lookup);
            }

            discI.Data.SerialNumber = header[4].Split(' ').First().Trim();
            discI.Data.Version = header[4].Split(' ').Last().Trim();
            discI.Data.GameTitle = (header[8] + header[9]).Trim();
            discI.Data.InternalDate = header[5].Trim();
            discI.Data.Publisher = header[1].Trim();
            discI.Data.AreaCodes = header[3].Split(' ').First().Trim();
            discI.Data.PeripheralCodes = header[3].Split(' ').Last().Trim();

            discI.Data.MediaID = header[2].Split(' ').First().Trim();
            discI.Data.MediaInfo = header[2].Trim().Split(' ').Last().Trim();

            discI.Data.DeviceInformation = header[0].Trim();
            discI.Data.ManufacturerID = header[7].Trim();

            return true;
        }
    }
}
