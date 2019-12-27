////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2019 Tim Stair
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

using System.IO;
using KeyCap.Util;

namespace KeyCap.Format
{
    public abstract class BaseIOConfig
    {
        public int Flags { get; set; }
        // https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        public byte VirtualKey { get; set; }
        public int Parameter { get; set; }

        public BaseIOConfig() { }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="config"></param>
        public BaseIOConfig(BaseIOConfig config)
        {
            Flags = config.Flags;
            VirtualKey = config.VirtualKey;
            Parameter = config.Parameter;
        }

        /// <summary>
        /// Constructor for an InputConfig
        /// </summary>
        /// <param name="byFlags">The flags defining the input</param>
        /// <param name="byVirtualKey">The value of the input</param>
        /// <param name="eKeyArgs">The input key arguments from user input</param>
        public BaseIOConfig(int nFlags, byte byVirtualKey, int nParameter)
        {
            Flags = nFlags;
            VirtualKey = byVirtualKey;
            Parameter = nParameter;
        }

        public BaseIOConfig(Stream zStream)
        {
            Flags = StreamUtil.ReadIntFromStream(zStream);
            VirtualKey = StreamUtil.ReadByteFromStream(zStream);
            StreamUtil.ReadByteFromStream(zStream);
            StreamUtil.ReadByteFromStream(zStream);
            StreamUtil.ReadByteFromStream(zStream);
            Parameter = StreamUtil.ReadIntFromStream(zStream);
        }

        /// <summary>
        /// Determines if the input type is flagged (as the there are multiple flags)
        /// </summary>
        /// <param name="eFlag">Flag to check for</param>
        /// <returns>true if the flag bit is 1, false otherwise</returns>
        public bool IsFlaggedAs(System.Enum eFlag)
        {
            return BitUtil.IsFlagged(Flags, eFlag);
        }

        public abstract string GetDescription();

        public void SerializeToStream(Stream zStream)
        {
            StreamUtil.WriteIntToStream(zStream, Flags);
            zStream.WriteByte(VirtualKey);
            zStream.WriteByte(0);
            zStream.WriteByte(0);
            zStream.WriteByte(0);
            StreamUtil.WriteIntToStream(zStream, Parameter);
        }
    }
}
