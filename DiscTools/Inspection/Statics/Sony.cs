using DiscTools.Inspection.Statics.SonyMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscTools.Inspection.Statics
{
    public class Sony
    {
        /*

        public Dictionary<string, string> PSPRegion { get; set; }

        public static string GetPSPRegion(string regionCode)
        {
            var psp = new Sony();
            if (psp.PSPRegion.ContainsKey(regionCode))
                return psp.PSPRegion[regionCode];

            return "Unknown Region";
        }

        public static DiscInspector ParsePSPParam(DiscInspector discI, byte[] currSector)
        {
            var sfo = new SFO(currSector);


            string sS = System.Text.Encoding.Default.GetString(currSector);

            // PARAM.SFO parsing
            if (sS.Contains("APP_VER"))
            {
                var data = Encoding.Default.GetBytes(sS).ToArray();

                var Labels = data.Skip(0x00e0 + 4);
                var Datas = data.Skip(0x0160 + 8);

                string l = Encoding.Default.GetString(Labels.ToArray());
                string d = Encoding.Default.GetString(Datas.ToArray());

                string off = Encoding.Default.GetString(Encoding.Default.GetBytes(sS).Skip(4).ToArray());
                string[] split = off.Split('\0').Where(a => a != "").ToArray();

                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i] == "APP_VER")
                        discI.Data.Version = split[i + 13];
                    if (split[i] == "BOOTABLE")
                        discI.Data.OtherData = split[i + 13];
                    if (split[i] == "CATEGORY")
                        discI.Data.MediaInfo = split[i + 13];
                    if (split[i] == "DISC_ID")
                        discI.Data.SerialNumber = split[i + 13];
                    if (split[i] == "REGION")
                        discI.Data.PeripheralCodes = split[i + 13];
                    if (split[i] == "TITLE")
                        discI.Data.GameTitle = split[i + 13];
                    if (split[i] == "DISC_VERSION")
                        discI.Data.DeviceInformation = split[i + 13];
                    if (split[i] == "PSP_SYSTEM_VER")
                        discI.Data.MediaID = split[i + 13];
                }
            }

            return discI;
        }

        public Sony()
        {
            PSPRegion = new Dictionary<string, string>()
            {
                { "0000", "Japan" },
                { "0001", "North America" },
                { "0002", "Australia / New Zealand" },
                { "0003", "United Kingdom" },
                { "0004", "Europe / India" },
                { "0005", "Korea" },
                { "0006", "Hong Kong / Singapore / Malaysia" },
                { "0007", "Taiwan" },
                { "0008", "CIS" },
                { "0009", "Mainland China" },
                { "0010", "Central / South America" }                
            };
        }

    */
    }
}
