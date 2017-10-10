using DiscTools.Inspection.Statics.SonyMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscTools.Objects
{
    /// <summary>
    /// PSP specific data obtained from PARAM.SFO
    /// </summary>
    public class CDiData
    {
        public string SystemID { get; set; }
        public string OperatingSystemID { get; set; }
        public string GameName { get; set; }
        public string GameName2 { get; set; }
        public string Publisher { get; set; }
        public string Developer { get; set; }
        public string DiscID { get; set; }
        public int MyProperty { get; set; }



        public static void ParseCDiData(DiscInspector di, SFO sfo)
        {
            
        }

        
    }

    
}
