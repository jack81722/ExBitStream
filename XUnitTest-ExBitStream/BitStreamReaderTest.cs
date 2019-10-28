using ExBitStream;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitTest_ExBitStream
{
    public class BitStreamReaderTest
    {
        private BitStream _stream;
        public BitStream stream
        {
            get
            {
                if(_stream == null)
                {
                    _stream = new BitStream();
                }
                return _stream;
            }
        }
        private BitStreamWriter _writer;
        public BitStreamWriter writer
        {
            get
            {
                if(_writer == null)
                {
                    _writer = new BitStreamWriter(stream);
                }
                return _writer;
            }
        }
        private int index = 0;

        [Fact]
        public void Test_ReadBoolean()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            bool result = reader.ReadBoolean(0);
            index++;

            Assert.True(result);
        }

        [Fact]
        public void Test_ReadByte()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            byte result = reader.ReadByte(index, 7);
            index += 7;

            Assert.Equal(125, result);
        }

        [Fact]
        public void Test_ReadInt16()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            index += 7;
            writer.Write(5, 3);
            short result = reader.ReadInt16(index, 3);
            index += 3;

            Assert.Equal(5, result);
        }

        [Fact]
        public void Test_ReadInt32()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            index += 7;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            int result = reader.ReadInt32(index, 6);
            index += 6;

            Assert.Equal(36, result);
        }

        [Fact]
        public void Test_ReadInt64()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            index += 7;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            long result = reader.ReadInt64(index, 10);
            index += 10;

            Assert.Equal(872, result);
        }

        [Fact]
        public void Test_ReadUInt16()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            index += 7;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            index += 10;
            writer.Write(5, 3);
            ushort result = reader.ReadUInt16(index, 3);
            index += 3;

            Assert.Equal((ushort)5, result);
        }

        [Fact]
        public void Test_ReadUInt32()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            index += 7;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            index += 10;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            uint result = reader.ReadUInt32(index, 6);
            index += 6;

            Assert.Equal((uint)36, result);
        }

        [Fact]
        public void Test_ReadUInt64()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            index += 7;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            index += 10;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            ulong result = reader.ReadUInt64(index, 10);
            index += 10;

            Assert.Equal((ulong)872, result);
        }

        [Fact]
        public void Test_ReadChar()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            index += 7;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            index += 10;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            index += 10;
            writer.Write('x');
            char result = reader.ReadChar(index);
            index += 8;

            Assert.Equal('x', result);
        }

        [Fact]
        public void Test_ReadString()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            index += 7;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            index += 10;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            index += 10;
            writer.Write('x');
            index += 8;
            string str = "Hello World!";
            writer.Write(str);
            string result = reader.ReadString(index, str.Length);
            index += str.Length * 8;

            Assert.Equal(str, result);
        }

        [Fact]
        public void Test_ByteArray()
        {
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            index++;
            writer.Write(125, 7);
            index += 7;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            index += 10;
            writer.Write(5, 3);
            index += 3;
            writer.Write(36, 6);
            index += 6;
            writer.Write(872, 10);
            index += 10;
            writer.Write('x');
            index += 8;
            string str = "Hello World!";
            writer.Write(str);
            index += str.Length * 8;
            byte[] array = new byte[] { 255, 254, 251, 250 };
            writer.Write(array);
            byte[] result = reader.ReadBytes(index, array.Length);
            index += array.Length * 8;

            Assert.Equal(array, result);
        }
    }
}
