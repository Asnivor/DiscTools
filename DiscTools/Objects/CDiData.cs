using DiscTools.Inspection.Statics.SonyMethods;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public class CDiVolumeDescriptor
    {
  /*    BP Size in Bytes Description
         1 1 Disc Label Record Type
         2 5 Volume Structure Standard ID
         7 1 Volume Structure Version number
         8 1 Volume flags
         9 32 System identifier
         41 32 Volume identifier
         73 12 Reserved
         85 4 Volume space size
         89 32 Coded Character Set identifier
         121 2 Reserved
         123 2 Number of Volumes in Album
         125 2 Reserved
         127 2 Album Set Sequence number
         129 2 Reserved
         131 2 Logical Block size
         133 4 Reserved
         137 4 Path Table size
         141 8 Reserved
         149 4 Address of Path Table
         153 38 Reserved
         191 128 Album identifier
         319 128 Publisher identifier
         447 128 Data Preparer identifier
         575 128 Application identifier
         703 32 Copyright file name
         735 5 Reserved
         740 32 Abstract file name
         772 5 Reserved
         777 32 Bibliographic file name
         809 5 Reserved
         814 16 Creation date and time
         830 1 Reserved
         831 16 Modification date and time
         847 1 Reserved
         848 16 Expiration date and time
         864 1 Reserved
         865 16 Effective date and time
         881 1 Reserved
         882 1 File Structure Standard Version number
         883 1 Reserved
         884 512 Application use
        1396 653 Reserved                      */

        public int? DiscLabelRecordType { get; set; }
        public string VolumeStructureStandardID { get; set; }
        public int? VolumeStructureVersionNumber { get; set; }
        public int? VolumeFlags { get; set; }
        public string SystemIdentifier { get; set; }
        public string VolumeIdentifier { get; set; }
        public int? VolumeSpaceSize { get; set; }
        public string CodedCharSetIndent { get; set; }
        public int? NumberOfVolumesInAlbum { get; set; }
        public int? AlbumSetSequenceNumber { get; set; }
        public int? LogicalBlockSize { get; set; }
        public int? PathTableSize { get; set; }
        public int? AddressOfPathTable { get; set; }
        public string AlbumIdentifier { get; set; }
        public string PublisherIdentifier { get; set; }
        public string DataPreparerIdentifier { get; set; }
        public string ApplicationIdentifier { get; set; }
        public string CopyrightFileName { get; set; }
        public string AbstractFileName { get; set; }
        public string BibliographicFileName { get; set; }
        public string CreationDateAndTime { get; set; }
        public string ModificationDateAndTime { get; set; }
        public string ExpirationDateAndTime { get; set; }
        public string EffectiveDateAndTime { get; set; }
        public int? FileStructureStandardVersionNumber { get; set; }
        public string ApplicationUse { get; set; }

        public byte[] _Data { get; set; }
        public string _DataString { get; set; }


        public void ParseData()
        {
            DiscLabelRecordType = ReadIntValueLE(0, 1);
            VolumeStructureStandardID = Encoding.ASCII.GetString(ReadBytes(1, 5)).TrimEnd();
            VolumeStructureVersionNumber = ReadIntValueLE(6, 1);
            VolumeFlags = ReadIntValueLE(7, 1);
            SystemIdentifier = Encoding.ASCII.GetString(ReadBytes(8, 32)).TrimEnd();
            VolumeIdentifier = Encoding.ASCII.GetString(ReadBytes(40, 32)).TrimEnd();
            VolumeSpaceSize = ReadIntValueLE(84, 4);
            CodedCharSetIndent = Encoding.ASCII.GetString(ReadBytes(88, 32)).TrimEnd();
            NumberOfVolumesInAlbum = ReadIntValueLE(122, 2);
            AlbumSetSequenceNumber = ReadIntValueLE(128, 2);
            LogicalBlockSize = ReadIntValueLE(130, 2);
            PathTableSize = ReadIntValueLE(126, 4);
            AddressOfPathTable = ReadIntValueLE(148, 4);
            AlbumIdentifier = Encoding.ASCII.GetString(ReadBytes(190, 128)).TrimEnd();
            PublisherIdentifier = Encoding.ASCII.GetString(ReadBytes(318, 128)).TrimEnd();
            DataPreparerIdentifier = Encoding.ASCII.GetString(ReadBytes(446, 128)).TrimEnd();
            ApplicationIdentifier = Encoding.ASCII.GetString(ReadBytes(574, 128)).TrimEnd();
            CopyrightFileName = Encoding.ASCII.GetString(ReadBytes(702, 32)).TrimEnd();
            AbstractFileName = Encoding.ASCII.GetString(ReadBytes(739, 32)).TrimEnd();
            BibliographicFileName = Encoding.ASCII.GetString(ReadBytes(776, 32)).TrimEnd();
            CreationDateAndTime = Encoding.ASCII.GetString(ReadBytes(813, 16)).TrimEnd();
            ModificationDateAndTime = Encoding.ASCII.GetString(ReadBytes(830, 16)).TrimEnd();
            ExpirationDateAndTime = Encoding.ASCII.GetString(ReadBytes(847, 16)).TrimEnd();
            EffectiveDateAndTime = Encoding.ASCII.GetString(ReadBytes(864, 16)).TrimEnd();
            FileStructureStandardVersionNumber = ReadIntValueLE(881, 1);
            ApplicationUse = Encoding.ASCII.GetString(ReadBytes(883, 512)).TrimEnd();
        }

        public CDiVolumeDescriptor(byte[] sector)
        {
            _DataString = Encoding.Default.GetString(sector);
            _Data = sector;

            ParseData();
        }

        public CDiVolumeDescriptor(string sectorString)
        {
            _DataString = sectorString;
            _Data = Encoding.ASCII.GetBytes(sectorString);

            ParseData();
        }



        public string ReadHexString(int offset, int length)
        {
            var bytes = _Data.Skip(offset).Take(length).ToArray();
            string hex = "";
            foreach (byte b in bytes)
            {
                hex += b.ToString("X2");
            }
            return hex;
        }

        public string ReadHexStringLE(int offset, int length)
        {
            var bytes = _Data.Skip(offset).Take(length).ToArray();
            Array.Reverse(bytes);
            string hex = "";
            foreach (byte b in bytes)
            {
                hex += b.ToString("X2");
            }
            return hex;
        }

        public byte[] ReadBytes(int offset, int length)
        {
            var bytes = _Data.Skip(offset).Take(length).ToArray();
            return bytes;
        }

        public int[] ReadInts(int offset, int length)
        {
            var bytes = _Data.Skip(offset).Take(length).ToArray();
            List<int> list = new List<int>();
            foreach (byte b in bytes)
            {
                list.Add(Convert.ToInt32(b));
            }

            return list.ToArray();
        }

        public int ReadIntValueLE(int offset, int length)
        {
            var bytes = _Data.Skip(offset).Take(length).ToArray();
            Array.Reverse(bytes);
            if (length == 1)
                return bytes.FirstOrDefault();

            if (length == 2)
                return BitConverter.ToInt16(bytes, 0);

            int result = BitConverter.ToInt32(bytes, 0);
            return result;
        }

        public int ReadIntValue(int offset, int length)
        {
            var bytes = _Data.Skip(offset).Take(length).ToArray();
            //Array.Reverse(bytes);

            if (length == 1)
                return bytes.FirstOrDefault();

            if (length == 2)
                return BitConverter.ToInt16(bytes, 0);

            int result = BitConverter.ToInt32(bytes, 0);
            return result;
        }

        public int ReadInt16ValueLE(int offset, int length)
        {
            var bytes = _Data.Skip(offset).Take(length).ToArray();
            Array.Reverse(bytes);
            int result = BitConverter.ToInt16(bytes, 0);
            return result;
        }

        public int ReadInt16Value(int offset, int length)
        {
            var bytes = _Data.Skip(offset).Take(length).ToArray();
            //Array.Reverse(bytes);
            int result = BitConverter.ToInt16(bytes, 0);
            return result;
        }
    }
    
}
