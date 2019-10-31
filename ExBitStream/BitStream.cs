using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ExBitStream
{
    public class BitStream
    {
        internal List<byte> Buffer { get; set; }

        public int CountOfBits { get; internal set; }

        public BitStream()
        {
            Buffer = new List<byte>();
        }

        public byte[] ToBytes()
        {
            return Buffer.ToArray();
        }

        public string ToByteString()
        {
            return Buffer.ToByteString();
        }
    }

    public static class ByteExtension
    {
        public static string ToBitString(this byte b)
        {
            return new string(new char[]
            {
                (b & 0x80) != 0 ? '1' : '0',
                (b & 0x40) != 0 ? '1' : '0',
                (b & 0x20) != 0 ? '1' : '0',
                (b & 0x10) != 0 ? '1' : '0',
                (b & 0x08) != 0 ? '1' : '0',
                (b & 0x04) != 0 ? '1' : '0',
                (b & 0x02) != 0 ? '1' : '0',
                (b & 0x01) != 0 ? '1' : '0',
            });
        }

        public static string ToByteString(this IList<byte> bytes)
        {
            if (bytes.Count <= 0)
                return "";
            string byteStr = bytes[0].ToBitString();
            for (int i = 1; i < bytes.Count; i++)
            {
                byteStr += " " + bytes[i].ToBitString();
            }
            return byteStr;
        }

        public static string ToByteString(this byte[] bytes)
        {
            if (bytes.Length <= 0)
                return "";
            string byteStr = bytes[0].ToBitString();
            for(int i = 1; i < bytes.Length; i++)
            {
                byteStr += " " + bytes[i].ToBitString();
            }
            return byteStr;
        }
    }
    
}
