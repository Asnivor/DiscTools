using DiscTools.ISO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscTools.Objects
{
    /// <summary>
    /// ISO data obtained directly from the volume descriptor
    /// </summary>
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
