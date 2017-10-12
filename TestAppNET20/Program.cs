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

            //DiscInspector.test();

            //string dcPath = @"G:\_Emulation\Dreamcast\games\Daytona_USA_USA_DC-ECHELON\E-DAYUSA.cue";
            //string dcPath = @"G:\_Emulation\Dreamcast\games\Sword_Of_The_Berserk_USA_DC-KALISTO\kal-sotb.cue";
            //string dcPath = @"G:\_Emulation\Dreamcast\games\Sword_Of_The_Berserk_USA_DC-KALISTO\kal-sotb.cdi";
            //string dcPath = @"G:\_Emulation\Dreamcast\games\Sega_Rally_2_Championship_USA_DC-KALISTO\KAL-SR2.CUE";
            //string dcPath = @"G:\_Emulation\PSX\iso\Legend of Mana (USA)\Legend of Mana (USA).cue";
            //string dcPath = @"G:\_Emulation\Sega Saturn\disks\ws-gals_panic.cue";
            //string dcPath = @"G:\_Emulation\NeoGeo CD\discs\Metal Slug (1996)(SNK)(Jp-US)[!].cue";
            //string dcPath = @"G:\_Emulation\NeoGeo CD\discs\Breakers (1997)(Visco)(Jp)[!].cue";
            //string dcPath = @"G:\_Emulation\Philips CD-i\discs\Hotel Mario.cue";
            //string dcPath = @"G:\_Emulation\Sega Megadrive - 32x - SegaCD\discs\segacd\Shining Force CD (Sega CD) (U)-redump.cue";
            //string dcPath = @"G:\_Emulation\Sega Saturn\disks\Primal Rage (Europe) (En,Fr,De,Es,It,Pt)\Primal Rage (Europe) (En,Fr,De,Es,It,Pt).cue";
            //string dcPath = @"G:\_Emulation\PCFX\Games\Battle Heat\Battle Heat.cue";
            //string dcPath = @"G:\_Emulation\PCFX\Games\Angelique Special\Angelique Special.cue";


            //string dcPath = @"G:\_Emulation\Philips CD-i\discs\Zombie Dinos.cue";


            //string dcPath = @"G:\_Emulation\PSX\iso\Metal Gear Solid - Integral (J) [SLPM-86247]\Metal Gear Solid - Integral (J) (Disc 1) [SLPM-86247].cue";
            //string dcPath = @"G:\_Emulation\PC Engine\discs\Cyber_City_OEDO_808_(NTSC-J)_[NSCD0003]\Cyber_City_OEDO_808_(NTSC-J)_[NSCD0003].cue";
            //string dcPath = @"G:\_Emulation\Sega Saturn\disks\Virtua Fighter CG Portrait Series Vol1 Sarah Bryant (J) [!]\Virtua Fighter CG Portrait Series Vol1 Sarah Bryant (J) [!] [SegaSaturn].CCD";

            //var cdi = DiscInspector.ScanCDi(@"G:\_Emulation\Philips CD-i\discs\Zombie Dinos.cue");
            //var psx = DiscInspector.ScanPSX(@"G:\_Emulation\PSX\iso\Metal Gear Solid - Integral (J) [SLPM-86247]\Metal Gear Solid - Integral (J) (Disc 1) [SLPM-86247].cue");
            //var saturn = DiscInspector.ScanSaturn(@"G:\_Emulation\Sega Saturn\disks\Primal Rage (Europe) (En,Fr,De,Es,It,Pt)\Primal Rage (Europe) (En,Fr,De,Es,It,Pt).cue");
            //var pcecd = DiscInspector.ScanPCECD(@"G:\_Emulation\PC Engine\discs\Godzilla [U][SCD][TGXCD1051][Toho][1993][PCE]\Godzilla [U][SCD][TGXCD1051][Toho][1993][PCE].cue");
            //var pcecd = DiscInspector.ScanPCECD(@"G:\_Emulation\PC Engine\discs\Cyber_City_OEDO_808_(NTSC-J)_[NSCD0003]\Cyber_City_OEDO_808_(NTSC-J)_[NSCD0003].cue");
            //var pcfx = DiscInspector.ScanPCFX(@"G:\_Emulation\PCFX\Games\Angelique Special\Angelique Special.cue");
            //var segacd = DiscInspector.ScanSegaCD(@"G:\_Emulation\Sega Megadrive - 32x - SegaCD\discs\segacd\Shining Force CD (Sega CD) (U)-redump.cue");
            //var neogeo = DiscInspector.ScanNeoGeoCD(@"G:\_Emulation\NeoGeo CD\discs\Breakers (1997)(Visco)(Jp)[!].cue");
            //var dreamcast = DiscInspector.ScanDreamcast(@"G:\_Emulation\Dreamcast\games\Daytona_USA_USA_DC-ECHELON\E-DAYUSA.cue1");

            //string dcPath = @"G:\_Emulation\PSX\iso\Alien Trilogy (E) [SLES-00101]\Alien Trilogy (E) [SLES-00101]\Alien Trilogy (E) [SLES-00101].cue";
            // string dcPath = @"G:\_Emulation\PC Engine\discs\L-Dis (1991)(Masaya - NCS)(JP).ccd";

            //DiscInspector.UnknownTest();

            //string dcPath = @"G:\_Emulation\Panasonic 3DO\Need for Speed, The (1994)(Electronic Arts)(US)[A1115 CC 735507-2 R70].cue";
            //string dcPath = @"G:\_Emulation\Sega Saturn\disks\ws-gals_panic.cue";
            //string dcPath = @"G:\_Emulation\Commadore Amiga\cd32\Zool - Ninja of the 'Nth' Dimension (1993)(Gremlin)[!].cue";
            //string dcPath = @"C:\Users\matt\Desktop\test1\Angelique Special.cue";

            //string dcPath = @"G:\_Emulation\Commadore Amiga\cdtv\Nemac IV - Director's Cut (1996)(ZenTek)[!].cue";

            //string dcPath = @"G:\_Emulation\Bandai Playdia\Aqua Adventure - Blue Lilty (1994)(Bandai)(JP)[!].cue";
            //string dcPath = @"G:\_Emulation\Bandai Playdia\Ultraman - Hiragana Dai Sakusen (1992)(Bandai)(JP).cue";

            //string dcPath = @"C:\Users\admin\Downloads\Snatcher_(NTSC-J)_[KMCD2002]\Snatcher_(NTSC-J)_[KMCD2002].cue";
            //string dcPath = @"C:\Users\admin\Downloads\Snatcher_(NTSC-J)_[KMCD2002]\Kisou_Louga_(NTSC-J)_[KSCD3004].cue";
            //string dcPath = @"C:\Users\admin\Downloads\Snatcher_(NTSC-J)_[KMCD2002]\Dragon_Ball_Z_-_Idainaru_Son_Gokuu_Densetsu_(NTSC-J)_[BNCD4001].cue";

            //string dcPath = @"G:\_Emulation\Dreamcast\games\Dead_or_Alive_2_USA_DC-UTOPIA\dead2.cdi";
            // string dcPath = @"G:\_Emulation\Gamecube - Wii\disks\Mortal Kombat - Deadly Alliance (Europe).iso";
            //string dcPath = @"G:\_Emulation\Gamecube - Wii\wii\Super Mario Galaxy 2 WII PAL MULTI5.iso";

            //string dcPath = @"G:\_Emulation\PS2\Games\Gran Turismo 4 (USA)\Gran Turismo 4 (USA).iso";

            //string dcPath = @"G:\_Emulation\PS2\Games\Grand Theft Auto III (Europe) (En,Fr,De,Es,It) (v1.60)\Grand Theft Auto III (Europe) (En,Fr,De,Es,It) (v1.60).iso";

            //string dcPath = @"G:\_Emulation\PS2\Games\Dark Wind (Europe) (En,Fr,De,Es,It)\Dark Wind (Europe) (En,Fr,De,Es,It).cue";

            //string dcPath = @"C:\Users\admin\Downloads\Speed Power Gunbike (Japan) [SLPS-01066]\Speed Power Gunbike (Japan) [SLPS-01066].cue";
            //string dcPath = @"C:\Users\admin\Downloads\Rakugaki Showtime (Japan) [SLPM-86272]\Rakugaki Showtime (Japan) [SLPM-86272].cue";

            //string dcPath = @"G:\_Emulation\psp\Ace_Combat_Joint_Assault_EUR_PSP-RoME\rm-acja.iso";
            // string dcPath = @"G:\_Emulation\psp\Star_Ocean_Second_Evolution_USA_PSP-PSPKiNG\pspking-sose.ISO";

            //string dcPath = @"G:\_Emulation\PSX\iso\Alien Trilogy (E) [SLES-00101]\Alien Trilogy (E) [SLES-00101].cue";
            //string dcPath = @"G:\_Emulation\PSX\iso\Cool Boarders 2 (USA)\Cool Boarders 2 (USA)2.cue";


            //string dcPath = @"G:\_Emulation\FM Towns\marty\Dungeon Master II for FM-Towns.cue";
            string dcPath = @"G:\_Emulation\FM Towns\marty\Dungeon.Master.ISO.FM-Towns-OpTiMaL.cue";

            var dc = DiscInspector.ScanDisc(dcPath);

            string stop = "";

            /*
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
            */

            Console.ReadKey();
        }
    }
}
