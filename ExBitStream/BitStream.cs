using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ExBitStream
{
    public class BitStream
    {
        internal List<byte> Buffer { get; set; }

        public BitStream()
        {
            Buffer = new List<byte>();
        }

        public byte[] ToBytes()
        {
            return Buffer.ToArray();
        }

        public string ToBitString()
        {
            string str = "";
            for(int i = 0; i < Buffer.Count; i++)
            {
                for(int j = 7; j >= 0; j--)
                {
                    if ((Buffer[i] & 1 << j) != 0)
                        str += "1";
                    else
                        str += "0";
                }
                str += " ";
            }
            return str;
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
    }
    
}
