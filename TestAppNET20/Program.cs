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
