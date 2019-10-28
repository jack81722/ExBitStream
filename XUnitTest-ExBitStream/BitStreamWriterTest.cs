using ExBitStream;
using System;
using Xunit;

namespace XUnitTest_ExBitStream
{
    public class BitStreamWriterTest
    {
        [Fact]
        public void Test1()
        {
            BitStream stream = new BitStream();
            BitStreamWriter writer = new BitStreamWriter(stream);
            byte sum = 0;
            for(int i = 0; i < 8; i++)
            {
                writer.Write(true);
                sum |= (byte)(1 << (7 - i));
            }

            var bytes = stream.ToBytes();
            Assert.True(sum == bytes[0], $"Sum:{sum} != Byte:{bytes[0].ToString()}");
        }
    }
}
