using DiscTools.ISO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscTools.Objects
{
    public class DiscData
    {
        public string GameTitle { get; set; }
        public string ManufacturerID { get; set; }
        public string SerialNumber { get; set; }
        public string Version { get; set; }
        public string InternalDate { get; set; }
        public string DeviceInformation { get; set; }
        public string AreaCodes { get; set; }
        public string PeripheralCodes { get; set; }
        public int TotalTracks { get; set; }
        public int TotalDataTracks { get; set; }
        public int TotalAudioTracks { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public DateTime? ModificationDateTime { get; set; }
    }    
}
