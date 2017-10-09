using DiscTools.ISO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        /// <summary>
        /// Runs through interrogation process to identify all discs
        /// </summary>
        public DetectedDiscType InterrogateALL()
        {            
            ///////////////////////
            /* First ISO related */
            ///////////////////////

            if (isIso)
            {
                // psx
                if (ScanISOPSX())
                    return DiscSubType;

                // psp
                if (ScanISOPSP())
                    return DetectedDiscType.SonyPSP;

                // saturn
                if (ScanISOSaturn())
                    return DetectedDiscType.SegaSaturn;

                // dreamcast
                if (ScanISODreamcast())
                    return DetectedDiscType.DreamCast;

                // sega cd (megacd)
                if (ScanISOSegaCD())
                    return DetectedDiscType.SegaCD;

                // amiga
                if (ScanISOAmiga())
                {
                    return DiscSubType;
                }

                // neogeo cd
                if (ScanISONeoGeoCD())
                    return DetectedDiscType.NeoGeoCD;

                // playdia
                if (ScanISOPlaydia())
                    return DetectedDiscType.BandaiPlaydia;

                // pcecd - currently no ISO lookup method

                // pcfx - currently no ISO lookup method

                // CD-i - currently no ISO lookup method

                // gamecube - currently no ISO lookup method

                // wii - currently no ISO lookup method
            }




            /////////////////////////////////////////////////////////
            /* Non-ISO Direct queries (where we can guess the LBA) */
            /////////////////////////////////////////////////////////

            // saturn
            CurrentLBA = 0;
            currSector = di.ReadData(CurrentLBA, 2048);
            if (GetSaturnData(System.Text.Encoding.Default.GetString(currSector)))
                return DetectedDiscType.SegaSaturn;

            // amiga - but of a kludge, maybe try lba 0
            if (GetAmigaData(System.Text.Encoding.Default.GetString(currSector)))
                return DiscSubType;

            // sega cd (megacd)
            if (GetSegaCDData(System.Text.Encoding.Default.GetString(currSector)))
                return DetectedDiscType.SegaCD;

            // gamecube
            if (GetGamecubeData(System.Text.Encoding.Default.GetString(currSector)))
                return DetectedDiscType.Gamecube;

            // wii
            if (GetWiiData(System.Text.Encoding.Default.GetString(currSector)))
                return DetectedDiscType.Wii;

            CurrentLBA = 16;
            currSector = di.ReadData(CurrentLBA, 2048);
            if (GetSegaCDData(System.Text.Encoding.Default.GetString(currSector)))
                return DetectedDiscType.SegaCD;

            // CD-i - check lba 16 first - this seems to be quite common  
            if (GetCDiData(System.Text.Encoding.Default.GetString(currSector)))
                return DetectedDiscType.PhilipsCDi;

            // psx
            CurrentLBA = 23;
            if (GetPSXData())
                return DiscSubType;

            // neogeo - no direct method yet

            // playdia - no direct method yet

            // pcecd - no direct method yet

            // pcfx - no direct method yet

            // dreamcast - no direct method yet





            ///////////////////////////////////////
            /* Non-ISO looping through TOC items */
            ///////////////////////////////////////

            var tocItems = disc.TOC.TOCItems.Where(a => a.Exists == true && a.IsData == true).ToList();

            List<int> tocLBAs = new List<int>();
            // we are going to check LBA + 1 as some systems (pcfx / pcecd) have some weird stuff going on that I havent been able to work out
            // possibly I am confused as to whether bizhawk has lba starting at 0 or 1
            foreach (var item in tocItems)
            {
                tocLBAs.Add(item.LBA);
                tocLBAs.Add(item.LBA + 1);
            }

            tocLBAs = tocLBAs.Distinct().OrderBy(a => a).ToList();

            foreach (int i in tocLBAs)
            {
                CurrentLBA = i;
                byte[] data = di.ReadData(i, 2048);
                currSector = data;
                string text = System.Text.Encoding.Default.GetString(data);

                // psx
                if (GetPSXData(text))
                    return DiscSubType;

                if (GetPSPData(text))
                    return DetectedDiscType.SonyPSP;

                // saturn
                if (GetSaturnData(text))
                    return DetectedDiscType.SegaSaturn;

                // gamecube
                if (GetGamecubeData(text))
                    return DetectedDiscType.Gamecube;

                // wii
                if (GetWiiData(text))
                    return DetectedDiscType.Wii;

                // dreamcast
                if (GetDreamcastData(text))
                    return DetectedDiscType.DreamCast;

                // sega cd (megacd)
                if (GetSegaCDData(text))
                    return DetectedDiscType.SegaCD;

                // amiga
                if (GetAmigaData(text))
                    return DiscSubType;

                // neogeo
                if (GetNeoGeoCDData(text))
                    return DetectedDiscType.NeoGeoCD;

                // playdia
                if (GetPlaydiaData(text))
                    return DetectedDiscType.BandaiPlaydia;

                // CDi
                if (GetCDiData(text))
                    return DetectedDiscType.PhilipsCDi;

                // pcfx
                if (GetPCFXData(text))
                    return DetectedDiscType.PCFX;

                // pce-cd
                if (GetPCECDData(text))
                    return DetectedDiscType.PCEngineCD;

                // 3DO
                if (Get3DOData(text))
                    return DetectedDiscType.Panasonic3DO;
            }





            /////////////////////////////////
            /* Non-ISO 0-n LBA iterations  */
            /////////////////////////////////

            for (int i = 0; i < 10000; i++)
            {
                byte[] data = di.ReadData(i, 2048);
                currSector = data;
                string dataStr = System.Text.Encoding.Default.GetString(data);

                // psx
                if (GetPSXData(dataStr))
                    return DiscSubType;

                if (GetPSPData(dataStr))
                    return DetectedDiscType.SonyPSP;

                // saturn
                if (GetSaturnData(dataStr))
                    return DetectedDiscType.SegaSaturn;

                // gamecube
                if (GetGamecubeData(dataStr))
                    return DetectedDiscType.Gamecube;

                // wii
                if (GetWiiData(dataStr))
                    return DetectedDiscType.Wii;

                // dreamcast
                if (GetDreamcastData(dataStr))
                    return DetectedDiscType.DreamCast;

                // sega cd (megacd)
                if (GetSegaCDData(dataStr))
                    return DetectedDiscType.SegaCD;

                // amiga
                if (GetAmigaData(dataStr))
                    return DiscSubType;

                // neogeo
                if (GetNeoGeoCDData(dataStr))
                    return DetectedDiscType.NeoGeoCD;

                // playdia
                if (GetPlaydiaData(dataStr))
                    return DetectedDiscType.BandaiPlaydia;

                // CDi
                if (GetCDiData(dataStr))
                    return DetectedDiscType.PhilipsCDi;

                // pcfx
                if (GetPCFXData(dataStr))
                    return DetectedDiscType.PCFX;

                // pce-cd
                if (GetPCECDData(dataStr))
                    return DetectedDiscType.PCEngineCD;

                // 3DO
                if (Get3DOData(dataStr))
                    return DetectedDiscType.Panasonic3DO;
            }






            /////////////////////////////////////////
            /* Any other misc long running queries */
            /////////////////////////////////////////

            if (IntenseScan)
            {
                for (int i = 0; i < 1000000; i++)
                {
                    byte[] data = di.ReadData(i, 2048);
                    currSector = data;
                    string dataStr = System.Text.Encoding.Default.GetString(data);

                    // psx
                    if (GetPSXData(dataStr))
                        return DiscSubType;

                    if (GetPSPData(dataStr))
                        return DetectedDiscType.SonyPSP;

                    // saturn
                    if (GetSaturnData(dataStr))
                        return DetectedDiscType.SegaSaturn;

                    // gamecube
                    if (GetGamecubeData(dataStr))
                        return DetectedDiscType.Gamecube;

                    // gamecube
                    if (GetWiiData(dataStr))
                        return DetectedDiscType.Wii;

                    // dreamcast
                    if (GetDreamcastData(dataStr))
                        return DetectedDiscType.DreamCast;

                    // sega cd (megacd)
                    if (GetSegaCDData(dataStr))
                        return DetectedDiscType.SegaCD;

                    // amiga
                    if (GetAmigaData(dataStr))
                        return DiscSubType;

                    // neogeo
                    if (GetNeoGeoCDData(dataStr))
                        return DetectedDiscType.NeoGeoCD;

                    // playdia
                    if (GetPlaydiaData(dataStr))
                        return DetectedDiscType.BandaiPlaydia;

                    // CDi
                    if (GetCDiData(dataStr))
                        return DetectedDiscType.PhilipsCDi;

                    // pcfx
                    if (GetPCFXData(dataStr))
                        return DetectedDiscType.PCFX;

                    // pce-cd
                    if (GetPCECDData(dataStr))
                        return DetectedDiscType.PCEngineCD;

                    // 3DO
                    if (Get3DOData(dataStr))
                        return DetectedDiscType.Panasonic3DO;

                }
            }




            ////////////////////////////////////////////////////////////////////
            /* Finally use bizhawk's detection to catch anything we've missed
             * This is almost certainly just AudioCD or UnknownCDFS */
            ////////////////////////////////////////////////////////////////////

            if (discI.DetectedDiscType == DetectedDiscType.UnknownFormat)
            {
                var dt = di.DetectDiscType();

                switch (dt)
                {
                    case DiscType.UnknownFormat: return DetectedDiscType.UnknownFormat;
                    case DiscType.UnknownCDFS: return DetectedDiscType.UnknownCDFS;
                    case DiscType.AudioDisc: return DetectedDiscType.AudioCD;
                }
            }

            return DetectedDiscType.UnknownFormat;
        }
    }
}
