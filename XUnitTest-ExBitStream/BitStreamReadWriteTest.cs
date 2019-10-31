using ExBitStream;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTest_ExBitStream
{
    public class BitStreamReadWriteTest : IClassFixture<BitStream_ClassFixture>
    {
        private ITestOutputHelper _helper;
        private BitStream_ClassFixture _fixture;
        private BitStream stream => _fixture.stream; 

        public BitStreamReadWriteTest(ITestOutputHelper helper, BitStream_ClassFixture fixture)
        {
            _helper = helper;
            _fixture = fixture;
        }

        [Fact]
        public void Test_ReadBoolean()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(true);
            bool result = reader.ReadBoolean(_fixture.index);
            _fixture.index++;


            Assert.True(result);
        }

        [Fact]
        public void Test_ReadByte()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(125, 7);
            byte result = reader.ReadByte(_fixture.index, 7);
            _fixture.index += 7;

            Assert.Equal(125, result);
        }

        [Fact]
        public void Test_ReadInt16()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(5, 3);
            short result = reader.ReadInt16(_fixture.index, 3);
            _fixture.index += 3;

            Assert.Equal(5, result);
        }

        [Fact]
        public void Test_ReadInt32()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(36, 6);
            int result = reader.ReadInt32(_fixture.index, 6);
            _fixture.index += 6;

            Assert.Equal(36, result);
        }

        [Fact]
        public void Test_ReadInt64()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(872, 10);
            long result = reader.ReadInt64(_fixture.index, 10);
            _fixture.index += 10;

            Assert.Equal(872, result);
        }

        [Fact]
        public void Test_ReadUInt16()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(5, 3);
            ushort result = reader.ReadUInt16(_fixture.index, 3);
            _fixture.index += 3;

            Assert.Equal((ushort)5, result);
        }

        [Fact]
        public void Test_ReadUInt32()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(36, 6);
            uint result = reader.ReadUInt32(_fixture.index, 6);
            _fixture.index += 6;

            Assert.Equal((uint)36, result);
        }

        [Fact]
        public void Test_ReadUInt64()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write(872, 10);
            ulong result = reader.ReadUInt64(_fixture.index, 10);
            _fixture.index += 10;

            Assert.Equal((ulong)872, result);
        }

        [Fact]
        public void Test_ReadChar()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            writer.Write('x');
            char result = reader.ReadChar(_fixture.index);
            _fixture.index += 8;

            Assert.Equal('x', result);
        }

        [Fact]
        public void Test_ReadString()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            string str = "Hello World!";
            writer.Write(str);
            string result = reader.ReadString(_fixture.index, str.Length);
            _fixture.index += str.Length * 8;

            Assert.Equal(str, result);
        }

        [Fact]
        public void Test_ByteArray()
        {
            BitStreamWriter writer = new BitStreamWriter(stream);
            BitStreamReader reader = new BitStreamReader(stream);
            byte[] array = new byte[] { 255, 254, 251, 250 };
            writer.Write(array);
            byte[] result = reader.ReadBytes(_fixture.index, array.Length);
            _fixture.index += array.Length * 8;

            Assert.Equal(array, result);
        }
    }
}
