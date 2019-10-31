using ExBitStream;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitTest_ExBitStream
{
    public class BitStream_ClassFixture : IDisposable
    {
        private BitStream _stream;
        public BitStream stream
        {
            get
            {
                if (_stream == null)
                {
                    _stream = new BitStream();
                }
                return _stream;
            }
        }

        public int index = 0;

        public void Dispose()
        {
            
        }
    }
}
