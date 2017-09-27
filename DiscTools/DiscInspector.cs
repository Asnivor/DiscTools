using System;
using System.Collections.Generic;
using System.Text;
using DiscTools.ISO;
using System.Linq;
using System.IO;
using DiscTools.Objects;

namespace DiscTools
{
    public class DiscInspector
    {
        public string CuePath { get; private set; }
        public DiscData Data { get; private set; }
        public DiscType DiscType { get; private set; }
        public string DiscTypeString { get; private set; }

        private Disc disc;
        private EDiscStreamView discView;
        private ISOFile iso;
        private DiscIdentifier di;
        private int PsxLba;

        public DiscInspector(string cuePath)
        {
            if (!File.Exists(cuePath))
                return;

            CuePath = cuePath;
            iso = new ISOFile();
            PsxLba = 23;

            // load the disc
            disc = Disc.LoadAutomagic(CuePath);

            if (disc == null)
                return;

            Data = new DiscData();

            // detect disc mode
            discView = EDiscStreamView.DiscStreamView_Mode1_2048;
            if (disc.TOC.Session1Format == SessionFormat.Type20_CDXA)
                discView = EDiscStreamView.DiscStreamView_Mode2_Form1_2048;

            di = new DiscIdentifier(disc);

            // identify disc type
            DiscType = di.DetectDiscType();

            if (DiscType != DiscType.SegaSaturn ||
                DiscType != DiscType.SonyPSP ||
                DiscType != DiscType.SonyPSX ||
                DiscType != DiscType.MegaCD)
            {
                // disc wasnt detected. Maybe audio at track one or something else. Use secondary method
                GetDiscTypeSecondary();
            }

            // populate basic disc data
            int dataTracks = 0;
            int audioTracks = 0;

            foreach (var t in disc.Structure.Sessions.Where(a => a != null))
            {
                for (int i = 0; i < t.Tracks.Count(); i++)
                {
                    // skip leadin
                    if (i == 0)
                        continue;
                    // skip leadout
                    if (i == t.Tracks.Count() - 1)
                        continue;

                    if (t.Tracks[i].IsData == true)
                    {
                        dataTracks++;
                        continue;
                    }

                    if (t.Tracks[i].IsAudio == true)
                    {
                        audioTracks++;
                        continue;
                    }
                }
            }

            Data.TotalAudioTracks = audioTracks;
            Data.TotalDataTracks = dataTracks;
            Data.TotalTracks = audioTracks + dataTracks;

            bool isIso = iso.Parse(new DiscStream(disc, discView, 0));

            if (isIso)
            {
                Data.GameTitle = System.Text.Encoding.Default.GetString(iso.VolumeDescriptors.Where(a => a != null).First().VolumeIdentifier).Trim();
                var vs = iso.VolumeDescriptors.Where(a => a != null).ToArray().First();
                Data.CreationDateTime = ParseDiscDateTime(TruncateLongString(System.Text.Encoding.Default.GetString(vs.VolumeCreationDateTime.ToArray()).Trim(), 12));
                Data.ModificationDateTime = ParseDiscDateTime(TruncateLongString(System.Text.Encoding.Default.GetString(vs.LastModifiedDateTime.ToArray()).Trim(), 12));

                if (DiscType == DiscType.SonyPSX)
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
                        PsxLba = 23;
                    }
                    else
                    {
                        PsxLba = Convert.ToInt32(ifn.Offset);
                    }
                }
            }

            // get information based on disc type
            switch (DiscType)
            {
                case DiscType.SegaSaturn:
                    GetSaturnInfo();
                    break;
                case DiscType.MegaCD:
                    GetMegaCDInfo();
                    break;
                case DiscType.SonyPSX:
                    GetPSXInfo();
                    break;
                case DiscType.TurboCD:
                    break;
                default:
                    break;
            }

            // set the typeString
            DiscTypeString = DiscType.ToString();
            if (DiscTypeString.ToLower() == "turbocd")
                DiscTypeString = "PC-Engine";
        }

        public void GetSaturnInfo()
        {
            // read 2048 bytes of data from lba 0 (as saturn info is in the header)
            byte[] d = di.ReadData(0, 2048);

            string temp = System.Text.Encoding.Default.GetString(d.ToList().Skip(0).Take(1024).ToArray());

            // read the info
            Data.ManufacturerID = System.Text.Encoding.Default.GetString(d.ToList().Skip(16).Take(16).ToArray()).Trim();
            Data.SerialNumber = System.Text.Encoding.Default.GetString(d.ToList().Skip(32).Take(9).ToArray()).Trim();
            Data.Version = System.Text.Encoding.Default.GetString(d.ToList().Skip(41).Take(7).ToArray()).Trim();
            Data.InternalDate = System.Text.Encoding.Default.GetString(d.ToList().Skip(48).Take(8).ToArray()).Trim();
            Data.DeviceInformation = System.Text.Encoding.Default.GetString(d.ToList().Skip(56).Take(8).ToArray()).Trim();
            Data.AreaCodes = System.Text.Encoding.Default.GetString(d.ToList().Skip(64).Take(16).ToArray()).Trim();
            Data.PeripheralCodes = System.Text.Encoding.Default.GetString(d.ToList().Skip(80).Take(8).ToArray()).Trim(); // saturn docs show this area as 10 bytes, but it looks like its actually 8
            Data.GameTitle = System.Text.Encoding.Default.GetString(d.ToList().Skip(86).Take(120).ToArray()).Trim();
        }

        public void GetMegaCDInfo()
        {
            // read 2048 bytes of data from lba 0 (as MegaCD info is in the header)
            byte[] d = di.ReadData(0, 2048);

            List<string> header = new List<string>();

            for (int i = 0; i < 100; i++)
            {
                string l = System.Text.Encoding.ASCII.GetString(d.ToList().Skip(i * 16).Take(16).ToArray());
                header.Add(l);
            }

            Data.ManufacturerID = header[16].Trim();
            Data.GameTitle = (header[18] + header[19]).Trim();
            Data.SerialNumber = header[24].Trim();
            Data.AreaCodes = header[31].Trim();
            Data.PeripheralCodes = header[25].Trim();
            Data.InternalDate = header[5].Trim();
            Data.DeviceInformation = header[0].Trim();
            Data.OtherData = header[17].Trim();
            
        }

        public DiscType GetDiscTypeSecondary()
        {
            // get TOC
            var tocItems = disc.TOC.TOCItems.Where(a => a.Exists == true && a.IsData == true).ToList();

            // iterate through each LBA specified in the TOC and search for system string
            int lb = 0;

            /*
            for (int i = 0; i < 2000000; i++)
            {
                lb = i;
                byte[] da = di.ReadData(lb, 2048);
                string s = System.Text.Encoding.Default.GetString(da);
                if (s.ToLower().Contains("Angelique"))
                {

                }
            }
            */
            

            foreach (var item in tocItems)
            {
                lb = item.LBA;
                int lbaPlus1 = item.LBA + 1;
                int lbaMinus1 = item.LBA - 1;
                  
                try
                {
                    // sometimes the identifier string can be found in lba+1 or lba-1 as well
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
                            DiscType = DiscType.PCFX;

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

                            
                            return DiscType;
                        }

                        if (sS.ToLower().Contains("pc engine"))
                        {
                            DiscType = DiscType.TurboCD;

                            byte[] newData = System.Text.Encoding.ASCII.GetBytes(sS);

                            // get game name
                            byte[] dataSm = newData.Skip(106).Take(48).ToArray();
                            string t = System.Text.Encoding.Default.GetString(dataSm).Replace('\0', ' ').Trim().Split(new string[] { "  " }, StringSplitOptions.None).FirstOrDefault();
                            Data.GameTitle = t;
                            return DiscType;
                        }
                        
                        if (sS.ToLower().Contains("sony computer"))
                        {
                            DiscType = DiscType.SonyPSX;
                            break;
                        }

                        if (sS.ToLower().Contains("segasaturn"))
                        {
                            DiscType = DiscType.SegaSaturn;
                            break;
                        }
                    }                    
                }
                catch (InvalidOperationException ex)
                {
                    string s = ex.ToString();
                    continue;
                }
                
            }

            
            return DiscType.UnknownFormat;
        }

        public void GetPSXInfo()
        {
            byte[] data = di.GetPSXSerialNumber(PsxLba);
            byte[] data32 = data.ToList().Take(200).ToArray();

            string sS = System.Text.Encoding.Default.GetString(data32);

            if (!sS.Contains("cdrom:"))           
                return;

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

            Data.SerialNumber = serial;

        }

        private string TruncateLongString(string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        private DateTime? ParseDiscDateTime(string dtString)
        {
            if (dtString.Contains("0000000"))
                return null;
            try
            {
                DateTime dt = DateTime.ParseExact(dtString, "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                return dt;
            }
            catch
            {
                return null;
            }
        }
    }
}
