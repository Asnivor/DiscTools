using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiscTools
{
    public partial class DiscInspector
    {
        public static DiscInspector ScanNeoGeoCD(string cuePath)
        {
            var DISC = new DiscInspector();
            DISC.CuePath = cuePath;
            DISC.IntensiveScanning = true;
            DISC.InitProcess();

            if (DISC.isIso == true)
            {
                ISO.ISONode ifn = null;

                // take only the first volume descriptor (all the discs Ive seen so far that have multiple - anything after the first is null values)
                var vs = DISC.iso.VolumeDescriptors.Where(a => a != null).ToArray().First();

                // translate the vd
                DISC.Data.ISOData = PopulateISOData(vs);
                DISC.Data.ISOData.ISOFiles = DISC.iso.Root.Children;

                var absTxt = DISC.Data.ISOData.ISOFiles.Where(a => a.Key.Contains("ABS.TXT")).FirstOrDefault();
                ifn = absTxt.Value;
                if (ifn != null)
                {
                    DISC.CurrentLBA = Convert.ToInt32(ifn.Offset);
                    if (DISC.GetNeoGeoCDInfo())
                    {
                        DISC.DetectedDiscType = DetectedDiscType.NeoGeoCD;
                        DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                        return DISC;
                    }
                        
                }
            }

            return DISC;
        }


        public bool GetNeoGeoCDInfo()
        {
            if (isIso)
            {
                // inspect currentlba to see if this is a neogeoCD
                byte[] dat = di.ReadData(CurrentLBA, 2048);
                string test1 = System.Text.Encoding.Default.GetString(dat);
                if (test1.ToLower().Contains("abstracted by snk"))
                {
                    return true;
                }
                return false;
            }

            if (IntensiveScanning == false)
                return false;

            // we havent found the identifier in the ISO, iterate through LBAs starting at 0
            for (int i = 0; i < 10000000; i++)
            {
                byte[] da = di.ReadData(i, 2048);
                string ttesties = System.Text.Encoding.Default.GetString(da);

                if (ttesties.ToLower().Contains("abstracted by snk"))
                {
                    return true;
                }
            }


            return false;
        }
    }
}
