using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ExBitStream
{
    /// <summary>
    /// A stream-style reader for retrieving packed bits from a byte array
    /// </summary>
    /// <remarks>This bits should packed into the leftmost position in each byte.
    /// For compatibility purposes with the v1 ISF encoder and decoder, the order of the
    /// packing must not be changed. This code is a from-scratch rewrite of the BitStream
    /// natice C++ class in the v1 Ink code, but still maintaining the same packing
    /// behavior.</remarks>
    public class BitStreamReader
    {
        private List<byte> _targetBuffer = null;

        // Privates
        // reference to the source byte buffer to read from
        //private byte[] _byteArray = null;

        // maximum length of buffer to read in bits
        private int _bufferLengthInBits { get { return _targetBuffer.Count * Native.BitsPerByte - _byteArrayIndex; } }

        // the index in the source buffer for the next byte to be read
        private int _byteArrayIndex = 0;

        // since the bits from multiple inputs can be packed into a single byte
        //  (e.g. 2 size of input fits 4 per byte), we use this field as a cache
        //  of the remaining partial bits.
        private byte _partialByte = 0;

        // the number of bits (partial byte) left to read in the overlapped byte field
        private int _cbitsInPartialByte = 0;

        #region Constructors 
        public BitStreamReader(BitStream stream)
        {
            _targetBuffer = stream.Buffer;
            
        }

        public BitStreamReader(BitStream stream, int startIndex)
        {
            _targetBuffer = stream.Buffer;
            _byteArrayIndex = startIndex;
        }

        /// <summary>
        /// Create a new BitStreamReader to unpack the bits in a buffer of bytes
        /// </summary>
        /// <param name="buffer">Buffer of bytes</param>
        public BitStreamReader(byte[] buffer)
        {
            Debug.Assert(buffer != null);

            _targetBuffer = new List<byte>(buffer);
        }

        /// <summary>
        /// Create a new BitStreamReader to unpack the bits in a buffer of bytes
        /// </summary>
        /// <param name="buffer">Buffer of bytes</param>
        /// <param name="startIndex">The index to start reading at</param>
        public BitStreamReader(byte[] buffer, int startIndex)
        {
            Debug.Assert(buffer != null);

            if (startIndex < 0 || startIndex >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            _targetBuffer = new List<byte>(buffer);
            _byteArrayIndex = startIndex;
        }
        #endregion

        private TValue _Read<TValue, TShifter>(int countOfBits, TShifter shifter) where TShifter : IByteUtil<TValue>
        {
            TValue retVal = default(TValue);
            while (countOfBits > 0)
            {
                int countToRead = (int)Native.BitsPerByte;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                //make room
                retVal = shifter.ShiftLeft(retVal, countToRead);
                byte b = ReadByte(countToRead);
                retVal = shifter.OR(retVal, b);
                countOfBits -= countToRead;
            }
            return retVal;
        }

        private TValue _Read<TValue, TShifter>(int indexOfBits, int countOfBits, TShifter shifter) where TShifter : IByteUtil<TValue>
        {
            TValue retVal = default(TValue);
            while (countOfBits > 0)
            {
                int countToRead = (int)Native.BitsPerByte;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                //make room
                retVal = shifter.ShiftLeft(retVal, countToRead);
                byte b = ReadByte(indexOfBits, countToRead);
                retVal = shifter.OR(retVal, b);
                countOfBits -= countToRead;
                indexOfBits += countToRead;
            }
            return retVal;
        }

        /// <summary>
        /// Reads a single bit from the buffer
        /// </summary>
        /// <returns></returns>
        public bool ReadBit()
        {
            byte b = ReadByte(1);
            return ((b & 1) == 1);
        }

        /// <summary>
        /// Reads a boolean value from the buffer
        /// </summary>
        /// <returns></returns>
        public bool ReadBoolean()
        {
            byte b = ReadByte(1);
            return ((b & 1) == 1);
        }

        public bool ReadBoolean(int indexOfBits)
        {
            byte b = ReadByte(indexOfBits, 1);
            return ((b & 1) == 1);
        }

        /// <summary>
        /// Read a specified number of bits from the stream into a single byte
        /// </summary>
        /// <param name="countOfBits">The number of bits to unpack</param>
        /// <returns>A single byte that contains up to 8 packed bits</returns>
        /// <remarks>For example, if 2 bits are read from the stream, then a full byte
        /// will be created with the least significant bits set to the 2 unpacked bits
        /// from the stream</remarks>
        public byte ReadByte(int countOfBits)
        {
            // if the end of the stream has been reached, then throw an exception
            if (EndOfStream)
            {
                throw new System.IO.EndOfStreamException("Stream is end.");
            }

            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerByte || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of byte.");
            }

            if (countOfBits > _bufferLengthInBits)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Remaining bits is not enough.");
            }

            // initialize return byte to 0 before reading from the cache
            byte returnByte = 0;

            // if the partial bit cache contains more bits than requested, then read the
            //      cache only
            if (_cbitsInPartialByte >= countOfBits)
            {
                // retrieve the requested count of most significant bits from the cache
                //      and store them in the least significant positions in the return byte
                int rightShiftPartialByteBy = Native.BitsPerByte - countOfBits;
                returnByte = (byte)(_partialByte >> rightShiftPartialByteBy);

                // reposition any unused portion of the cache in the most significant part of the bit cache
                unchecked // disable overflow checking since we are intentionally throwing away
                          //  the significant bits
                {
                    _partialByte <<= countOfBits;
                }
                // update the bit count in the cache
                _cbitsInPartialByte -= countOfBits;
            }
            // otherwise, we need to retrieve more full bytes from the stream
            else
            {
                // retrieve the next full byte from the stream
                //byte nextByte = _byteArray[_byteArrayIndex];
                byte nextByte = _targetBuffer[_byteArrayIndex];
                _byteArrayIndex++;

                //right shift partial byte to get it ready to or with the partial next byte
                int rightShiftPartialByteBy = Native.BitsPerByte - countOfBits;
                returnByte = (byte)(_partialByte >> rightShiftPartialByteBy);

                // now copy the remaining chunk of the newly retrieved full byte
                int rightShiftNextByteBy = Math.Abs((countOfBits - _cbitsInPartialByte) - Native.BitsPerByte);
                returnByte |= (byte)(nextByte >> rightShiftNextByteBy);

                // update the partial bit cache with the remainder of the newly retrieved full byte
                unchecked // disable overflow checking since we are intentionally throwing away
                          //  the significant bits
                {
                    _partialByte = (byte)(nextByte << (countOfBits - _cbitsInPartialByte));
                }

                _cbitsInPartialByte = Native.BitsPerByte - (countOfBits - _cbitsInPartialByte);
            }
            return returnByte;
        }

        public byte ReadByte(int indexOfBits, int countOfBits)
        {
            // if the end of the stream has been reached, then throw an exception
            if (EndOfStream)
            {
                throw new System.IO.EndOfStreamException("Stream is end.");
            }

            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerByte || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of byte.");
            }

            if (countOfBits > _bufferLengthInBits)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Remaining bits is not enough.");
            }

            // initialize return byte to 0 before reading from the cache
            byte returnByte = 0;
            int indexOfByte = indexOfBits / Native.BitsPerByte;
            int indexOfPartial = indexOfBits % Native.BitsPerByte;
            byte partialByte = (byte)(_targetBuffer[indexOfByte] << indexOfPartial);
            int cbitsInPartialByte = Native.BitsPerByte - indexOfPartial;

            // if the partial bit cache contains more bits than requested, then read the
            //      cache only
            if (cbitsInPartialByte >= countOfBits)
            {
                // retrieve the requested count of most significant bits from the cache
                //      and store them in the least significant positions in the return byte
                int rightShiftPartialByteBy = Native.BitsPerByte - countOfBits;
                returnByte = (byte)(partialByte >> rightShiftPartialByteBy);

                // reposition any unused portion of the cache in the most significant part of the bit cache
                unchecked // disable overflow checking since we are intentionally throwing away
                          //  the significant bits
                {
                    partialByte <<= countOfBits;
                }
                // update the bit count in the cache
                cbitsInPartialByte -= countOfBits;
            }
            // otherwise, we need to retrieve more full bytes from the stream
            else
            {
                // retrieve the next full byte from the stream
                //byte nextByte = _byteArray[_byteArrayIndex];
                byte nextByte = _targetBuffer[indexOfByte + 1];

                //right shift partial byte to get it ready to or with the partial next byte
                int rightShiftPartialByteBy = Native.BitsPerByte - countOfBits;
                returnByte = (byte)(partialByte >> rightShiftPartialByteBy);

                // now copy the remaining chunk of the newly retrieved full byte
                int rightShiftNextByteBy = Math.Abs((countOfBits - cbitsInPartialByte) - Native.BitsPerByte);
                returnByte |= (byte)(nextByte >> rightShiftNextByteBy);

                // update the partial bit cache with the remainder of the newly retrieved full byte
                unchecked // disable overflow checking since we are intentionally throwing away
                          //  the significant bits
                {
                    partialByte = (byte)(nextByte << (countOfBits - cbitsInPartialByte));
                }

                cbitsInPartialByte = Native.BitsPerByte - (countOfBits - cbitsInPartialByte);
            }
            return returnByte;
        }

        public short ReadInt16(int countOfBits)
        {
            // we only support 1-16 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerShort || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of short.");
            }

            short result = _Read<short, ShortUtil>(countOfBits, ByteUtilManager.ShortUtil);
            return result;
        }

        public short ReadInt16(int indexOfBits, int countOfBits)
        {
            // we only support 1-16 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerShort || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of short.");
            }

            short result = _Read<short, ShortUtil>(indexOfBits, countOfBits, ByteUtilManager.ShortUtil);
            return result;
        }

        /// <summary>
        /// Read a single UInt16 from the byte[]
        /// </summary>
        /// <param name="countOfBits"></param>
        /// <returns></returns>
        public ushort ReadUInt16(int countOfBits)
        {
            // we only support 1-16 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerShort || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of short.");
            }

            ushort result = _Read<ushort, UShortUtil>(countOfBits, ByteUtilManager.UShortUtil);
            return result;
        }

        public ushort ReadUInt16(int indexOfBits, int countOfBits)
        {
            // we only support 1-16 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerShort || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of short.");
            }

            ushort result = _Read<ushort, UShortUtil>(indexOfBits, countOfBits, ByteUtilManager.UShortUtil);
            return result;
        }

        public int ReadInt32(int countOfBits)
        {
            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerInt || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of integer.");
            }

            int result = _Read<int, IntUtil>(countOfBits, ByteUtilManager.IntUtil);
            return result;
        }

        public int ReadInt32(int indexOfBits, int countOfBits)
        {
            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerInt || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of integer.");
            }

            int result = _Read<int, IntUtil>(indexOfBits, countOfBits, ByteUtilManager.IntUtil);
            return result;
        }

        /// <summary>
        /// Read a specified number of bits from the stream into a single byte
        /// </summary>
        public uint ReadUInt32(int countOfBits)
        {
            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerInt || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of integer.");
            }

            uint result = _Read<uint, UIntUtil>(countOfBits, ByteUtilManager.UIntUtil);
            return result;
        }

        public uint ReadUInt32(int indexOfBits, int countOfBits)
        {
            // we only support 1-8 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerInt || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of integer.");
            }

            uint result = _Read<uint, UIntUtil>(indexOfBits, countOfBits, ByteUtilManager.UIntUtil);
            return result;
        }

        /// <summary>
        /// Read a specified number of bits from the stream into a long
        /// </summary>
        public long ReadInt64(int countOfBits)
        {
            // we only support 1-64 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerLong || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of long.");
            }
            long result = _Read<long, LongUtil>(countOfBits, ByteUtilManager.LongUtil);
            return result;
        }

        public long ReadInt64(int indexOfBits, int countOfBits)
        {
            // we only support 1-64 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerLong || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of long.");
            }
            long result = _Read<long, LongUtil>(indexOfBits, countOfBits, ByteUtilManager.LongUtil);
            return result;
        }

        public ulong ReadUInt64(int countOfBits)
        {
            // we only support 1-64 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerLong || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of long.");
            }
            ulong result = _Read<ulong, ULongUtil>(countOfBits, ByteUtilManager.ULongUtil);
            return result;
        }

        public ulong ReadUInt64(int indexOfBits, int countOfBits)
        {
            // we only support 1-64 bits currently, not multiple bytes, and not 0 bits
            if (countOfBits > Native.BitsPerLong || countOfBits <= 0)
            {
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "Count of bits must be greater than zero and less than size of long.");
            }
            ulong result = _Read<ulong, ULongUtil>(indexOfBits, countOfBits, ByteUtilManager.ULongUtil);
            return result;
        }

        public float ReadSingle()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }

        public char ReadChar()
        {
            return (char)ReadByte(Native.BitsPerByte);
        }

        public char ReadChar(int indexOfBits)
        {
            return (char)ReadByte(indexOfBits, Native.BitsPerByte);
        }

        public char[] ReadChars(int countOfChars)
        {
            char[] chars = new char[countOfChars];
            for (int i = 0; i < countOfChars; i++)
            {
                chars[i] = ReadChar();
            }
            return chars;
        }

        public char[] ReadChars(int indexOfBits, int countOfChars)
        {
            char[] chars = new char[countOfChars];
            for (int i = 0; i < countOfChars; i++)
            {
                chars[i] = ReadChar(indexOfBits);
                indexOfBits += Native.BitsPerByte;
            }
            return chars;
        }

        public byte[] ReadBytes(int countOfBytes)
        {
            byte[] bytes = new byte[countOfBytes];
            for(int i = 0; i < countOfBytes; i++)
            {
                bytes[i] = ReadByte(Native.BitsPerByte);
            }
            return bytes;
        }

        public byte[] ReadBytes(int indexOfBits, int countOfBytes)
        {
            byte[] bytes = new byte[countOfBytes];
            for (int i = 0; i < countOfBytes; i++)
            {
                bytes[i] = ReadByte(indexOfBits, Native.BitsPerByte);
                indexOfBits += Native.BitsPerByte;
            }
            return bytes;
        }

        public string ReadString(int lengthOfStr)
        {
            char[] chars = ReadChars(lengthOfStr);
            return new string(chars);
        }

        public string ReadString(int indexOfBits, int lengthOfStr)
        {
            char[] chars = ReadChars(indexOfBits, lengthOfStr);
            return new string(chars);
        }

        /// <summary>
        /// Since the return value of Read cannot distinguish between valid and invalid
        /// data (e.g. 8 bits set), the EndOfStream property detects when there is no more
        /// data to read.
        /// </summary>
        /// <value>True if stream end has been reached</value>
        public bool EndOfStream
        {
            get
            {
                return 0 == _bufferLengthInBits;
            }
        }

        /// <summary>
        /// The current read index in the array
        /// </summary>
        internal int CurrentIndex
        {
            get
            {
                //_byteArrayIndex is always advanced to the next index
                // so we always decrement before returning
                return _byteArrayIndex - 1;
            }
        }


        
    }
}
