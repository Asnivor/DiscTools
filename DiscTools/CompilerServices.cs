using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using DiscTools.ISO;
using System.Linq;

// you need this once (only), and it must be in this namespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
         | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}
// you can have as many of these as you like, in any namespaces
public static class MyExtensionMethods
{


    public static byte[] ReadAllBytes(this Stream stream)
    {
        const int BUFF_SIZE = 4096;
        var buffer = new byte[BUFF_SIZE];

        int bytesRead;
        var inStream = new BufferedStream(stream);
        var outStream = new MemoryStream();

        while ((bytesRead = inStream.Read(buffer, 0, BUFF_SIZE)) > 0)
        {
            outStream.Write(buffer, 0, bytesRead);
        }

        return outStream.ToArray();
    }

    // Read bytes from a BinaryReader and translate them into the UTF-8 string they represent.
    //WHAT? WHY IS THIS NAMED ASCII BUT USING UTF8
    public static string ReadStringFixedAscii(this BinaryReader r, int bytes)
    {
        var read = new byte[bytes];
        r.Read(read, 0, bytes);
        return Encoding.UTF8.GetString(read);
    }

    public static string ReadStringUtf8NullTerminated(this BinaryReader br)
    {
        MemoryStream ms = new MemoryStream();
        for (;;)
        {
            var b = br.ReadByte();
            if (b == 0)
                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            ms.WriteByte(b);
        }
    }

    public static void CopyTo(this Stream src, Stream dest)
    {
        int size = (src.CanSeek) ? Math.Min((int)(src.Length - src.Position), 0x2000) : 0x2000;
        byte[] buffer = new byte[size];
        int n;
        do
        {
            n = src.Read(buffer, 0, buffer.Length);
            dest.Write(buffer, 0, n);
        } while (n != 0);
    }

    public static void CopyTo(this MemoryStream src, Stream dest)
    {
        dest.Write(src.GetBuffer(), (int)src.Position, (int)(src.Length - src.Position));
    }

    public static void CopyTo(this Stream src, MemoryStream dest)
    {
        if (src.CanSeek)
        {
            int pos = (int)dest.Position;
            int length = (int)(src.Length - src.Position) + pos;
            dest.SetLength(length);

            while (pos < length)
            {
                pos += src.Read(dest.GetBuffer(), pos, length - pos);
            }
        }
        else
        {
            src.CopyTo(dest);
        }
    }

    public static void Write(this BinaryWriter bw, int[] buffer)
    {
        foreach (int b in buffer)
        {
            bw.Write(b);
        }
    }

    public static void Write(this BinaryWriter bw, uint[] buffer)
    {
        foreach (uint b in buffer)
        {
            bw.Write(b);
        }
    }

    public static void Write(this BinaryWriter bw, short[] buffer)
    {
        foreach (short b in buffer)
        {
            bw.Write(b);
        }
    }

    public static void Write(this BinaryWriter bw, ushort[] buffer)
    {
        foreach (ushort t in buffer)
        {
            bw.Write(t);
        }
    }

    public static int[] ReadInt32s(this BinaryReader br, int num)
    {
        int[] ret = new int[num];
        for (int i = 0; i < num; i++)
        {
            ret[i] = br.ReadInt32();
        }

        return ret;
    }

    public static short[] ReadInt16s(this BinaryReader br, int num)
    {
        short[] ret = new short[num];
        for (int i = 0; i < num; i++)
        {
            ret[i] = br.ReadInt16();
        }

        return ret;
    }

    public static ushort[] ReadUInt16s(this BinaryReader br, int num)
    {
        ushort[] ret = new ushort[num];
        for (int i = 0; i < num; i++)
        {
            ret[i] = br.ReadUInt16();
        }

        return ret;
    }

    public static void WriteBit(this BinaryWriter bw, Bit bit)
    {
        bw.Write((bool)bit);
    }

    public static Bit ReadBit(this BinaryReader br)
    {
        return br.ReadBoolean();
    }

    public static string ToHexString(this int n, int numdigits)
    {
        return string.Format("{0:X" + numdigits + "}", n);
    }

    public static string ToHexString(this uint n, int numdigits)
    {
        return string.Format("{0:X" + numdigits + "}", n);
    }

    public static string ToHexString(this byte n, int numdigits)
    {
        return string.Format("{0:X" + numdigits + "}", n);
    }

    public static string ToHexString(this ushort n, int numdigits)
    {
        return string.Format("{0:X" + numdigits + "}", n);
    }

    public static string ToHexString(this long n, int numdigits)
    {
        return string.Format("{0:X" + numdigits + "}", n);
    }

    public static string ToHexString(this ulong n, int numdigits)
    {
        return string.Format("{0:X" + numdigits + "}", n);
    }

    public static bool Bit(this byte b, int index)
    {
        return (b & (1 << index)) != 0;
    }

    public static bool Bit(this int b, int index)
    {
        return (b & (1 << index)) != 0;
    }

    public static bool Bit(this ushort b, int index)
    {
        return (b & (1 << index)) != 0;
    }

    public static bool In(this int i, params int[] options)
    {
        return options.Any(j => i == j);
    }

    public static byte BinToBCD(this byte v)
    {
        return (byte)(((v / 10) * 16) + (v % 10));
    }

    public static byte BCDtoBin(this byte v)
    {
        return (byte)(((v / 16) * 10) + (v % 16));
    }

    /// <summary>
    /// Receives a number and returns the number of hexadecimal digits it is
    /// Note: currently only returns 2, 4, 6, or 8
    /// </summary>
    public static int NumHexDigits(this long i)
    {
        //now this is a bit of a trick. if it was less than 0, it mustve been >= 0x80000000 and so takes all 8 digits
        if (i < 0)
        {
            return 8;
        }

        if (i < 0x100)
        {
            return 2;
        }

        if (i < 0x10000)
        {
            return 4;
        }

        if (i < 0x1000000)
        {
            return 6;
        }

        if (i < 0x100000000)
        {
            return 8;
        }

        return 16;
    }

    /// <summary>
    /// The % operator is a remainder operator. (e.g. -1 mod 4 returns -1, not 3.)
    /// </summary>
    public static int Mod(this int a, int b)
    {
        return a - (b * (int)System.Math.Floor((float)a / b));
    }

    /// <summary>
    /// Force the value to be stricly between min and max (both exclued)
    /// </summary>
    /// <typeparam name="T">Anything that implements <see cref="IComparable{T}"/></typeparam>
    /// <param name="val">Value that will be clamped</param>
    /// <param name="min">Minimum allowed</param>
    /// <param name="max">Maximum allowed</param>
    /// <returns>The value if strictly between min and max; otherwise min (or max depending of what is passed)</returns>
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0)
        {
            return min;
        }
        else if (val.CompareTo(max) > 0)
        {
            return max;
        }
        else
        {
            return val;
        }
    }

    public static void SaveAsHex(this byte[] buffer, TextWriter writer)
    {
        foreach (var b in buffer)
        {
            writer.Write("{0:X2}", b);
        }

        writer.WriteLine();
    }


    public unsafe static void SaveAsHexFast(this byte[] buffer, TextWriter writer)
    {
        char* table = Util.HexConvPtr;
        if (buffer.Length > 0)
        {
            int len = buffer.Length;
            fixed (byte* src = &buffer[0])
                for (int i = 0; i < len; i++)
                {
                    writer.Write(table[src[i] >> 4]);
                    writer.Write(table[src[i] & 15]);
                }
        }
        writer.WriteLine();
    }

    public static void SaveAsHex(this byte[] buffer, TextWriter writer, int length)
    {
        for (int i = 0; i < length; i++)
        {
            writer.Write("{0:X2}", buffer[i]);
        }
        writer.WriteLine();
    }

    public static void SaveAsHex(this short[] buffer, TextWriter writer)
    {
        foreach (var b in buffer)
        {
            writer.Write("{0:X4}", b);
        }
        writer.WriteLine();
    }

    public static void SaveAsHex(this ushort[] buffer, TextWriter writer)
    {
        foreach (var b in buffer)
        {
            writer.Write("{0:X4}", b);
        }
        writer.WriteLine();
    }

    public static void SaveAsHex(this int[] buffer, TextWriter writer)
    {
        foreach (int b in buffer)
        {
            writer.Write("{0:X8}", b);
        }
        writer.WriteLine();
    }

    public static void SaveAsHex(this uint[] buffer, TextWriter writer)
    {
        foreach (var b in buffer)
        {
            writer.Write("{0:X8}", b);
        }
        writer.WriteLine();
    }

    public static void ReadFromHex(this byte[] buffer, string hex)
    {
        if (hex.Length % 2 != 0)
        {
            throw new Exception("Hex value string does not appear to be properly formatted.");
        }

        for (int i = 0; i < buffer.Length && i * 2 < hex.Length; i++)
        {
            var bytehex = "" + hex[i * 2] + hex[i * 2 + 1];
            buffer[i] = byte.Parse(bytehex, NumberStyles.HexNumber);
        }
    }

    public static unsafe void ReadFromHexFast(this byte[] buffer, string hex)
    {
        if (buffer.Length * 2 != hex.Length)
        {
            throw new Exception("Data size mismatch");
        }

        int count = buffer.Length;
        fixed (byte* _dst = buffer)
        fixed (char* _src = hex)
        {
            byte* dst = _dst;
            char* src = _src;
            while (count > 0)
            {
                // in my tests, replacing Hex2Int() with a 256 entry LUT slowed things down slightly
                *dst++ = (byte)(Hex2Int(*src++) << 4 | Hex2Int(*src++));
                count--;
            }
        }
    }

    public static void ReadFromHex(this short[] buffer, string hex)
    {
        if (hex.Length % 4 != 0)
        {
            throw new Exception("Hex value string does not appear to be properly formatted.");
        }

        for (int i = 0; i < buffer.Length && i * 4 < hex.Length; i++)
        {
            var shorthex = "" + hex[i * 4] + hex[(i * 4) + 1] + hex[(i * 4) + 2] + hex[(i * 4) + 3];
            buffer[i] = short.Parse(shorthex, NumberStyles.HexNumber);
        }
    }

    public static void ReadFromHex(this ushort[] buffer, string hex)
    {
        if (hex.Length % 4 != 0)
        {
            throw new Exception("Hex value string does not appear to be properly formatted.");
        }

        for (int i = 0; i < buffer.Length && i * 4 < hex.Length; i++)
        {
            var ushorthex = "" + hex[i * 4] + hex[(i * 4) + 1] + hex[(i * 4) + 2] + hex[(i * 4) + 3];
            buffer[i] = ushort.Parse(ushorthex, NumberStyles.HexNumber);
        }
    }

    public static void ReadFromHex(this int[] buffer, string hex)
    {
        if (hex.Length % 8 != 0)
        {
            throw new Exception("Hex value string does not appear to be properly formatted.");
        }

        for (int i = 0; i < buffer.Length && i * 8 < hex.Length; i++)
        {
            //string inthex = "" + hex[i * 8] + hex[(i * 8) + 1] + hex[(i * 4) + 2] + hex[(i * 4) + 3] + hex[(i*4
            var inthex = hex.Substring(i * 8, 8);
            buffer[i] = int.Parse(inthex, NumberStyles.HexNumber);
        }
    }

    /// <summary>
    /// Converts bytes to an uppercase string of hex numbers in upper case without any spacing or anything
    /// </summary>
    public static string BytesToHexString(this byte[] bytes)
    {
        var sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.AppendFormat("{0:X2}", b);
        }

        return sb.ToString();
    }

    public static bool FindBytes(this byte[] array, byte[] pattern)
    {
        var fidx = 0;
        int result = Array.FindIndex(array, 0, array.Length, (byte b) =>
        {
            fidx = (b == pattern[fidx]) ? fidx + 1 : 0;
            return (fidx == pattern.Length);
        });

        return (result >= pattern.Length - 1);
    }

    public static string HashMD5(this byte[] data, int offset, int len)
    {
        using (var md5 = MD5.Create())
        {
            md5.ComputeHash(data, offset, len);
            return md5.Hash.BytesToHexString();
        }
    }

    public static string HashMD5(this byte[] data)
    {
        return HashMD5(data, 0, data.Length);
    }

    public static string HashSHA1(this byte[] data, int offset, int len)
    {
        using (var sha1 = SHA1.Create())
        {
            sha1.ComputeHash(data, offset, len);
            return sha1.Hash.BytesToHexString();
        }
    }

    public static string HashSHA1(this byte[] data)
    {
        return HashSHA1(data, 0, data.Length);
    }

    #region Helpers

    private static int Hex2Int(char c)
    {
        if (c <= '9')
        {
            return c - '0';
        }

        if (c <= 'F')
        {
            return c - '7';
        }

        return c - 'W';
    }

    public static int LowerBoundBinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key) where TKey : IComparable<TKey>
    {
        int min = 0;
        int max = list.Count;
        int mid;
        TKey midKey;
        while (min < max)
        {
            mid = (max + min) / 2;
            T midItem = list[mid];
            midKey = keySelector(midItem);
            int comp = midKey.CompareTo(key);
            if (comp < 0)
            {
                min = mid + 1;
            }
            else if (comp > 0)
            {
                max = mid - 1;
            }
            else
            {
                return mid;
            }
        }

        //did we find it exactly?
        if (min == max && keySelector(list[min]).CompareTo(key) == 0)
        {
            return min;
        }

        mid = min;

        //we didnt find it. return something corresponding to lower_bound semantics

        if (mid == list.Count)
        {
            return max; // had to go all the way to max before giving up; lower bound is max
        }

        if (mid == 0)
        {
            return -1; // had to go all the way to min before giving up; lower bound is min
        }

        midKey = keySelector(list[mid]);
        if (midKey.CompareTo(key) >= 0)
        {
            return mid - 1;
        }

        return mid;
    }

    // http://stackoverflow.com/questions/1766328/can-linq-use-binary-search-when-the-collection-is-ordered
    public static T BinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
    where TKey : IComparable<TKey>
    {
        int min = 0;
        int max = list.Count;
        while (min < max)
        {
            int mid = (max + min) / 2;
            T midItem = list[mid];
            TKey midKey = keySelector(midItem);
            int comp = midKey.CompareTo(key);
            if (comp < 0)
            {
                min = mid + 1;
            }
            else if (comp > 0)
            {
                max = mid - 1;
            }
            else
            {
                return midItem;
            }
        }
        if (min == max &&
            keySelector(list[min]).CompareTo(key) == 0)
        {
            return list[min];
        }

        throw new InvalidOperationException("Item not found");
    }

    public static byte[] ToByteArray(this IEnumerable<bool> list)
    {
        var bits = new BitArray(list.ToArray());
        byte[] bytes = new byte[bits.Length / 8 + (bits.Length % 8 == 0 ? 0 : 1)];
        bits.CopyTo(bytes, 0);
        return bytes;
    }

    /// <summary>
    /// Converts any byte array into a bit array represented as a list of bools
    /// </summary>
    public static IEnumerable<bool> ToBools(this byte[] bytes)
    {
        var bits = new BitArray(bytes);
        var bools = new bool[bits.Length];
        bits.CopyTo(bools, 0);

        return bools;
    }

    #endregion
}
