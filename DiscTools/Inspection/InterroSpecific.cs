using DiscTools.ISO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscTools.Inspection
{
    public partial class Interrogator
    {
        /// <summary>
        /// Runs through interrogation process to identify all discs
        /// </summary>
        public DetectedDiscType InterrogateSpecific(DetectedDiscType systemType)
        {
            ///////////////////////
            /* First ISO related */
            ///////////////////////

            if (isIso)
            {
                // psx
                if (systemType == DetectedDiscType.SonyPSX)
                {
                    if (ScanISOPSX())
                        return DetectedDiscType.SonyPSX;
                }

                // saturn
                if (systemType == DetectedDiscType.SegaSaturn)
                {
                    if (ScanISOSaturn())
                        return DetectedDiscType.SegaSaturn;
                }

                // dreamcast
                if (systemType == DetectedDiscType.DreamCast)
                {
                    if (ScanISODreamcast())
                        return DetectedDiscType.DreamCast;
                }

                // sega cd (megacd)
                if (systemType == DetectedDiscType.SegaCD)
                {
                    if (ScanISOSegaCD())
                        return DetectedDiscType.SegaCD;
                }

                // amiga
                if (systemType == DetectedDiscType.AmigaCDTV || systemType == DetectedDiscType.AmigaCD32)
                {
                    if (ScanISOAmiga())
                    {
                        return DiscSubType;
                    }
                }

                // neogeo cd
                if (systemType == DetectedDiscType.NeoGeoCD)
                {
                    if (ScanISONeoGeoCD())
                        return DetectedDiscType.NeoGeoCD;
                }

                // playdia
                if (systemType == DetectedDiscType.BandaiPlaydia)
                {
                    if (ScanISOPlaydia())
                        return DetectedDiscType.BandaiPlaydia;
                }

                // pcecd - currently no ISO lookup method

                // pcfx - currently no ISO lookup method

                // CD-i - currently no ISO lookup method

                // 3do - currently no ISO lookup method
            }




            /////////////////////////////////////////////////////////
            /* Non-ISO Direct queries (where we can guess the LBA) */
            /////////////////////////////////////////////////////////

            // psx
            if (systemType == DetectedDiscType.SonyPSX)
            {
                CurrentLBA = 23;
                if (GetPSXData())
                    return DetectedDiscType.SonyPSX;
            }

            // saturn
            if (systemType == DetectedDiscType.SegaSaturn)
            {
                CurrentLBA = 0;
                if (GetSaturnData())
                    return DetectedDiscType.SegaSaturn;
            }

            // dreamcast - no direct method yet

            // sega cd (megacd)
            if (systemType == DetectedDiscType.SegaCD)
            {
                CurrentLBA = 0;
                if (GetSegaCDData())
                    return DetectedDiscType.SegaCD;
                CurrentLBA = 16;
                if (GetSegaCDData())
                    return DetectedDiscType.SegaCD;
            }

            // amiga - but of a kludge, maybe try lba 0
            if (systemType == DetectedDiscType.AmigaCDTV || systemType == DetectedDiscType.AmigaCD32)
            {
                CurrentLBA = 0;
                if (GetAmigaData())
                    return DiscSubType;
            }

            // neogeo - no direct method yet

            // playdia - no direct method yet

            // pcecd - no direct method yet

            // pcfx - no direct method yet

            // CD-i - check lba 16 first - this seems to be quite common
            if (systemType == DetectedDiscType.PhilipsCDi)
            {
                CurrentLBA = 16;
                if (GetCDiData())
                    return DetectedDiscType.PhilipsCDi;
            }

            // 3do - no direct method yet





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
                if (systemType == DetectedDiscType.SonyPSX)
                {
                    if (GetPSXData(text))
                        return DetectedDiscType.SonyPSX;
                }

                // saturn
                if (systemType == DetectedDiscType.SegaSaturn)
                {
                    if (GetSaturnData(text))
                        return DetectedDiscType.SegaSaturn;
                }

                // dreamcast
                if (systemType == DetectedDiscType.DreamCast)
                {
                    if (GetDreamcastData(text))
                        return DetectedDiscType.DreamCast;
                }

                // sega cd (megacd)
                if (systemType == DetectedDiscType.SegaCD)
                {
                    if (GetSegaCDData(text))
                        return DetectedDiscType.SegaCD;
                }

                // amiga
                if (systemType == DetectedDiscType.AmigaCDTV || systemType == DetectedDiscType.AmigaCD32)
                {
                    if (GetAmigaData(text))
                        return DiscSubType;
                }

                // neogeo
                if (systemType == DetectedDiscType.NeoGeoCD)
                {
                    if (GetNeoGeoCDData(text))
                        return DetectedDiscType.NeoGeoCD;
                }

                // playdia
                if (systemType == DetectedDiscType.BandaiPlaydia)
                {
                    if (GetPlaydiaData(text))
                        return DetectedDiscType.BandaiPlaydia;
                }

                // CDi
                if (systemType == DetectedDiscType.PhilipsCDi)
                {
                    if (GetCDiData(text))
                        return DetectedDiscType.PhilipsCDi;
                }

                // pcfx
                if (systemType == DetectedDiscType.PCFX)
                {
                    if (GetPCFXData(text))
                        return DetectedDiscType.PCFX;
                }

                // pce-cd
                if (systemType == DetectedDiscType.PCEngineCD)
                {
                    if (GetPCECDData(text))
                        return DetectedDiscType.PCEngineCD;
                }

                // 3DO
                if (systemType == DetectedDiscType.Panasonic3DO)
                {
                    if (Get3DOData(text))
                        return DetectedDiscType.Panasonic3DO;
                }
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
                if (systemType == DetectedDiscType.SonyPSX)
                {
                    if (GetPSXData(dataStr))
                        return DetectedDiscType.SonyPSX;
                }

                // saturn
                if (systemType == DetectedDiscType.SegaSaturn)
                {
                    if (GetSaturnData(dataStr))
                        return DetectedDiscType.SegaSaturn;
                }

                // dreamcast
                if (systemType == DetectedDiscType.DreamCast)
                {
                    if (GetDreamcastData(dataStr))
                        return DetectedDiscType.DreamCast;
                }

                // sega cd (megacd)
                if (systemType == DetectedDiscType.SegaCD)
                {
                    if (GetSegaCDData(dataStr))
                        return DetectedDiscType.SegaCD;
                }

                // amiga
                if (systemType == DetectedDiscType.AmigaCDTV || systemType == DetectedDiscType.AmigaCD32)
                {
                    if (GetAmigaData(dataStr))
                        return DiscSubType;
                }

                // neogeo
                if (systemType == DetectedDiscType.NeoGeoCD)
                {
                    if (GetNeoGeoCDData(dataStr))
                        return DetectedDiscType.NeoGeoCD;
                }

                // playdia
                if (systemType == DetectedDiscType.BandaiPlaydia)
                {
                    if (GetPlaydiaData(dataStr))
                        return DetectedDiscType.BandaiPlaydia;
                }

                // CDi
                if (systemType == DetectedDiscType.PhilipsCDi)
                {
                    if (GetCDiData(dataStr))
                        return DetectedDiscType.PhilipsCDi;
                }

                // pcfx
                if (systemType == DetectedDiscType.PCFX)
                {
                    if (GetPCFXData(dataStr))
                        return DetectedDiscType.PCFX;
                }

                // pce-cd
                if (systemType == DetectedDiscType.PCEngineCD)
                {
                    if (GetPCECDData(dataStr))
                        return DetectedDiscType.PCEngineCD;
                }

                // 3DO
                if (systemType == DetectedDiscType.Panasonic3DO)
                {
                    if (Get3DOData(dataStr))
                        return DetectedDiscType.Panasonic3DO;
                }
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
                    if (systemType == DetectedDiscType.SonyPSX)
                    {
                        if (GetPSXData(dataStr))
                            return DetectedDiscType.SonyPSX;
                    }

                    // saturn
                    if (systemType == DetectedDiscType.SegaSaturn)
                    {
                        if (GetSaturnData(dataStr))
                            return DetectedDiscType.SegaSaturn;
                    }


                    // dreamcast
                    if (systemType == DetectedDiscType.DreamCast)
                    {
                        if (GetDreamcastData(dataStr))
                            return DetectedDiscType.DreamCast;
                    }

                    // sega cd (megacd)
                    if (systemType == DetectedDiscType.SegaCD)
                    {
                        if (GetSegaCDData(dataStr))
                            return DetectedDiscType.SegaCD;
                    }

                    // amiga
                    if (systemType == DetectedDiscType.AmigaCDTV || systemType == DetectedDiscType.AmigaCD32)
                    {
                        if (GetAmigaData(dataStr))
                            return DiscSubType;
                    }

                    // neogeo
                    if (systemType == DetectedDiscType.NeoGeoCD)
                    {
                        if (GetNeoGeoCDData(dataStr))
                            return DetectedDiscType.NeoGeoCD;
                    }

                    // playdia
                    if (systemType == DetectedDiscType.BandaiPlaydia)
                    {
                        if (GetPlaydiaData(dataStr))
                            return DetectedDiscType.BandaiPlaydia;
                    }

                    // CDi
                    if (systemType == DetectedDiscType.PhilipsCDi)
                    {
                        if (GetCDiData(dataStr))
                            return DetectedDiscType.PhilipsCDi;
                    }

                    // pcfx
                    if (systemType == DetectedDiscType.PCFX)
                    {
                        if (GetPCFXData(dataStr))
                            return DetectedDiscType.PCFX;
                    }

                    // pce-cd
                    if (systemType == DetectedDiscType.PCEngineCD)
                    {
                        if (GetPCECDData(dataStr))
                            return DetectedDiscType.PCEngineCD;
                    }

                    // 3DO
                    if (systemType == DetectedDiscType.Panasonic3DO)
                    {
                        if (Get3DOData(dataStr))
                            return DetectedDiscType.Panasonic3DO;
                    }

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
