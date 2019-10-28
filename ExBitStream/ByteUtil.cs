using System;
using System.Collections.Generic;
using System.Text;

namespace ExBitStream
{
    internal interface IByteUtil<T>
    {
        byte ShiftRight(T value, int bits);
        T ShiftLeft(T value, int bits);
        T OR(T value, byte b);
    }

    internal class ByteUtilManager
    {
        public static readonly IntUtil IntUtil = new IntUtil();
        public static readonly UIntUtil UIntUtil = new UIntUtil();
        public static readonly ShortUtil ShortUtil = new ShortUtil();
        public static readonly UShortUtil UShortUtil = new UShortUtil();
        public static readonly LongUtil LongUtil = new LongUtil();
        public static readonly ULongUtil ULongUtil = new ULongUtil();
        public static readonly CharUtil CharUtil = new CharUtil();
    }

    internal class IntUtil : IByteUtil<int>
    {
        public byte ShiftRight(int value, int bits)
        {
            return (byte)(value >> bits);
        }

        public int ShiftLeft(int value, int bits)
        {
            return value << bits;
        }

        public int OR(int value, byte b)
        {
            return value | b;
        }
    }

    internal class UIntUtil : IByteUtil<uint>
    {
        public byte ShiftRight(uint value, int bits)
        {
            return (byte)(value >> bits);
        }

        public uint ShiftLeft(uint value, int bits)
        {
            return value << bits;
        }

        public uint OR(uint value, byte b)
        {
            return value | b;
        }
    }

    internal class ShortUtil : IByteUtil<short>
    {
        public byte ShiftRight(short value, int bits)
        {
            return (byte)(value >> bits);
        }

        public short ShiftLeft(short value, int bits)
        {
            return (short)(value << bits);
        }

        public short OR(short value, byte b)
        {
            return (short)(value | b);
        }
    }

    internal class UShortUtil : IByteUtil<ushort>
    {
        public byte ShiftRight(ushort value, int bits)
        {
            return (byte)(value >> bits);
        }

        public ushort ShiftLeft(ushort value, int bits)
        {
            return (ushort)(value << bits);
        }

        public ushort OR(ushort value, byte b)
        {
            return (ushort)(value | b);
        }
    }

    internal class LongUtil : IByteUtil<long>
    {
        public byte ShiftRight(long value, int bits)
        {
            return (byte)(value >> bits);
        }

        public long ShiftLeft(long value, int bits)
        {
            return value << bits;
        }

        public long OR(long value, byte b)
        {
            return (long)(value | b);
        }
    }

    internal class ULongUtil : IByteUtil<ulong>
    {
        public byte ShiftRight(ulong value, int bits)
        {
            return (byte)(value >> bits);
        }

        public ulong ShiftLeft(ulong value, int bits)
        {
            return value << bits;
        }

        public ulong OR(ulong value, byte b)
        {
            return (ulong)(value | b);
        }
    }

    internal class CharUtil : IByteUtil<char>
    {
        public byte ShiftRight(char value, int bits)
        {
            return (byte)(value >> bits);
        }

        public char ShiftLeft(char value, int bits)
        {
            return (char)(value << bits);
        }

        public char OR(char value, byte b)
        {
            return (char)(value | b);
        }
    }
}
