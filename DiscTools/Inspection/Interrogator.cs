using DiscTools.ISO;
using DiscTools.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace DiscTools.Inspection
{
    /// <summary>
    /// handles all disc indentification and data retrieval stuff
    /// </summary>
    public partial class Interrogator
    {
        public DiscInspector discI = new DiscInspector();
        public bool IntenseScan { get; set; }

        public Disc disc;
        public EDiscStreamView discView;
        public ISOFile iso;
        public DiscIdentifier di;
        public int CurrentLBA;
        public bool isIso;
        public ISONode ifn;
        public byte[] currSector = new byte[2048];

        private DetectedDiscType DiscSubType { get; set; }

        /* Constructors */
        //public Interrogator() { }

        public Interrogator(string cuePath, bool intenseScan)
        {
            discI.CuePath = cuePath;
            IntenseScan = intenseScan;

            discI.DetectedDiscType = DetectedDiscType.UnknownFormat;
            discI.iso = new ISOFile();
            discI.Data = new DiscData();
        }

        /* Methods */

        public DiscInspector Start()
        {
            return Start(DetectedDiscType.UnknownFormat);
        }

        public DiscInspector Start(DetectedDiscType detectedDiscType)
        {
            // cue existance check
            if (discI.CuePath == null || !File.Exists(discI.CuePath))
                return discI;

            // attempt to mount the disc
            try
            {
                disc = Disc.LoadAutomagic(discI.CuePath);
                if (disc == null)
                    return discI;
            }  catch { return discI; }

            // detect disc mode
            discView = EDiscStreamView.DiscStreamView_Mode1_2048;
            if (disc.TOC.Session1Format == SessionFormat.Type20_CDXA)
                discView = EDiscStreamView.DiscStreamView_Mode2_Form1_2048;

            // try and mount it as an ISO
            isIso = iso.Parse(new DiscStream(disc, discView, 0));

            // if iso is mounted, populate data from volume descriptor(s) (at the moment just from the first one)
            var vs = iso.VolumeDescriptors.Where(a => a != null).ToArray().First();

            // translate the vd
            discI.Data.ISOData = PopulateISOData(vs);
            discI.Data.ISOData.ISOFiles = iso.Root.Children;
            ifn = null;

            // populate basic disc data
            int dataTracks = 0;
            int audioTracks = 0;

            foreach (var t in disc.Structure.Sessions.Where(a => a != null))
            {
                for (int i = 0; i < t.Tracks.Count(); i++)
                {
                    if (t.Tracks[i].IsData == true)
                    {
                        dataTracks++;
                        continue;
                    }

                    if (t.Tracks[i].IsAudio == true)
                    {
                        audioTracks++;
                        continue;
                    }
                }
            }

            discI.Data.TotalAudioTracks = audioTracks;
            discI.Data.TotalDataTracks = dataTracks;
            discI.Data.TotalTracks = audioTracks + dataTracks;

            // do actual interrogation
            switch (detectedDiscType)
            {
                case DetectedDiscType.UnknownFormat:
                case DetectedDiscType.UnknownCDFS:
                    discI.DetectedDiscType = InterrogateALL();
                    break;
            }


            discI.DiscTypeString = discI.DetectedDiscType.ToString();
            discI.DiscViewString = discView.ToString();

            return discI;
        }

        private readonly Dictionary<int, byte[]> _sectorCache = new Dictionary<int, byte[]>();

        private byte[] ReadSectorCached(int lba)
        {
            //read it if we dont have it cached
            //we wont be caching very much here, it's no big deal
            //identification is not something we want to take a long time
            byte[] data;
            if (!_sectorCache.TryGetValue(lba, out data))
            {
                data = new byte[2048];
                int read = di.dsr.ReadLBA_2048(lba, data, 0);
                if (read != 2048)
                    return null;
                _sectorCache[lba] = data;
            }
            return data;
        }

        private bool StringAt(string s, int n, int lba = 0)
        {
            var data = ReadSectorCached(lba);
            if (data == null) return false;
            byte[] cmp = System.Text.Encoding.ASCII.GetBytes(s);
            byte[] cmp2 = new byte[cmp.Length];
            Buffer.BlockCopy(data, n, cmp2, 0, cmp.Length);
            return System.Linq.Enumerable.SequenceEqual(cmp, cmp2);
        }


        /* Static Methods */
        private static ISOData PopulateISOData(ISOVolumeDescriptor vd)
        {
            ISOData i = new ISOData();

            // strings
            i.AbstractFileIdentifier = System.Text.Encoding.Default.GetString(vd.AbstractFileIdentifier).TrimEnd('\0', ' ');
            i.ApplicationIdentifier = System.Text.Encoding.Default.GetString(vd.ApplicationIdentifier).TrimEnd('\0', ' ');
            i.BibliographicalFileIdentifier = System.Text.Encoding.Default.GetString(vd.BibliographicalFileIdentifier).TrimEnd('\0', ' ');
            i.CopyrightFileIdentifier = System.Text.Encoding.Default.GetString(vd.CopyrightFileIdentifier).TrimEnd('\0', ' ');
            i.DataPreparerIdentifier = System.Text.Encoding.Default.GetString(vd.DataPreparerIdentifier).TrimEnd('\0', ' ');
            i.PublisherIdentifier = System.Text.Encoding.Default.GetString(vd.PublisherIdentifier).TrimEnd('\0', ' ');
            i.Reserved = System.Text.Encoding.Default.GetString(vd.Reserved).Trim('\0');
            i.SystemIdentifier = System.Text.Encoding.Default.GetString(vd.SystemIdentifier).TrimEnd('\0', ' ');
            i.VolumeIdentifier = System.Text.Encoding.Default.GetString(vd.VolumeIdentifier).TrimEnd('\0', ' ');
            i.VolumeSetIdentifier = System.Text.Encoding.Default.GetString(vd.VolumeSetIdentifier).TrimEnd('\0', ' ');

            // ints
            i.NumberOfSectors = vd.NumberOfSectors;
            i.PathTableSize = vd.PathTableSize;
            i.SectorSize = vd.SectorSize;
            i.Type = vd.Type;
            i.VolumeSequenceNumber = vd.VolumeSequenceNumber;

            // datetimes
            i.EffectiveDateTime = TextConverters.ParseDiscDateTime(TextConverters.TruncateLongString(System.Text.Encoding.Default.GetString(vd.EffectiveDateTime.ToArray()).Trim(), 12));
            i.ExpirationDateTime = TextConverters.ParseDiscDateTime(TextConverters.TruncateLongString(System.Text.Encoding.Default.GetString(vd.ExpirationDateTime.ToArray()).Trim(), 12));
            i.LastModifiedDateTime = TextConverters.ParseDiscDateTime(TextConverters.TruncateLongString(System.Text.Encoding.Default.GetString(vd.LastModifiedDateTime.ToArray()).Trim(), 12));
            i.VolumeCreationDate = TextConverters.ParseDiscDateTime(TextConverters.TruncateLongString(System.Text.Encoding.Default.GetString(vd.VolumeCreationDateTime.ToArray()).Trim(), 12));

            // other
            i.RootDirectoryRecord = vd.RootDirectoryRecord;

            return i;
        }
    }
}
