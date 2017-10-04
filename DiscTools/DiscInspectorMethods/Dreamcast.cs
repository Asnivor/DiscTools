using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DiscTools
{
    public partial class DiscInspector
    {
        public static DiscInspector ScanDreamcast(string cuePath)
        {
            var DISC = new DiscInspector();
            DISC.CuePath = cuePath;
            DISC.IntensiveScanning = true;
            DISC.InitProcess();

            if (DISC.disc == null || DISC.DetectedDiscType == DetectedDiscType.UnknownFormat || DISC.DetectedDiscType == DetectedDiscType.UnknownCDFS)
            {
                string newCue = CueHandler.ParseCue(cuePath);
                DISC.CuePath = newCue;
                DISC.InitProcess();
                if (System.IO.File.Exists(newCue))
                    System.IO.File.Delete(newCue);
                DISC.CuePath = cuePath;
            }

            if (DISC.isIso == true)
            {
                // take only the first volume descriptor (all the discs Ive seen so far that have multiple - anything after the first is null values)
                var vs = DISC.iso.VolumeDescriptors.Where(a => a != null).ToArray().First();
                ISO.ISONode ifn = null;


                // translate the vd
                DISC.Data.ISOData = PopulateISOData(vs);
                DISC.Data.ISOData.ISOFiles = DISC.iso.Root.Children;

                if (DISC.Data.ISOData.SystemIdentifier.Contains("SEGAKATANA"))
                {
                    var ipBin = DISC.Data.ISOData.ISOFiles.Where(a => a.Key.Contains("IP.BIN")).FirstOrDefault();
                    ifn = ipBin.Value;
                    DISC.CurrentLBA = Convert.ToInt32(ifn.Offset);
                    if (DISC.GetDreamcastInfo())
                    {
                        DISC.DetectedDiscType = DetectedDiscType.DreamCast;
                        DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                        return DISC;
                    }                        
                }
            }

            // long test
            for (int it = 0; it < 20000000; it++)
            {
                byte[] data = DISC.di.ReadData(it, 2048);
                string res = System.Text.Encoding.Default.GetString(data);
                if (res.ToLower().Contains("segakatana"))
                {
                    int ind = res.ToLower().IndexOf("segakatana");
                    string d = res.Substring(res.ToLower().IndexOf("segakatana"));

                    List<string> header = new List<string>();

                    byte[] dat = System.Text.Encoding.Default.GetBytes(d);

                    for (int i = 0; i < 20; i++)
                    {
                        string lookup = System.Text.Encoding.Default.GetString(dat.Skip((i * 16) - 5).Take(16).ToArray());
                        header.Add(lookup);
                    }

                    DISC.Data.SerialNumber = header[4].Split(' ').First().Trim();
                    DISC.Data.Version = header[4].Split(' ').Last().Trim();
                    DISC.Data.GameTitle = (header[8] + header[9]).Trim();
                    DISC.Data.InternalDate = header[5].Trim();
                    DISC.Data.Publisher = header[1].Trim();
                    DISC.Data.AreaCodes = header[3].Split(' ').First().Trim();
                    DISC.Data.PeripheralCodes = header[3].Split(' ').Last().Trim();

                    DISC.Data.MediaID = header[2].Split(' ').First().Trim();
                    DISC.Data.MediaInfo = header[2].Trim().Split(' ').Last().Trim();

                    DISC.Data.DeviceInformation = header[0].Trim();
                    DISC.Data.ManufacturerID = header[7].Trim();


                    DISC.DetectedDiscType = DetectedDiscType.DreamCast;
                    DISC.DiscTypeString = DISC.DetectedDiscType.ToString();
                    return DISC;
                }
            }

            return DISC;
        }


        public bool GetDreamcastInfo()
        {
           
            if (isIso)
            {
                byte[] dat = di.ReadData(CurrentLBA, 2048);
                string test1 = System.Text.Encoding.Default.GetString(dat);
                if (test1.ToLower().Contains("segakatana"))
                {
                    List<string> header = new List<string>();
                    for (int i = 0; i < 20; i++)
                    {
                        string lookup = System.Text.Encoding.Default.GetString(dat.Skip(i * 16).Take(16).ToArray());
                        header.Add(lookup);
                    }

                    Data.SerialNumber = header[4].Split(' ').First().Trim();
                    Data.Version = header[4].Split(' ').Last().Trim();
                    Data.GameTitle = (header[8] + header[9]).Trim();
                    Data.InternalDate = header[5].Trim();
                    Data.Publisher = header[1].Trim();
                    Data.AreaCodes = header[3].Split(' ').First().Trim();
                    Data.PeripheralCodes = header[3].Split(' ').Last().Trim();

                    Data.MediaID = header[2].Split(' ').First().Trim();
                    Data.MediaInfo = header[2].Trim().Split(' ').Last().Trim();

                    Data.DeviceInformation = header[0].Trim();
                    Data.ManufacturerID = header[7].Trim();


                    return true;
                }
                return false;
            }
          

            //if (IntensiveScanning == false)
            //return false;
            /*
                       var tracks = disc.Session1.Tracks;
                       foreach (var track in tracks.Where(a => a.LBA >= 0 && a.NextTrack != null))
                       {
                           if (track.IsAudio)
                               continue;

                           int trackLength = track.NextTrack.LBA - track.LBA;
                           int startLba = track.LBA;

                           for (int sector = 0; sector < trackLength; sector++)
                           {
                               //di.dsr.ReadLBA_2048(startLba + sector, trackData, sector * 2048);
                               byte[] sData = di.ReadData(startLba + sector, 2352);
                               string txt = System.Text.Encoding.Default.GetString(sData);

                               if (txt.ToLower().Contains("segakatana"))
                               {

                               }
                           }
                       }


                       // get TOC items
                       var tocItems = disc.TOC.TOCItems.ToList();

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
                                   if (sS.ToLower().Contains("segakatana"))
                                   {
                                       byte[] newData = System.Text.Encoding.ASCII.GetBytes(sS);


                                       return true;
                                   }
                               }
                           }
                           catch (Exception ex)
                           {
                               string s = ex.ToString();
                               continue;
                           }

                       }

                       */



            // long test
            for (int it = 0; it < 20000000; it++)
            {
                byte[] data = di.ReadData(it, 2048);
                string res = System.Text.Encoding.Default.GetString(data);
                if (res.ToLower().Contains("segakatana"))
                {
                    int ind = res.ToLower().IndexOf("segakatana");
                    string d = res.Substring(res.ToLower().IndexOf("segakatana"));

                    List<string> header = new List<string>();

                    byte[] dat = System.Text.Encoding.Default.GetBytes(d);

                    for (int i = 0; i < 20; i++)
                    {
                        string lookup = System.Text.Encoding.Default.GetString(dat.Skip((i * 16) - 5).Take(16).ToArray());
                        header.Add(lookup);
                    }

                    Data.SerialNumber = header[4].Split(' ').First().Trim();
                    Data.Version = header[4].Split(' ').Last().Trim();
                    Data.GameTitle = (header[8] + header[9]).Trim();
                    Data.InternalDate = header[5].Trim();
                    Data.Publisher = header[1].Trim();
                    Data.AreaCodes = header[3].Split(' ').First().Trim();
                    Data.PeripheralCodes = header[3].Split(' ').Last().Trim();

                    Data.MediaID = header[2].Split(' ').First().Trim();
                    Data.MediaInfo = header[2].Trim().Split(' ').Last().Trim();

                    Data.DeviceInformation = header[0].Trim();
                    Data.ManufacturerID = header[7].Trim();


                    return true;
                }
            }

  

            // no dreamcast detected
            return false;

        }
    }
}
