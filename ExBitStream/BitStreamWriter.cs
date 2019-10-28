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
        // the buffer that the bits are written into
        private List<byte> _targetBuffer = null;

        // number of free bits remaining in the last byte added to the target buffer
        private int _remaining = 0;

        public BitStreamWriter(BitStream stream)
        {
            _targetBuffer = stream.Buffer;
        }

        private void _Write<TValue, TShifter>(TValue value, TShifter shifter, int countOfBits) where TShifter : IByteUtil<TValue>
        {
            // calculate the number of full bytes
            //   Example: 10 bits would require 1 full byte
            int fullBytes = countOfBits / Native.BitsPerByte;

            // calculate the number of bits that spill beyond the full byte boundary
            //   Example: 10 buttons would require 2 extra bits (8 fit in a full byte)
            int bitsToWrite = countOfBits % Native.BitsPerByte;

            for (; fullBytes >= 0; fullBytes--)
            {
                byte byteOfData = shifter.ShiftRight(value, fullBytes * Native.BitsPerByte);
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
        }

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
        }

        public void Write(short value, int bits = Native.BitsPerShort)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerShort)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of short.");


            // calculate the number of full bytes
            //   Example: 10 bits would require 1 full byte
            int fullBytes = bits / Native.BitsPerByte;

            // calculate the number of bits that spill beyond the full byte boundary
            //   Example: 10 buttons would require 2 extra bits (8 fit in a full byte)
            int bitsToWrite = bits % Native.BitsPerByte;

            for (; fullBytes >= 0; fullBytes--)
            {
                byte byteOfData = (byte)(value >> (fullBytes * Native.BitsPerByte));
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

        public void Write(ushort value, int bits = Native.BitsPerShort)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerShort)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of ushort.");


            // calculate the number of full bytes
            //   Example: 10 bits would require 1 full byte
            int fullBytes = bits / Native.BitsPerByte;

            // calculate the number of bits that spill beyond the full byte boundary
            //   Example: 10 buttons would require 2 extra bits (8 fit in a full byte)
            int bitsToWrite = bits % Native.BitsPerByte;

            for (; fullBytes >= 0; fullBytes--)
            {
                byte byteOfData = (byte)(value >> (fullBytes * Native.BitsPerByte));
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

        public void Write(int value, int bits = Native.BitsPerInt)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerInt)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of integer.");

            IntUtil shifter = ByteUtilManager.IntUtil;
            _Write(value, shifter, bits);
        }

        /// <summary>
        /// Writes the count of bits from the int to the left packed buffer
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bits"></param>
        public void Write(uint value, int bits = Native.BitsPerInt)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerInt)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of integer.");


            UIntUtil shifter = new UIntUtil();
            _Write(value, shifter, bits);
        }

        public void Write(long value, int bits = Native.BitsPerLong)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerLong)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of integer.");


            LongUtil shifter = new LongUtil();
            _Write(value, shifter, bits);
        }

        public void Write(ulong value, int bits = Native.BitsPerLong)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerLong)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of integer.");


            ULongUtil shifter = new ULongUtil();
            _Write(value, shifter, bits);
        }

        public void Write(float value, int bits)
        {
            var bytes = BitConverter.GetBytes(value);
            Write(bytes);
        }

        public void Write(double value, int bits)
        {
            var bytes = BitConverter.GetBytes(value);
            Write(bytes);
        }
        
        public void Write(byte[] buffer)
        {
            for(int i = 0; i < buffer.Length; i++)
            {
                Write(buffer[i]);
            }
        }

        public void Write(char ch, int bits = Native.BitsPerByte)
        {
            // validate that a subset of the bits in a single byte are being written
            if (bits <= 0 || bits > Native.BitsPerLong)
                throw new ArgumentOutOfRangeException("countOfBits", bits, "Count of bits must be greater than zero and less than size of integer.");


            CharUtil shifter = new CharUtil();
            _Write(ch, shifter, bits);
        }

        public void Write(char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                Write(chars[i]);
            }
        }

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
        void Write(float value, int bits);
        void Write(double value, int bits);
        void Write(byte[] buffer);
        void Write(char ch, int bits);
        void Write(char[] chars);
        void Write(string value);
    }
    
    
}
