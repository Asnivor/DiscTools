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

                    switch (DISC.DiscType)
                    {
                        case DiscTools.ISO.DiscType.PCFX:
                            pcfx.Add(DISC);
                            break;
                        case DiscTools.ISO.DiscType.SegaSaturn:
                            ss.Add(DISC);
                            break;
                        case DiscTools.ISO.DiscType.SonyPSX:
                            psx.Add(DISC);
                            break;
                        case DiscTools.ISO.DiscType.TurboCD:
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
