using System;
using System.Collections.Generic;
using System.Text;
using DiscTools;
using System.Linq;

namespace TestAppNET20
{
    class Program
    {
        static void Main(string[] args)
        {
            //string dcPath = @"G:\_Emulation\Dreamcast\games\Daytona_USA_USA_DC-ECHELON\E-DAYUSA.cue";
            //string dcPath = @"G:\_Emulation\Dreamcast\games\Sword_Of_The_Berserk_USA_DC-KALISTO\kal-sotb.cue";
            string dcPath = @"G:\_Emulation\Dreamcast\games\Sword_Of_The_Berserk_USA_DC-KALISTO\kal-sotb.cdi";
            //string dcPath = @"G:\_Emulation\Dreamcast\games\Sega_Rally_2_Championship_USA_DC-KALISTO\KAL-SR2.CUE";
            var dc = new DiscInspector(dcPath);

            //string neocdPath = @"G:\_Emulation\NeoGeo CD\discs\Metal Slug (1996)(SNK)(Jp-US)[!].cue";
            string neocdPath = @"G:\_Emulation\NeoGeo CD\discs\Breakers (1997)(Visco)(Jp)[!].cue";
            var neocd = new DiscInspector(neocdPath);

            string cdiPath = @"G:\_Emulation\Philips CD-i\discs\Hotel Mario.cue";
            //string cdiPath = @"G:\_Emulation\Philips CD-i\discs\Zombie Dinos.cue";
            var cdi = new DiscInspector(cdiPath);

            string sega = @"G:\_Emulation\Sega Megadrive - 32x - SegaCD\discs\segacd\sonic_cd_-_sega_cd_MK-4407_(redump).cue";
            var sDisc = new DiscInspector(sega);

            sega = @"G:\_Emulation\Sega Megadrive - 32x - SegaCD\discs\segacd\Shining Force CD (Sega CD) (U)-redump.cue";
            var sDisc2 = new DiscInspector(sega);

            string baseFolder = @"G:\_Emulation";
            string[] discFolders = new string[]
            {
                "\\PC Engine",
                "\\PCFX",
                "\\PSX",
                "\\Sega Saturn"
            };

            List<string> files = new List<string>();

            foreach (var f in discFolders)
            {
                List<string> allFiles = System.IO.Directory.GetFiles(baseFolder + f, "*.*", System.IO.SearchOption.AllDirectories)
                .Where(a => System.IO.Path.GetExtension(a).ToLower() == ".cue" ||
                System.IO.Path.GetExtension(a).ToLower() == ".ccd" ||
                System.IO.Path.GetExtension(a).ToLower() == ".toc").ToList();

                files.AddRange(allFiles);
            }

            List<DiscInspector> psx = new List<DiscInspector>();
            List<DiscInspector> ss = new List<DiscInspector>();
            List<DiscInspector> pce = new List<DiscInspector>();
            List<DiscInspector> pcfx = new List<DiscInspector>();
            List<DiscInspector> unknownOrAudio = new List<DiscInspector>();


            foreach (var s in files)
            {
                try
                {
                    var DISC = new DiscInspector(s);
                    if (DISC == null)
                        continue;
                    if (DISC.Data == null)
                        continue;

                    switch (DISC.DetectedDiscType)
                    {
                        case DetectedDiscType.PCFX:
                            pcfx.Add(DISC);
                            break;
                        case DetectedDiscType.SegaSaturn:
                            ss.Add(DISC);
                            break;
                        case DetectedDiscType.SonyPSX:
                            psx.Add(DISC);
                            break;
                        case DetectedDiscType.SegaCD:
                            pce.Add(DISC);
                            break;
                        default:
                            unknownOrAudio.Add(DISC);
                            break;
                    }
                }

                catch
                {
                    continue;
                }
                
            }

            Console.ReadKey();
        }
    }
}
