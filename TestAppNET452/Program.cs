using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscTools;

namespace TestAppNET452
{
    class Program
    {
        static void Main(string[] args)
        {



            // get ss serial
            //string ssPath = @"G:\_Emulation\Sega Saturn\disks\Virtua Fighter CG Portrait Series Vol1 Sarah Bryant (J) [!]\Virtua Fighter CG Portrait Series Vol1 Sarah Bryant (J) [!] [SegaSaturn].CCD";
            string ssPath = @"G:\_Emulation\PSX\iso\Resident Evil 2 - Dual Shock Ver. (USA) (Disc 1) (Leon)\Resident Evil 2 - Dual Shock Ver. (USA) (Disc 1) (Leon).cue";


            var data = new DiscInspector(ssPath);

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
