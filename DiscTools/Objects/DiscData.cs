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
        public string Publisher { get; set; }
        public string Developer { get; set; }
        public string SerialNumber { get; set; }
        public string Version { get; set; }
        public string InternalDate { get; set; }
        public string DeviceInformation { get; set; }
        public string AreaCodes { get; set; }
        public string PeripheralCodes { get; set; }
        public string OtherData { get; set; }
        public string MediaInfo { get; set; }
        public string MediaID { get; set; }
        public int TotalTracks { get; set; }
        public int TotalDataTracks { get; set; }
        public int TotalAudioTracks { get; set; }
        public ISOData ISOData { get; set; }
    }    

    public class ISOData
    {
        public string AbstractFileIdentifier { get; set; }
        public string ApplicationIdentifier { get; set; }
        public string BibliographicalFileIdentifier { get; set; }
        public string CopyrightFileIdentifier { get; set; }
        public string DataPreparerIdentifier { get; set; }
        public DateTime? EffectiveDateTime { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime? LastModifiedDateTime { get; set; }
        public int NumberOfSectors { get; set; }
        public int PathTableSize { get; set; }
        public string PublisherIdentifier { get; set; }
        public string Reserved { get; set; }
        public ISONodeRecord RootDirectoryRecord { get; set; }
        public int SectorSize { get; set; }
        public string SystemIdentifier { get; set; }
        public int Type { get; set; }
        public DateTime? VolumeCreationDate { get; set; }
        public string VolumeIdentifier { get; set; }
        public int VolumeSequenceNumber { get; set; }
        public string VolumeSetIdentifier { get; set; }
        public int VolumeSetSize { get; set; }
        public Dictionary<string, ISONode> ISOFiles { get; set; }
    }
}
