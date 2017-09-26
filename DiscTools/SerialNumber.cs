using System;
using System.Collections.Generic;
using System.Text;
using DiscTools.ISO;
using System.Linq;
using System.IO;
using DiscTools.Objects;

namespace DiscTools
{
    public class SerialNumber
    {
        public static string GetSaturnSerial(string cuePath)
        {
            if (!File.Exists(cuePath))
            {
                return "";
            }

            int lba = 0;
            Disc disc = Disc.LoadAutomagic(cuePath);

            if (disc == null)
            {
                // unable to mount disc - return null
                return "";
            }

            var discView = EDiscStreamView.DiscStreamView_Mode1_2048;
            if (disc.TOC.Session1Format == SessionFormat.Type20_CDXA)
                discView = EDiscStreamView.DiscStreamView_Mode2_Form1_2048;

            var iso = new ISOFile();
            bool isIso = iso.Parse(new DiscStream(disc, discView, 0));

            /*
            if (isIso)
            {
                var appId = System.Text.Encoding.ASCII.GetString(iso.VolumeDescriptors[0].ApplicationIdentifier).TrimEnd('\0', ' ');

                var desc = iso.Root.Children;

                ISONode ifn = null;

                foreach (var i in desc)
                {
                    if (i.Key.Contains("SYSTEM.CNF"))
                        ifn = i.Value;
                }

                if (ifn == null)
                {
                    lba = 23;
                }
                else
                {
                    lba = Convert.ToInt32(ifn.Offset);
                }
            }
            else
            {
                lba = 23;
            }

            */

            DiscIdentifier di = new DiscIdentifier(disc);

            // start at lba 0 (saturn header information is usually there)
            byte[] data = di.GetPSXSerialNumber(lba);

            Dictionary<int, string> sDict = new Dictionary<int, string>();

            DiscData sd = new DiscData();
            
            for (int i = 0; i < 8; i++)
            {
                int blockSize = 32;
                byte[] data32 = data.ToList().Skip(i * (blockSize)).Take(blockSize).ToArray();
                string sS = System.Text.Encoding.Default.GetString(data32);
                sDict.Add(i, sS);
            }

            

            


            return "";
        }

        /// <summary>
        /// returns the PSX serial - Bizhawk DiscSystem requires either cue, ccd or iso (not bin or img)
        /// </summary>
        /// <param name="cuePath"></param>
        /// <returns></returns>
        public static string GetPSXSerial(string cuePath)
        {
            if (!File.Exists(cuePath))
            {
                return "";
            }

            int lba = 23;
            Disc disc = Disc.LoadAutomagic(cuePath);

            if (disc == null)
            {
                // unable to mount disc - return null
                return "";
            }

            var discView = EDiscStreamView.DiscStreamView_Mode1_2048;
            if (disc.TOC.Session1Format == SessionFormat.Type20_CDXA)
                discView = EDiscStreamView.DiscStreamView_Mode2_Form1_2048;

            var iso = new ISOFile();
            bool isIso = iso.Parse(new DiscStream(disc, discView, 0));

            if (isIso)
            {
                var appId = System.Text.Encoding.ASCII.GetString(iso.VolumeDescriptors[0].ApplicationIdentifier).TrimEnd('\0', ' ');

                var desc = iso.Root.Children;

                ISONode ifn = null;

                foreach (var i in desc)
                {
                    if (i.Key.Contains("SYSTEM.CNF"))
                        ifn = i.Value;
                }

                if (ifn == null)
                {
                    lba = 23;
                }
                else
                {
                    lba = Convert.ToInt32(ifn.Offset);
                }
            }
            else
            {
                lba = 23;
            }


            DiscIdentifier di = new DiscIdentifier(disc);

            // start by checking sector 23 (as most discs seem to have system.cfg there
            byte[] data = di.GetPSXSerialNumber(lba);
            // take first 32 bytes
            byte[] data32 = data.ToList().Take(46).ToArray();

            string sS = System.Text.Encoding.Default.GetString(data32);

            if (!sS.Contains("cdrom:"))
            {
                return null;
            }

            // get the actual serial number from the returned string
            string[] arr = sS.Split(new string[] { "cdrom:" }, StringSplitOptions.None);
            string[] arr2 = arr[1].Split(new string[] { ";1" }, StringSplitOptions.None);
            string serial = arr2[0].Replace("_", "-").Replace(".", "");
            if (serial.Contains("\\"))
                serial = serial.Split('\\').Last();
            else
                serial = serial.TrimStart('\\').TrimStart('\\');

            // try and remove any nonsense after the serial
            string[] sarr2 = serial.Split('\r');
            if (sarr2.Length > 1)
                serial = sarr2.First();

            return serial;
        }

        /*
        public static string GetSaturnSerial(string cuePath)
        {
            if (!File.Exists(cuePath))
                return "";

            // set start position
            int pos = 16;
            // set read length
            int required = 16;

            List<string> str = new List<string>();

            while (pos < required * 13)
            {
                byte[] by = new byte[required];

                using (BinaryReader b = new BinaryReader(File.Open(cuePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    try
                    {
                        // seek to required position
                        b.BaseStream.Seek(pos, SeekOrigin.Begin);

                        // Read the required bytes into a bytearray
                        by = b.ReadBytes(required);
                    }
                    catch
                    {

                    }
                    pos += required;
                }
                // convert byte array to string
                str.Add(System.Text.Encoding.Default.GetString(by));
            }
            string[] spline = str[2].Split(' ');

            if (spline.Length > 1)
            {
               return spline[0].Trim();
            }

            return "";
        }
        */
    }
}
