using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiscTools
{
    public partial class DiscInspector
    {
        public static DiscInspector ScanPCFX(string cuePath)
        {
            var DISC = new DiscInspector();
            DISC.CuePath = cuePath;
            DISC.IntensiveScanning = true;
            DISC.InitProcess();

            if (DISC.isIso == true)
            {
                // take only the first volume descriptor (all the discs Ive seen so far that have multiple - anything after the first is null values)
                var vs = DISC.iso.VolumeDescriptors.Where(a => a != null).ToArray().First();

                // translate the vd
                DISC.Data.ISOData = PopulateISOData(vs);
                DISC.Data.ISOData.ISOFiles = DISC.iso.Root.Children;
            }


            if (DISC.GetPCFXInfo())
            {
                DISC.DetectedDiscType = DetectedDiscType.PCFX;
                DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                return DISC;
            }

            return DISC;
        }

        public bool GetPCFXInfo()
        {
            
            // PCFX appears to have its ident information either in the LBA of one of the TOC items
            // or in LBA +- 1

            // get TOC items
            var tocItems = disc.TOC.TOCItems.Where(a => a.Exists == true && a.IsData == true).ToList();

            // iterate through each LBA specified in the TOC and search for system string            
            foreach (var item in tocItems)
            {
                int lb = item.LBA;
                int lbaPlus1 = item.LBA + 1;
                int lbaMinus1 = item.LBA - 1;

                try
                {
                    List<string> datas = new List<string>();

                    byte[] data = di.ReadData(lb, 2048);
                    datas.Add(System.Text.Encoding.Default.GetString(data));

                    byte[] data1 = di.ReadData(lbaPlus1, 2048);
                    datas.Add(System.Text.Encoding.Default.GetString(data1));

                    byte[] data2 = di.ReadData(lbaMinus1, 2048);
                    datas.Add(System.Text.Encoding.Default.GetString(data2));

                    // iterate through each string
                    foreach (string sS in datas)
                    {
                        if (sS.ToLower().Contains("pc-fx"))
                        {
                            byte[] newData = System.Text.Encoding.ASCII.GetBytes(sS);

                            if (sS.ToLower().StartsWith("pc-fx:hu_cd"))
                            {
                                // disc format does not have a gametitle
                            }
                            else
                            {
                                // game title should exist
                                byte[] dataSm = newData.Skip(106).Take(48).ToArray();
                                string t = System.Text.Encoding.Default.GetString(dataSm).Replace('\0', ' ').Trim().Split(new string[] { "  " }, StringSplitOptions.None).FirstOrDefault();
                                Data.GameTitle = t;
                            }
                            return true;
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    string s = ex.ToString();
                    continue;
                }

            }

            // no pcfx detected
           
            return false;
            
        }
    }
}
