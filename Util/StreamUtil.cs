////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2021 Tim Stair
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using Support.UI;

namespace KeyCap.Format
{
    public static class StreamUtil
    {
        /// <summary>
        /// Reads a single byte from the file stream
        /// </summary>
        /// <param name="zStream">The file stream to read from</param>
        /// <returns>The byte value read (throws exception otherwise)</returns>
        public static byte ReadByteFromStream(Stream zStream)
        {
            var nReadByte = zStream.ReadByte();
            if (-1 == nReadByte)
            {
                throw new Exception("Hit the end of the stream unexpectedly. Invalid File!");
            }
            return (byte)nReadByte;
        }

        /// <summary>
        /// Reads a single byte from the file stream
        /// </summary>
        /// <param name="zStream">The file stream to read from</param>
        /// <param name="nByteReadCount"></param>
        /// <returns>The byte value read (throws exception otherwise)</returns>
        public static byte[] ReadBytesFromStream(Stream zStream, int nByteReadCount)
        {
            var arrayBytes = new byte[nByteReadCount];
            var nReadBytes = zStream.Read(arrayBytes, 0, nByteReadCount);
            if (-1 == nReadBytes)
            {
                throw new Exception("Hit the end of the stream unexpectedly. Invalid File!");
            }
            else if (nByteReadCount != nReadBytes)
            {
                throw new Exception("Unable to read {0} bytes. Read {1} bytes.".FormatString(nByteReadCount, nReadBytes));
            }
            return arrayBytes;
        }

        /// <summary>
        /// Reads a single byte from the file stream
        /// </summary>
        /// <param name="zStream">The file stream to read from</param>
        /// <returns>The byte value read (throws exception otherwise)</returns>
        public static int ReadIntFromStream(Stream zStream)
        {
            var nValue = 0;
            for (var nIdx = 0; nIdx < 4; nIdx++)
            {
                nValue |= ReadByteFromStream(zStream) << (nIdx * 8);
            }
            return nValue;
        }

        /// <summary>
        /// Reads a single byte from the file stream
        /// </summary>
        /// <param name="zStream">The file stream to read from</param>
        /// <returns>The byte value read (throws exception otherwise)</returns>
        public static void WriteIntToStream(Stream zStream, int nValue)
        {
            for (var nIdx = 0; nIdx < 4; nIdx++)
            {
                zStream.WriteByte((byte) (nValue >> (nIdx * 8)));
            }
        }
    }
}
