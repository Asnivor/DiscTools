using DiscTools.ISO.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using DiscTools.ISO.DiscFormats;
using DiscTools.ISO.Internal.Algorithms;
using DiscTools.ISO.Internal.Jobs;
using DiscTools.ISO.DiscFormats.Blobs;
using DiscTools.ISO.DiscFormats.CUE;
using System.IO;
using System.Linq;

namespace DiscTools.ISO
{
    public partial class Disc : IDisposable
    {
        /// <summary>
        /// Automagically loads a disc, without any fine-tuned control at all
        /// </summary>
        public static Disc LoadAutomagic(string path)
        {
            var job = new DiscMountJob { IN_FromPath = path };
            //job.IN_DiscInterface = DiscInterface.MednaDisc; //TEST
            job.Run();
            return job.OUT_Disc;
        }

        /// <summary>
        /// The DiscStructure corresponding to the TOCRaw
        /// </summary>
        public DiscStructure Structure;

        /// <summary>
        /// DiscStructure.Session 1 of the disc, since that's all thats needed most of the time.
        /// </summary>
        public DiscStructure.Session Session1 { get { return Structure.Sessions[1]; } }

        /// <summary>
        /// The name of a disc. Loosely based on the filename. Just for informational purposes.
        /// </summary>
        public string Name;

        /// <summary>
        /// The DiscTOCRaw corresponding to the RawTOCEntries.
        /// TODO - there's one of these for every session, so... having one here doesnt make sense
        /// so... 
        /// TODO - remove me
        /// </summary>
        public DiscTOC TOC;

        /// <summary>
        /// The raw TOC entries found in the lead-in track.
        /// These aren't very useful, but theyre one of the most lowest-level data structures from which other TOC-related stuff is derived
        /// </summary>
        public List<RawTOCEntry> RawTOCEntries = new List<RawTOCEntry>();

        /// <summary>
        /// Free-form optional memos about the disc
        /// </summary>
        public Dictionary<string, object> Memos = new Dictionary<string, object>();

        public void Dispose()
        {
            foreach (var res in DisposableResources)
            {
                res.Dispose();
            }
        }

        /// <summary>
        /// The DiscMountPolicy used to mount the disc. Consider this read-only.
        /// NOT SURE WE NEED THIS
        /// </summary>
        //public DiscMountPolicy DiscMountPolicy;

        //----------------------------------------------------------------------------

        /// <summary>
        /// Disposable resources (blobs, mostly) referenced by this disc
        /// </summary>
        internal List<IDisposable> DisposableResources = new List<IDisposable>();

        /// <summary>
        /// The sectors on the disc. Don't use this directly! Use the SectorSynthProvider instead.
        /// TODO - eliminate this entirely and do entirely with the delegate (much faster disc loading... but massively annoying architecture inside-out logic)
        /// </summary>
        internal List<ISectorSynthJob2448> _Sectors = new List<ISectorSynthJob2448>();

        /// <summary>
        /// ISectorSynthProvider instance for the disc. May be daisy-chained
        /// </summary>
        internal ISectorSynthProvider SynthProvider;

        /// <summary>
        /// Parameters set during disc loading which can be referenced by the sector synthesizers
        /// </summary>
        internal SectorSynthParams SynthParams = new SectorSynthParams();

        /// <summary>
        /// Forbid public construction
        /// </summary>
        internal Disc()
        { }

    }

    sealed partial class Disc
    {
        internal class Blob_ECM : IBlob
        {
            FileStream stream;

            public void Dispose()
            {
                if (stream != null)
                    stream.Dispose();
                stream = null;
            }

            private class IndexEntry
            {
                public int Type;
                public uint Number;
                public long ECMOffset;
                public long LogicalOffset;
            }

            /// <summary>
            /// an index of blocks within the ECM file, for random-access.
            /// itll be sorted by logical ordering, so you can binary search for the address you want
            /// </summary>
            private readonly List<IndexEntry> Index = new List<IndexEntry>();

            /// <summary>
            /// the ECMfile-provided EDC integrity checksum. not being used right now
            /// </summary>
            int EDC;

            public long Length;

            public void Load(string path)
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

                //skip header
                stream.Seek(4, SeekOrigin.Current);

                long logOffset = 0;
                for (;;)
                {
                    //read block count. this format is really stupid. maybe its good for detecting non-ecm files or something.
                    int b = stream.ReadByte();
                    if (b == -1) MisformedException();
                    int bytes = 1;
                    int T = b & 3;
                    long N = (b >> 2) & 0x1F;
                    int nbits = 5;
                    while (b.Bit(7))
                    {
                        if (bytes == 5) MisformedException(); //if we're gonna need a 6th byte, this file is broken
                        b = stream.ReadByte();
                        bytes++;
                        if (b == -1) MisformedException();
                        N |= (long)(b & 0x7F) << nbits;
                        nbits += 7;
                    }

                    //end of blocks section
                    if (N == 0xFFFFFFFF)
                        break;

                    //the 0x80000000 business is confusing, but this is almost positively an error
                    if (N >= 0x100000000)
                        MisformedException();

                    uint todo = (uint)N + 1;

                    IndexEntry ie = new IndexEntry
                    {
                        Number = todo,
                        ECMOffset = stream.Position,
                        LogicalOffset = logOffset,
                        Type = T
                    };

                    Index.Add(ie);

                    if (T == 0)
                    {
                        stream.Seek(todo, SeekOrigin.Current);
                        logOffset += todo;
                    }
                    else if (T == 1)
                    {
                        stream.Seek(todo * (2048 + 3), SeekOrigin.Current);
                        logOffset += todo * 2352;
                    }
                    else if (T == 2)
                    {
                        stream.Seek(todo * 2052, SeekOrigin.Current);
                        logOffset += todo * 2336;
                    }
                    else if (T == 3)
                    {
                        stream.Seek(todo * 2328, SeekOrigin.Current);
                        logOffset += todo * 2336;
                    }
                    else MisformedException();
                }

                //TODO - endian bug. need an endian-independent binary reader with good license (miscutils is apache license) 
                //extension methods on binary reader wont suffice, we need something that lets you control the endianness used for reading. a complete replacement.
                var br = new BinaryReader(stream);
                EDC = br.ReadInt32();

                Length = logOffset;
            }

            void MisformedException()
            {
                throw new InvalidOperationException("Mis-formed ECM file");
            }

            public static bool IsECM(string path)
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int e = fs.ReadByte();
                    int c = fs.ReadByte();
                    int m = fs.ReadByte();
                    int o = fs.ReadByte();
                    if (e != 'E' || c != 'C' || m != 'M' || o != 0)
                        return false;
                }

                return true;
            }

            /// <summary>
            /// finds the IndexEntry for the specified logical offset
            /// </summary>
            int FindInIndex(long offset, int LastReadIndex)
            {
                //try to avoid searching the index. check the last index we we used.
                for (int i = 0; i < 2; i++) //try 2 times
                {
                    IndexEntry last = Index[LastReadIndex];
                    if (LastReadIndex == Index.Count - 1)
                    {
                        //byte_pos would have to be after the last entry
                        if (offset >= last.LogicalOffset)
                        {
                            return LastReadIndex;
                        }
                    }
                    else
                    {
                        IndexEntry next = Index[LastReadIndex + 1];
                        if (offset >= last.LogicalOffset && offset < next.LogicalOffset)
                        {
                            return LastReadIndex;
                        }

                        //well, maybe we just advanced one sector. just try again one sector ahead
                        LastReadIndex++;
                    }
                }

                //Console.WriteLine("binary searched"); //use this to check for mistaken LastReadIndex logic resulting in binary searches during sequential access
                int listIndex = Index.LowerBoundBinarySearch(idx => idx.LogicalOffset, offset);
                System.Diagnostics.Debug.Assert(listIndex < Index.Count);
                //Console.WriteLine("byte_pos {0:X8} using index #{1} at offset {2:X8}", offset, listIndex, Index[listIndex].LogicalOffset);

                return listIndex;
            }

            void Reconstruct(byte[] secbuf, int type)
            {
                //sync
                secbuf[0] = 0;
                for (int i = 1; i <= 10; i++)
                    secbuf[i] = 0xFF;
                secbuf[11] = 0x00;

                //misc stuff
                switch (type)
                {
                    case 1:
                        //mode 1
                        secbuf[15] = 0x01;
                        //reserved
                        for (int i = 0x814; i <= 0x81B; i++)
                            secbuf[i] = 0x00;
                        break;

                    case 2:
                    case 3:
                        //mode 2
                        secbuf[15] = 0x02;
                        //flags - apparently CD XA specifies two copies of these 4bytes of flags. ECM didnt store the first copy; so we clone the second copy which was stored down to the spot for the first copy.
                        secbuf[0x10] = secbuf[0x14];
                        secbuf[0x11] = secbuf[0x15];
                        secbuf[0x12] = secbuf[0x16];
                        secbuf[0x13] = secbuf[0x17];
                        break;
                }

                //edc
                switch (type)
                {
                    case 1: ECM.PokeUint(secbuf, 0x810, ECM.EDC_Calc(secbuf, 0, 0x810)); break;
                    case 2: ECM.PokeUint(secbuf, 0x818, ECM.EDC_Calc(secbuf, 16, 0x808)); break;
                    case 3: ECM.PokeUint(secbuf, 0x92C, ECM.EDC_Calc(secbuf, 16, 0x91C)); break;
                }

                //ecc
                switch (type)
                {
                    case 1: ECM.ECC_Populate(secbuf, 0, secbuf, 0, false); break;
                    case 2: ECM.ECC_Populate(secbuf, 0, secbuf, 0, true); break;
                }

            }

            //we dont want to keep churning through this many big byte arrays while reading stuff, so we save a sector cache.
            readonly byte[] Read_SectorBuf = new byte[2352];
            int Read_LastIndex = 0;

            public int Read(long byte_pos, byte[] buffer, int offset, int _count)
            {
                long remain = _count;
                int completed = 0;

                //we take advantage of the fact that we pretty much always read one sector at a time.
                //this would be really inefficient if we only read one byte at a time.
                //on the other hand, just in case, we could keep a cache of the most recently decoded sector. that would be easy and would solve that problem (if we had it)

                while (remain > 0)
                {
                    int listIndex = FindInIndex(byte_pos, Read_LastIndex);

                    IndexEntry ie = Index[listIndex];
                    Read_LastIndex = listIndex;

                    if (ie.Type == 0)
                    {
                        //type 0 is special: its just a raw blob. so all we need to do is read straight out of the stream
                        long blockOffset = byte_pos - ie.LogicalOffset;
                        long bytesRemainInBlock = ie.Number - blockOffset;

                        long todo = remain;
                        if (bytesRemainInBlock < todo)
                            todo = bytesRemainInBlock;

                        stream.Position = ie.ECMOffset + blockOffset;
                        while (todo > 0)
                        {
                            int toRead;
                            if (todo > int.MaxValue)
                                toRead = int.MaxValue;
                            else toRead = (int)todo;

                            int done = stream.Read(buffer, offset, toRead);
                            if (done != toRead)
                                return completed;

                            completed += done;
                            remain -= done;
                            todo -= done;
                            offset += done;
                            byte_pos += done;
                        }

                        //done reading the raw block; go back to check for another block
                        continue;
                    } //if(type 0)
                    else
                    {
                        //these are sector-based types. they have similar handling.

                        long blockOffset = byte_pos - ie.LogicalOffset;

                        //figure out which sector within the block we're in
                        int outSecSize;
                        int inSecSize;
                        int outSecOffset;
                        if (ie.Type == 1) { outSecSize = 2352; inSecSize = 2048; outSecOffset = 0; }
                        else if (ie.Type == 2) { outSecSize = 2336; inSecSize = 2052; outSecOffset = 16; }
                        else if (ie.Type == 3) { outSecSize = 2336; inSecSize = 2328; outSecOffset = 16; }
                        else throw new InvalidOperationException();

                        long secNumberInBlock = blockOffset / outSecSize;
                        long secOffsetInEcm = secNumberInBlock * outSecSize;
                        long bytesAskedIntoSector = blockOffset % outSecSize;
                        long bytesRemainInSector = outSecSize - bytesAskedIntoSector;

                        long todo = remain;
                        if (bytesRemainInSector < todo)
                            todo = bytesRemainInSector;

                        //move stream to beginning of this sector in ecm
                        stream.Position = ie.ECMOffset + inSecSize * secNumberInBlock;

                        //read and decode the sector
                        switch (ie.Type)
                        {
                            case 1:
                                //TODO - read first 3 bytes
                                if (stream.Read(Read_SectorBuf, 16, 2048) != 2048)
                                    return completed;
                                Reconstruct(Read_SectorBuf, 1);
                                break;
                            case 2:
                                if (stream.Read(Read_SectorBuf, 20, 2052) != 2052)
                                    return completed;
                                Reconstruct(Read_SectorBuf, 2);
                                break;
                            case 3:
                                if (stream.Read(Read_SectorBuf, 20, 2328) != 2328)
                                    return completed;
                                Reconstruct(Read_SectorBuf, 3);
                                break;
                        }

                        //sector is decoded to 2352 bytes. Handling doesnt depend much on type from here

                        Array.Copy(Read_SectorBuf, (int)bytesAskedIntoSector + outSecOffset, buffer, offset, todo);
                        int done = (int)todo;

                        offset += done;
                        completed += done;
                        remain -= done;
                        byte_pos += done;

                    } //not type 0

                } // while(Remain)

                return completed;
            }
        }
    }

    partial class Disc
    {
        internal class Blob_RawFile : IBlob
        {
            public string PhysicalPath
            {
                get
                {
                    return physicalPath;
                }
                set
                {
                    physicalPath = value;
                    length = new FileInfo(physicalPath).Length;
                }
            }
            string physicalPath;
            long length;

            public long Offset = 0;

            BufferedStream fs;
            public void Dispose()
            {
                if (fs != null)
                {
                    fs.Dispose();
                    fs = null;
                }
            }
            public int Read(long byte_pos, byte[] buffer, int offset, int count)
            {
                //use quite a large buffer, because normally we will be reading these sequentially but in small chunks.
                //this enhances performance considerably

                //NOTE: wouldnt very large buffering create stuttering? this would depend on how it's implemented.
                //really, we need a smarter asynchronous read-ahead buffer. that requires substantially more engineering, some kind of 'DiscUniverse' of carefully managed threads and such.

                const int buffersize = 2352 * 75 * 2;
                if (fs == null)
                    fs = new BufferedStream(new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read), buffersize);
                long target = byte_pos + Offset;
                if (fs.Position != target)
                    fs.Position = target;
                return fs.Read(buffer, offset, count);
            }
            public long Length
            {
                get
                {
                    return length;
                }
            }
        }
    }

    partial class Disc
    {
        /// <summary>
        /// TODO - doublecheck that riffmaster is not filling memory at load-time but reading through to the disk
        /// TODO - clarify stream disposing semantics
        /// </summary>
        internal class Blob_WaveFile : IBlob
        {
            [Serializable]
            public class Blob_WaveFile_Exception : Exception
            {
                public Blob_WaveFile_Exception(string message)
                    : base(message)
                {
                }
            }

            public Blob_WaveFile()
            {
            }

            private class Blob_RawFile : IBlob
            {
                public string PhysicalPath
                {
                    get
                    {
                        return physicalPath;
                    }
                    set
                    {
                        physicalPath = value;
                        length = new FileInfo(physicalPath).Length;
                    }
                }
                string physicalPath;
                long length;

                public long Offset = 0;

                BufferedStream fs;
                public void Dispose()
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                        fs = null;
                    }
                }
                public int Read(long byte_pos, byte[] buffer, int offset, int count)
                {
                    //use quite a large buffer, because normally we will be reading these sequentially but in small chunks.
                    //this enhances performance considerably
                    const int buffersize = 2352 * 75 * 2;
                    if (fs == null)
                        fs = new BufferedStream(new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read), buffersize);
                    long target = byte_pos + Offset;
                    if (fs.Position != target)
                        fs.Position = target;
                    return fs.Read(buffer, offset, count);
                }
                public long Length
                {
                    get
                    {
                        return length;
                    }
                }
            }

            public void Load(byte[] waveData)
            {
            }

            public void Load(string wavePath)
            {
                var stream = new FileStream(wavePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                Load(stream);
            }

            public void Load(Stream stream)
            {
                try
                {
                    RiffSource = null;
                    var rm = new RiffMaster();
                    rm.LoadStream(stream);
                    RiffSource = rm;

                    //analyze the file to make sure its an OK wave file

                    if (rm.riff.type != "WAVE")
                    {
                        throw new Blob_WaveFile_Exception("Not a RIFF WAVE file");
                    }

                    var fmt = rm.riff.subchunks.FirstOrDefault(chunk => chunk.tag == "fmt ") as RiffMaster.RiffSubchunk_fmt;
                    if (fmt == null)
                    {
                        throw new Blob_WaveFile_Exception("Not a valid RIFF WAVE file (missing fmt chunk");
                    }

                    if (1 != rm.riff.subchunks.Count(chunk => chunk.tag == "data"))
                    {
                        //later, we could make a Stream which would make an index of data chunks and walk around them
                        throw new Blob_WaveFile_Exception("Multi-data-chunk WAVE files not supported");
                    }

                    if (fmt.format_tag != RiffMaster.RiffSubchunk_fmt.FORMAT_TAG.WAVE_FORMAT_PCM)
                    {
                        throw new Blob_WaveFile_Exception("Not a valid PCM WAVE file (only PCM is supported)");
                    }

                    if (fmt.channels != 2 || fmt.bitsPerSample != 16 || fmt.samplesPerSec != 44100)
                    {
                        throw new Blob_WaveFile_Exception("Not a CDA format WAVE file (conversion not yet supported)");
                    }

                    //acquire the start of the data chunk
                    var dataChunk = rm.riff.subchunks.FirstOrDefault(chunk => chunk.tag == "data") as RiffMaster.RiffSubchunk;
                    waveDataStreamPos = dataChunk.Position;
                    mDataLength = dataChunk.Length;
                }
                catch (Exception)
                {
                    Dispose();
                    throw;
                }
            }

            public int Read(long byte_pos, byte[] buffer, int offset, int count)
            {
                RiffSource.BaseStream.Position = byte_pos + waveDataStreamPos;
                return RiffSource.BaseStream.Read(buffer, offset, count);
            }

            RiffMaster RiffSource;
            long waveDataStreamPos;
            long mDataLength;
            public long Length { get { return mDataLength; } }

            public void Dispose()
            {
                if (RiffSource != null)
                    RiffSource.Dispose();
                RiffSource = null;
            }
        }
    }

    public partial class Disc : IDisposable
    {
        internal sealed class Blob_ZeroPadAdapter : IBlob
        {
            IBlob srcBlob;
            long srcBlobLength;
            public Blob_ZeroPadAdapter(IBlob srcBlob, long srcBlobLength)
            {
                this.srcBlob = srcBlob;
                this.srcBlobLength = srcBlobLength;
            }



            public int Read(long byte_pos, byte[] buffer, int offset, int count)
            {
                int todo = count;
                long end = byte_pos + todo;
                if (end > srcBlobLength)
                {
                    long temp = (int)(srcBlobLength - byte_pos);
                    if (temp > int.MaxValue)
                        throw new InvalidOperationException();
                    todo = (int)temp;

                    //zero-fill the unused part (just for safety's sake)
                    Array.Clear(buffer, offset + todo, count - todo);
                }

                srcBlob.Read(byte_pos, buffer, offset, todo);

                //since it's zero padded, this never fails and always reads the requested amount
                return count;
            }

            public void Dispose()
            {
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Disc() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /*
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        */
        #endregion

    }
}
