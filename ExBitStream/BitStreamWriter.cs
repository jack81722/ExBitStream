using System;
using System.Collections.Generic;
using System.Text;

namespace ExBitStream
{
    /// <summary>
    /// A stream-like writer for packing bits into a byte buffer
    /// </summary>
    /// <remarks>This class is to be used with the BitStreamReader for reading
    /// and writing bytes. Note that the bytes should be read in the same order
    /// and lengths as they were written to retrieve the same values.
    /// See remarks in BitStreamReader regarding compatibility with the native C++
    /// BitStream class.</remarks>
    public class BitStreamWriter : IBitWriter
    {
        /// <summary>
        /// The target stream
        /// </summary>
        private BitStream _targetStream;

        /// <summary>
        /// The buffer that the bits are written into
        /// </summary>
        private List<byte> _targetBuffer = null;

        /// <summary>
        /// Number of free bits remaining in the last byte added to the target buffer
        /// </summary>
        private int _remaining = 0;

        public BitStreamWriter(BitStream stream)
        {
            _targetStream = stream;
            _targetBuffer = stream.Buffer;
            _remaining = (Native.BitsPerByte - (stream.CountOfBits % Native.BitsPerByte)) % Native.BitsPerByte;
        }

        /// <summary>
        /// Write bits into target buffer
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <typeparam name="TUtil">operation utility type</typeparam>
        /// <param name="value">value which will be written</param>
        /// <param name="util">byte operation utility</param>
        /// <param name="countOfBits">bits of value</param>
        private void _Write<TValue, TUtil>(TValue value, TUtil util, int countOfBits) where TUtil : IByteUtil<TValue>
        {
            // calculate the value of full bytes
            int fullBytes = countOfBits / Native.BitsPerByte;

            // calculate the value of bits that spill beyond the full byte boundary
            int bitsToWrite = countOfBits % Native.BitsPerByte;

            // foreach bytes of value
            for (; fullBytes >= 0; fullBytes--)
            {
                byte byteOfData = util.ShiftRight(value, fullBytes * Native.BitsPerByte);
                //
                // write 8 or less bytes to the bitwriter
                // checking for 0 handles the case where we're writing 8, 16 or 24 bytes
                // and bitsToWrite is initialize to zero
                //
                if (bitsToWrite > 0)
                {
                    Write(byteOfData, bitsToWrite);
                }
                if (fullBytes > 0)
                {
                    bitsToWrite = Native.BitsPerByte;
                }
            }
        }

        /// <summary>
        /// Write a bit of boolean into the stream
        /// </summary>
        /// <param name="value">The boolean to read the bits from</param>
        public void Write(bool value)
        {   
            Write((byte)1, 1);
        }

        /// <summary>
        /// Write a specific number of bits from byte input into the stream
        /// </summary>
        /// <param name="value">The byte to read the bits from</param>
        /// <param name="bits">The number of bits to read</param>
        public void Write(byte value, int bits = Native.BitsPerByte)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerByte)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of byte.");

            byte buffer;
            // if there is remaining bits in the last byte in the stream
            //      then use those first
            if (_remaining > 0)
            {
                // retrieve the last byte from the stream, update it, and then replace it
                buffer = _targetBuffer[_targetBuffer.Count - 1];
                // if the remaining bits aren't enough then just copy the significant bits
                //      of the input into the remainder
                if (bits > _remaining)
                {
                    buffer |= (byte)((value & (0xFF >> (Native.BitsPerByte - bits))) >> (bits - _remaining));
                }
                // otherwise, copy the entire set of input bits into the remainder
                else
                {
                    buffer |= (byte)((value & (0xFF >> (Native.BitsPerByte - bits))) << (_remaining - bits));
                }
                _targetBuffer[_targetBuffer.Count - 1] = buffer;
            }

            // if the remainder wasn't large enough to hold the entire input set
            if (bits > _remaining)
            {
                // then copy the uncontained portion of the input set into a temporary byte
                _remaining = Native.BitsPerByte - (bits - _remaining);
                unchecked // disable overflow checking since we are intentionally throwing away
                          //  the significant bits
                {
                    buffer = (byte)(value << _remaining);
                }
                // and add it to the target buffer
                _targetBuffer.Add(buffer);
            }
            else
            {
                // otherwise, simply update the amount of remaining bits we have to spare
                _remaining -= bits;
            }
            _targetStream.CountOfBits += bits;
        }

        /// <summary>
        /// Write a specific number of bits from sbyte input into the stream
        /// </summary>
        /// <param name="value">The sbyte to read the bits from</param>
        /// <param name="bits">The number of bits to read</param>
        public void Write(sbyte value, int bits = Native.BitsPerByte)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerByte)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of byte.");

            byte buffer;
            // if there is remaining bits in the last byte in the stream
            //      then use those first
            if (_remaining > 0)
            {
                // retrieve the last byte from the stream, update it, and then replace it
                buffer = _targetBuffer[_targetBuffer.Count - 1];
                // if the remaining bits aren't enough then just copy the significant bits
                //      of the input into the remainder
                if (bits > _remaining)
                {
                    buffer |= (byte)((value & (0xFF >> (Native.BitsPerByte - bits))) >> (bits - _remaining));
                }
                // otherwise, copy the entire set of input bits into the remainder
                else
                {
                    buffer |= (byte)((value & (0xFF >> (Native.BitsPerByte - bits))) << (_remaining - bits));
                }
                _targetBuffer[_targetBuffer.Count - 1] = buffer;
            }

            // if the remainder wasn't large enough to hold the entire input set
            if (bits > _remaining)
            {
                // then copy the uncontained portion of the input set into a temporary byte
                _remaining = Native.BitsPerByte - (bits - _remaining);
                unchecked // disable overflow checking since we are intentionally throwing away
                          //  the significant bits
                {
                    buffer = (byte)(value << _remaining);
                }
                // and add it to the target buffer
                _targetBuffer.Add(buffer);
            }
            else
            {
                // otherwise, simply update the amount of remaining bits we have to spare
                _remaining -= bits;
            }
            _targetStream.CountOfBits += bits;
        }

        /// <summary>
        /// Write a specific number of bits from short input into the stream
        /// </summary>
        /// <param name="value">The short to read the bits from</param>
        /// <param name="bits">The number of bits to read</param>
        public void Write(short value, int bits = Native.BitsPerShort)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerShort)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of short.");


            _Write<short, ShortUtil>(value, ByteUtilManager.ShortUtil, bits);
        }

        /// <summary>
        /// Write a specific number of bits from ushort input into the stream
        /// </summary>
        /// <param name="value">The ushort to read the bits from</param>
        /// <param name="bits">The number of bits to read</param>
        public void Write(ushort value, int bits = Native.BitsPerShort)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerShort)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of ushort.");


            _Write<ushort, UShortUtil>(value, ByteUtilManager.UShortUtil, bits);
        }

        /// <summary>
        /// Write a specific number of bits from integer input into the stream
        /// </summary>
        /// <param name="value">The integer to read the bits from</param>
        /// <param name="bits">The number of bits to read</param>
        public void Write(int value, int bits = Native.BitsPerInt)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerInt)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of integer.");

            IntUtil shifter = ByteUtilManager.IntUtil;
            _Write(value, shifter, bits);
        }

        /// <summary>
        /// Write a specific number of bits from unsigned integer input into the stream
        /// </summary>
        /// <param name="value">The unsigned integer to read the bits from</param>
        /// <param name="bits">The number of bits to read</param>
        public void Write(uint value, int bits = Native.BitsPerInt)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerInt)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of unsigned integer.");


            UIntUtil shifter = new UIntUtil();
            _Write(value, shifter, bits);
        }

        /// <summary>
        /// Write a specific number of bits from long input into the stream
        /// </summary>
        /// <param name="value">The long to read the bits from</param>
        /// <param name="bits">The number of bits to read</param>
        public void Write(long value, int bits = Native.BitsPerLong)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerLong)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of long.");

            LongUtil shifter = new LongUtil();
            _Write(value, shifter, bits);
        }

        /// <summary>
        /// Write a specific number of bits from unsigned long input into the stream
        /// </summary>
        /// <param name="value">The unsigned long to read the bits from</param>
        /// <param name="bits">The number of bits to read</param>
        public void Write(ulong value, int bits = Native.BitsPerLong)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerLong)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of integer.");


            ULongUtil shifter = new ULongUtil();
            _Write(value, shifter, bits);
        }

        /// <summary>
        /// Write a byte array from single input into the stream
        /// </summary>
        /// <param name="value">The single to read the bytes from</param>
        public void Write(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            Write(bytes);
        }

        /// <summary>
        /// Write a byte array from double input into the stream
        /// </summary>
        /// <param name="value">The double to read the bytes from</param>
        public void Write(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            Write(bytes);
        }

        /// <summary>
        /// Write a byte array into the stream
        /// </summary>
        public void Write(byte[] buffer)
        {
            for(int i = 0; i < buffer.Length; i++)
            {
                Write(buffer[i]);
            }
        }

        /// <summary>
        /// Write a specific number of bits from character input into the stream
        /// </summary>
        /// <param name="value">The character to read the bits from</param>
        /// <param name="bits">The number of bits to read</param>
        public void Write(char ch, int bits = Native.BitsPerByte)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerLong)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of integer.");


            CharUtil shifter = new CharUtil();
            _Write(ch, shifter, bits);
        }

        /// <summary>
        /// Write a byte array from character array input into the stream
        /// </summary>
        /// <param name="value">The character to read the bytes from</param>
        public void Write(char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                Write(chars[i]);
            }
        }

        /// <summary>
        /// Write a byte array from string input into the stream
        /// </summary>
        /// <param name="value">The string to read the bytes from</param>
        public void Write(string value)
        {
            var chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                Write(chars[i]);
            }
        }
    }

    public interface IBitWriter
    {
        void Write(bool value);
        void Write(byte value, int bits);
        void Write(sbyte value, int bits);
        void Write(short value, int bits);
        void Write(ushort value, int bits);
        void Write(int value, int bits);
        void Write(uint value, int bits);
        void Write(long value, int bits);
        void Write(ulong value, int bits);
        void Write(float value);
        void Write(double value);
        void Write(byte[] buffer);
        void Write(char ch, int bits);
        void Write(char[] chars);
        void Write(string value);
    }
    
    
}
