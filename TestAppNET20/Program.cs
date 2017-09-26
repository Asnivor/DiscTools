using System;
using System.Collections.Generic;
using System.Text;
using DiscTools;

namespace TestAppNET20
{
    class Program
    {
        static void Main(string[] args)
        {



            // get ss serial
            string ssPath = @"G:\_Emulation\Sega Saturn\disks\Virtua Fighter CG Portrait Series Vol1 Sarah Bryant (J) [!]\Virtua Fighter CG Portrait Series Vol1 Sarah Bryant (J) [!] [SegaSaturn].CCD";
            //string ssPath = @"G:\_Emulation\PC Engine\discs\Ys III - Wanderers from Ys [U][CD][TGXCD1015][Falcom][1991][PCE]\Ys III - Wanderers from Ys [U][CD][TGXCD1015][Falcom][1991][PCE].cue";
            //string ssPath = @"G:\_Emulation\PC Engine\discs\Godzilla [U][SCD][TGXCD1051][Toho][1993][PCE]\Godzilla [U][SCD][TGXCD1051][Toho][1993][PCE].cue";
            //string ssPath = @"G:\_Emulation\PCFX\Games\Battle Heat\Battle Heat.cue";
            //string ssPath = @"G:\_Emulation\PCFX\Games\Angelique Special\Angelique Special.cue";

            var disc = new DiscInspector(ssPath);
            
            //string ssResult = SerialNumber.GetSaturnSerial(ssPath);

            // get psx serial
            string psxPath = @"G:\_Emulation\PSX\iso\Tekken 3 (USA)\Tekken 3 (USA).cue";
            string psxResult = SerialNumber.GetPSXSerial(psxPath);
            Console.WriteLine(psxResult);

            psxPath = @"G:\_Emulation\PSX\iso\Tekken 3 (E) [SCES-01237]\Tekken 3 (E) [SCES-01237].cue";
            psxResult = SerialNumber.GetPSXSerial(psxPath);
            Console.WriteLine(psxResult);


            /*
            // get ss serial
            string ssPath = @"D:\Dropbox\Dropbox\_Games\Emulation\_Emulators\Mednafen\mednafen-latest\_roms\ss\Virtual Open Tennis\Virtual Open Tennis.ccd";
            string ssResult = SerialNumber.GetSaturnSerial(ssPath);
            Console.WriteLine(ssResult);
            */

            Console.ReadKey();
        }
    }
}
