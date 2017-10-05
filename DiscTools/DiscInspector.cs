using System;
using System.Collections.Generic;
using System.Text;
using DiscTools.ISO;
using System.Linq;
using System.IO;
using DiscTools.Objects;

namespace DiscTools
{    
    public partial class DiscInspector
    {
        public string CuePath { get; set; }
        public DiscData Data { get; set; }
        public DetectedDiscType DetectedDiscType { get; set; }
        public string DiscTypeString { get; set; }
        public string DiscViewString { get; set; }

        private Disc disc;
        private EDiscStreamView discView;
        public ISOFile iso;
        public DiscIdentifier di;
        private int CurrentLBA;
        private bool isIso;
        private bool IntensiveScanning = true;

        public DiscInspector()
        {   
        }

        public DiscInspector Scan()
        {
            if (CuePath == null)
                return this;

            DetectedDiscType = DetectedDiscType.UnknownFormat;

            if (!File.Exists(CuePath))
                return this;

            iso = new ISOFile();
            CurrentLBA = 23;

            // load the disc
            try
            {
                disc = Disc.LoadAutomagic(CuePath);
            }

            catch { return this; }


            if (disc == null)
                return this;

            Data = new DiscData();

            // detect disc mode
            discView = EDiscStreamView.DiscStreamView_Mode1_2048;
            if (disc.TOC.Session1Format == SessionFormat.Type20_CDXA)
                discView = EDiscStreamView.DiscStreamView_Mode2_Form1_2048;

            // try and mount it as an ISO
            isIso = iso.Parse(new DiscStream(disc, discView, 0));

            // Process disc
            DetectedDiscType = ProcessDisc();

            // populate basic disc data
            int dataTracks = 0;
            int audioTracks = 0;

            foreach (var t in disc.Structure.Sessions.Where(a => a != null))
            {
                for (int i = 0; i < t.Tracks.Count(); i++)
                {
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

            if (DetectedDiscType == DetectedDiscType.UnknownFormat)
            {
                var test = di.DetectDiscType();
                if (test == DiscType.AudioDisc)
                    DetectedDiscType = DetectedDiscType.AudioCD;
            }

            DiscTypeString = DetectedDiscType.ToString();

            return this;
        }

        private void InitProcess()
        {
            if (CuePath == null)
                return;

            DetectedDiscType = DetectedDiscType.UnknownFormat;

            if (!File.Exists(CuePath))
                return;

            iso = new ISOFile();
            CurrentLBA = 23;

            // load the disc
            try
            {
                disc = Disc.LoadAutomagic(CuePath);
            }

            catch { return; }


            if (disc == null)
                return;

            

            Data = new DiscData();

            // detect disc mode
            discView = EDiscStreamView.DiscStreamView_Mode1_2048;
            if (disc.TOC.Session1Format == SessionFormat.Type20_CDXA)
                discView = EDiscStreamView.DiscStreamView_Mode2_Form1_2048;

            di = new DiscIdentifier(disc);

            // try and mount it as an ISO
            isIso = iso.Parse(new DiscStream(disc, discView, 0));

            // populate basic disc data
            int dataTracks = 0;
            int audioTracks = 0;

            foreach (var t in disc.Structure.Sessions.Where(a => a != null))
            {
                for (int i = 0; i < t.Tracks.Count(); i++)
                {
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

            DiscTypeString = DetectedDiscType.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DetectedDiscType ProcessDisc()
        {
            di = new DiscIdentifier(disc);

            // check whether disc is ISO format and process this first (as it is more accurate for certain systems)
            if (isIso == true)
            {
                // take only the first volume descriptor (all the discs Ive seen so far that have multiple - anything after the first is null values)
                var vs = iso.VolumeDescriptors.Where(a => a != null).ToArray().First();

                // translate the vd
                Data.ISOData = PopulateISOData(vs);
                Data.ISOData.ISOFiles = iso.Root.Children;
                ISONode ifn = null;

                /* PSX */                
                if (Data.ISOData.ApplicationIdentifier == "PLAYSTATION")
                {
                    // store lba for SYSTEM.CNF
                    var cnf = Data.ISOData.ISOFiles.Where(a => a.Key.Contains("SYSTEM.CNF")).FirstOrDefault();
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

                    if (GetPSXInfo())
                        return DetectedDiscType.SonyPSX;
                }

                /* Saturn */
                if (Data.ISOData.SystemIdentifier.Contains("SEGA SEGASATURN"))
                {
                    if (GetSaturnInfo())
                        return DetectedDiscType.SegaSaturn;
                    /*
                    var ipBin = Data.ISOData.ISOFiles.Where(a => a.Key.Contains("IP.BIN")).FirstOrDefault();
                    ifn = ipBin.Value;
                    CurrentLBA = Convert.ToInt32(ifn.Offset);
                    if (GetDreamcastInfo())
                        return DetectedDiscType.DreamCast;
                        */
                }


                /* PSP */
                if (Data.ISOData.ApplicationIdentifier == "PSP GAME")
                {
                    // still to do - getpspinfo
                    return DetectedDiscType.SonyPSP;
                }


                /* DreamCast */
                if (Data.ISOData.SystemIdentifier.Contains("SEGAKATANA"))
                {
                    var ipBin = Data.ISOData.ISOFiles.Where(a => a.Key.Contains("IP.BIN")).FirstOrDefault();
                    ifn = ipBin.Value;
                    CurrentLBA = Convert.ToInt32(ifn.Offset);
                    if (GetDreamcastInfo())
                        return DetectedDiscType.DreamCast;
                }
               

                /* MegaCD */
                if (Data.ISOData.SystemIdentifier.Contains("MEGA_CD"))
                {                    
                    if (GetMegaCDInfo())
                        return DetectedDiscType.SegaCD;
                }

                /* NeoGeo CD */             
                var absTxt = Data.ISOData.ISOFiles.Where(a => a.Key.Contains("ABS.TXT")).FirstOrDefault();
                ifn = absTxt.Value;
                if (ifn != null)
                {
                    CurrentLBA = Convert.ToInt32(ifn.Offset);
                    if (GetNeoGeoCDInfo())
                        return DetectedDiscType.NeoGeoCD;
                }

                /* Saturn */
                // Currently no found lookup implementation using ISO

                /* PCECD */
                // Havent found an ISO format pcecd image yet

                /* PC-FX */
                // Havent found an ISO format pcfx image yet

                /* Philips CD-i */
                // Havent found an ISO format CD-i image yet

                /* CD32 */
                if (Data.ISOData.SystemIdentifier.Contains("CDTV") || Data.ISOData.SystemIdentifier.Contains("AMIGA"))
                {
                    // its either CDTV or CD32
                    foreach (var child in Data.ISOData.ISOFiles)
                    {
                        if (child.Key.ToLower().Contains("cd32"))
                            return DetectedDiscType.AmigaCD32;
                        else
                            return DetectedDiscType.AmigaCDTV;
                    }
                    
                }

                /* Playdia */
                if (Data.ISOData.SystemIdentifier.Contains("ASAHI-CDV"))
                {
                    return DetectedDiscType.BandaiPlaydia;

                }
            }

            // ISO-related checks completed
            // ON TO NON-ISO LOOKUPS

            // Do quick checks where we know (or can find) the LBA already

            /* Saturn */
            bool satTest = StringAt("SEGA SEGASATURN", 0);
            if (satTest)
            {
                if (GetSaturnInfo())
                    return DetectedDiscType.SegaSaturn;
            }

            /* Sega CD */
            bool segacdTest = StringAt("SEGADISCSYSTEM", 0) || StringAt("SEGADISCSYSTEM", 16);
            if (segacdTest)
            {
                if (GetMegaCDInfo())
                    return DetectedDiscType.SegaCD;
            }      

            /* Dreamcast */
            //GetDreamcastInfo();
            /*
            bool dreamTest = StringAt("SEGAKATANA", 0);
            if (dreamTest)
            {
                CurrentLBA = 0;
                if (GetDreamcastInfo())
                {
                    return DetectedDiscType.DreamCast;
                }
            }
            */
            

            // Now looping through TOCs to try and identify LBAs
            var tocItems = disc.TOC.TOCItems.Where(a => a.Exists == true && a.IsData == true).ToList(); //&& a.IsData == true).ToList();
            for (int i = 0; i < tocItems.Count(); i++)
            {
                // we are going to check LBA +- 1 as some systems (pcfx) has some weird stuff going on that I havent been able to work out
                int lb = tocItems[0].LBA;
                int lbaPlus1 = tocItems[0].LBA + 1;
                //int lbaMinus1 = tocItems[0].LBA - 1;

                try
                {
                    List<string> datas = new List<string>();

                    byte[] data = di.ReadData(lb, 2048);
                    datas.Add(System.Text.Encoding.Default.GetString(data));

                    byte[] data1 = di.ReadData(lbaPlus1, 2048);
                    datas.Add(System.Text.Encoding.Default.GetString(data1));

                    /*
                    if (lbaMinus1 >= 0)
                    {
                        byte[] data2 = di.ReadData(lbaMinus1, 2048);
                        datas.Add(System.Text.Encoding.Default.GetString(data2));
                    }  
                    */

                    // iterate through each string
                    int strCount = 0;
                    foreach (string sS in datas)
                    {
                        strCount++;
                        /* PCFX */
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
                            return DetectedDiscType.PCFX;
                        }


                        /* PC Engine CD */
                        if (sS.ToLower().Contains("pc engine") && !sS.ToLower().Contains("pc-fx"))
                        {
                            byte[] newData = System.Text.Encoding.ASCII.GetBytes(sS);

                            // get game name
                            byte[] dataSm = newData.Skip(106).Take(48).ToArray();
                            string t = System.Text.Encoding.Default.GetString(dataSm).Replace('\0', ' ').Trim().Split(new string[] { "  " }, StringSplitOptions.None).FirstOrDefault();
                            Data.GameTitle = t;
                            return DetectedDiscType.PCEngineCD;
                        }


                        /* NeoGeo CD */
                        if (sS.ToLower().Contains("abstracted by snk"))
                        {
                            return DetectedDiscType.NeoGeoCD;
                        }


                        /* PSX */
                        CurrentLBA = 23;
                        if (GetPSXInfo())
                            return DetectedDiscType.SonyPSX;

                        /* 3DO */
                        if (sS.ToLower().Contains("iamaduckiamaduck"))
                        {
                            return DetectedDiscType.Panasonic3DO;
                        }

                        /* CD-32 */
                        if (sS.ToLower().Contains("cdtv"))
                        {
                            return DetectedDiscType.AmigaCDTV;
                        }
                           
                    }
                }
                catch (InvalidOperationException ex)
                {
                    string s = ex.ToString();
                    continue;
                }
            }


            // now misc detection

            /* CD-i */
            if (GetCDiInfo())
                return DetectedDiscType.PhilipsCDi;

            /* Dreamcast */
            if (IntensiveScanning == true)
            {
                if (GetDreamcastInfo())
                    return DetectedDiscType.DreamCast;
            }

            /* cdtv */
            // some discs appear to be formatted weirdly.
            for (int i = 0; i < 1000; i++)
            {
                byte[] data = di.ReadData(i, 2048);
                string cdtv = System.Text.Encoding.Default.GetString(data);
                if (cdtv.ToLower().Contains("amiga"))
                    return DetectedDiscType.AmigaCDTV;

                if (cdtv.ToLower().Contains("asahi-cdv"))
                    return DetectedDiscType.BandaiPlaydia;
            }

            


            // attempt to identify disc type using BizHawk DiscSystem if type still unknown
            if (DetectedDiscType == DetectedDiscType.UnknownFormat)
            {
                var dt = di.DetectDiscType();

                switch (dt)
                {
                    case DiscType.SegaSaturn: GetSaturnInfo(); return DetectedDiscType.SegaSaturn;
                    case DiscType.SonyPSX: GetPSXInfo(); return DetectedDiscType.SonyPSX;
                    case DiscType.MegaCD: GetMegaCDInfo(); return DetectedDiscType.SegaCD;
                    case DiscType.UnknownCDFS: return DetectedDiscType.UnknownCDFS;
                }
            }

            return DetectedDiscType.UnknownFormat;
        }
        

        private static string TruncateLongString(string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        private static DateTime? ParseDiscDateTime(string dtString)
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

        private readonly Dictionary<int, byte[]> _sectorCache = new Dictionary<int, byte[]>();

        private byte[] ReadSectorCached(int lba)
        {
            //read it if we dont have it cached
            //we wont be caching very much here, it's no big deal
            //identification is not something we want to take a long time
            byte[] data;
            if (!_sectorCache.TryGetValue(lba, out data))
            {
                data = new byte[2048];
                int read = di.dsr.ReadLBA_2048(lba, data, 0);
                if (read != 2048)
                    return null;
                _sectorCache[lba] = data;
            }
            return data;
        }

        private bool StringAt(string s, int n, int lba = 0)
        {
            var data = ReadSectorCached(lba);
            if (data == null) return false;
            byte[] cmp = System.Text.Encoding.ASCII.GetBytes(s);
            byte[] cmp2 = new byte[cmp.Length];
            Buffer.BlockCopy(data, n, cmp2, 0, cmp.Length);
            return System.Linq.Enumerable.SequenceEqual(cmp, cmp2);
        }

        private static ISOData PopulateISOData(ISOVolumeDescriptor vd)
        {
            ISOData i = new ISOData();

            // strings
            i.AbstractFileIdentifier = System.Text.Encoding.Default.GetString(vd.AbstractFileIdentifier).TrimEnd('\0', ' ');
            i.ApplicationIdentifier = System.Text.Encoding.Default.GetString(vd.ApplicationIdentifier).TrimEnd('\0', ' ');
            i.BibliographicalFileIdentifier = System.Text.Encoding.Default.GetString(vd.BibliographicalFileIdentifier).TrimEnd('\0', ' ');
            i.CopyrightFileIdentifier = System.Text.Encoding.Default.GetString(vd.CopyrightFileIdentifier).TrimEnd('\0', ' ');
            i.DataPreparerIdentifier = System.Text.Encoding.Default.GetString(vd.DataPreparerIdentifier).TrimEnd('\0', ' ');
            i.PublisherIdentifier = System.Text.Encoding.Default.GetString(vd.PublisherIdentifier).TrimEnd('\0', ' ');
            i.Reserved = System.Text.Encoding.Default.GetString(vd.Reserved).Trim('\0');
            i.SystemIdentifier = System.Text.Encoding.Default.GetString(vd.SystemIdentifier).TrimEnd('\0', ' ');
            i.VolumeIdentifier = System.Text.Encoding.Default.GetString(vd.VolumeIdentifier).TrimEnd('\0', ' ');
            i.VolumeSetIdentifier = System.Text.Encoding.Default.GetString(vd.VolumeSetIdentifier).TrimEnd('\0', ' ');

            // ints
            i.NumberOfSectors = vd.NumberOfSectors;
            i.PathTableSize = vd.PathTableSize;
            i.SectorSize = vd.SectorSize;
            i.Type = vd.Type;
            i.VolumeSequenceNumber = vd.VolumeSequenceNumber;

            // datetimes
            i.EffectiveDateTime = ParseDiscDateTime(TruncateLongString(System.Text.Encoding.Default.GetString(vd.EffectiveDateTime.ToArray()).Trim(), 12));
            i.ExpirationDateTime = ParseDiscDateTime(TruncateLongString(System.Text.Encoding.Default.GetString(vd.ExpirationDateTime.ToArray()).Trim(), 12));
            i.LastModifiedDateTime = ParseDiscDateTime(TruncateLongString(System.Text.Encoding.Default.GetString(vd.LastModifiedDateTime.ToArray()).Trim(), 12));
            i.VolumeCreationDate = ParseDiscDateTime(TruncateLongString(System.Text.Encoding.Default.GetString(vd.VolumeCreationDateTime.ToArray()).Trim(), 12));
            
            // other
            i.RootDirectoryRecord = vd.RootDirectoryRecord;

            return i;
        }

        /// <summary>
        /// Return a DiscInspector Object
        /// IntensiveScan will return more matches but may take longer
        /// </summary>
        /// <param name="cuePath"></param>
        /// <param name="IntensiveScan"></param>
        public static DiscInspector ScanDisc(string cuePath, bool IntensiveScan)
        {
            var DI = new DiscInspector();
            DI.CuePath = cuePath;
            DI.IntensiveScanning = IntensiveScan;

            var res = DI.Scan();

            if (res == null || res.DetectedDiscType == DetectedDiscType.UnknownFormat || res.DetectedDiscType == DetectedDiscType.UnknownCDFS)
            {
                string newCue = CueHandler.ParseCue(cuePath);
                DI.CuePath = newCue;
                res = DI.Scan();
                if (File.Exists(newCue))
                    File.Delete(newCue);
            }
            res.CuePath = cuePath;
            res.DiscViewString = DI.discView.ToString();

            return res;
        }

        public static DiscInspector ScanDiscNoCorrection(string cuePath)
        {
            var DI = new DiscInspector();
            DI.CuePath = cuePath;
            DI.IntensiveScanning = true;

            var res = DI.Scan();

            return res;
        }

        /// <summary>
        /// Return a DiscInspector Object - quick scan that may miss detection on some non-iso based images
        /// </summary>
        /// <param name="cuePath"></param>
        /// <param name="IntensiveScan"></param>
        public static DiscInspector ScanDiscQuick(string cuePath)
        {
            return ScanDisc(cuePath, false);
        }

        /// <summary>
        /// Return a DiscInspector Object - Intensive scan that has more chance of detection (but may take longer)
        /// </summary>
        /// <param name="cuePath"></param>
        /// <param name="IntensiveScan"></param>
        public static DiscInspector ScanDisc(string cuePath)
        {
            return ScanDisc(cuePath, true);
        }

        public static void UnknownTest()
        {
            string path = @"G:\_Emulation\Commadore Amiga\cd32\Zool - Ninja of the 'Nth' Dimension (1993)(Gremlin)[!].cue";
            var DISC = new DiscInspector();
            DISC.CuePath = path;
            DISC.IntensiveScanning = true;
            DISC.InitProcess();

            var tocItems = DISC.disc.TOC.TOCItems.Where(a => a.Exists == true && a.IsData == true).ToList();
            foreach (var item in tocItems)
            {
                int lba = item.LBA;
                int lbam1 = item.LBA - 1;
                int lbap1 = item.LBA + 1;

                byte[] data = DISC.di.ReadData(lba, 2048);
                string t1 = System.Text.Encoding.Default.GetString(data);

                if (t1.Contains("PC Engine"))
                {
                    break;
                }

                byte[] datam1 = DISC.di.ReadData(lbam1, 2048);
                string t2 = System.Text.Encoding.Default.GetString(datam1);

                if (t2.Contains("PC Engine"))
                {
                    break;
                }

                byte[] datap1 = DISC.di.ReadData(lbap1, 2048);
                string t3 = System.Text.Encoding.Default.GetString(datap1);

                if (t3.Contains("PC Engine"))
                {
                    break;
                }

            }
        }


        public static void test()
        {

            List<string> allFiles = System.IO.Directory.GetFiles(@"G:\_Emulation\PC Engine\discs", "*.*", System.IO.SearchOption.AllDirectories)
                .Where(a => System.IO.Path.GetExtension(a).ToLower() == ".cue" ||
                System.IO.Path.GetExtension(a).ToLower() == ".ccd" ||
                System.IO.Path.GetExtension(a).ToLower() == ".toc").ToList();

            foreach (var file in allFiles)
            {
                var DISC = new DiscInspector();
                DISC.CuePath = file;
                DISC.IntensiveScanning = true;
                DISC.InitProcess();

                var ident = DISC.di.DetectDiscType();

                
                var tocItems = DISC.disc.TOC.TOCItems.Where(a => a.Exists == true && a.IsData == true).ToList();
                foreach (var item in tocItems)
                {
                    int lba = item.LBA;
                    int lbam1 = item.LBA - 1;
                    int lbap1 = item.LBA + 1;

                    byte[] data = DISC.di.ReadData(lba, 2048);
                    string t1 = System.Text.Encoding.Default.GetString(data);

                    if (t1.Contains("PC Engine"))
                    {
                        break;
                    }

                    byte[] datam1 = DISC.di.ReadData(lbam1, 2048);
                    string t2 = System.Text.Encoding.Default.GetString(datam1);

                    if (t2.Contains("PC Engine"))
                    {
                        break;
                    }

                    byte[] datap1 = DISC.di.ReadData(lbap1, 2048);
                    string t3 = System.Text.Encoding.Default.GetString(datap1);

                    if (t3.Contains("PC Engine"))
                    {
                        break;
                    }
                }

                string no = "nothing";

  
            }

            
        }
    }

    public enum DetectedDiscType
    {
        SonyPSX,
        SonyPSP,
        SegaSaturn,
        PCEngineCD,
        PCFX,
        SegaCD,
        PhilipsCDi,
        AudioCD,
        NeoGeoCD,
        DreamCast,
        UnknownCDFS,
        UnknownFormat,
        Panasonic3DO,
        AmigaCDTV,
        AmigaCD32,
        BandaiPlaydia
    }

}
