////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2016 Tim Stair
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
using System.Text;

namespace KeyCap.Format
{
    /// <summary>
    /// Definition of a pair of inputs, the captured input and actual input to pass to the os
    /// </summary>
    public class IOPairDefinition
    {
        /// <summary>
        /// Format is a byte[] [flags][value][count of outputs]([flags][value])([flags][value])([flags][value])...
        /// </summary>
        private readonly MemoryStream m_zStream = new MemoryStream();

        private readonly int m_nHash;

        /// <summary>
        /// The byte indicies for the stream representation of the class
        /// </summary>
        public enum KeyDefinitionIndices
        {
            Flags,
            Value,
            Count // not applicable to the output definitions
        }

        /// <summary>
        /// Constructor based on input/output definitions
        /// </summary>
        /// <param name="zInputDef">the input definition</param>
        /// <param name="zOutputDef">the output definition</param>
        public IOPairDefinition(IODefinition zInputDef, IODefinition zOutputDef)
        {
            m_nHash = (int)(zInputDef.Flags & 0xFF) + (int)((zInputDef.Value & 0xFF) << 8);
            m_zStream.WriteByte(zInputDef.Flags);
            m_zStream.WriteByte(zInputDef.Value);
            m_zStream.WriteByte(0x01);
            m_zStream.WriteByte(zOutputDef.Flags);
            m_zStream.WriteByte(zOutputDef.Value);
        }

        /// <summary>
        /// Constructor based on an input stream
        /// </summary>
        /// <param name="zFileStream">The file stream to read the definition from</param>
        public IOPairDefinition(FileStream zFileStream)
        {
            var byFlags = ReadByteFromFileStream(zFileStream);
            var byValue = ReadByteFromFileStream(zFileStream);
            m_nHash = (int)(byFlags & 0xFF) + (int)((byValue & 0xFF) << 8);

            m_zStream.WriteByte(byFlags);
            m_zStream.WriteByte(byValue);

            var nOutputDefinitions = zFileStream.ReadByte();
            if (0 >= nOutputDefinitions)
            {
                throw new Exception("Output definition length must be > 0. Invalid File!");
            }

            m_zStream.WriteByte((byte)nOutputDefinitions);

            for (var nIdx = 0; nIdx < nOutputDefinitions; nIdx++)
            {
                byFlags = ReadByteFromFileStream(zFileStream);
                byValue = ReadByteFromFileStream(zFileStream);
                m_zStream.WriteByte(byFlags);
                m_zStream.WriteByte(byValue);
            }

        }

        /// <summary>
        /// Appends an output definition
        /// </summary>
        /// <param name="zOutputDef"></param>
        /// <returns></returns>
        public bool AddOutputDefinition(IODefinition zOutputDef)
        {
            m_zStream.Seek((int)KeyDefinitionIndices.Count, SeekOrigin.Begin);
            byte byCount = (byte)m_zStream.ReadByte();
            // don't allow more than 255 outputs
            if (byCount == 0xFF)
            {
                return false;
            }
            m_zStream.Seek((int)KeyDefinitionIndices.Count, SeekOrigin.Begin);
            m_zStream.WriteByte(++byCount);
            m_zStream.Seek(0, SeekOrigin.End);
            m_zStream.WriteByte(zOutputDef.Flags);
            m_zStream.WriteByte(zOutputDef.Value);
            return true;
        }

        /// <summary>
        /// Returns the byte[] representation
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return m_zStream.ToArray();
        }

        /// <summary>
        /// Returns the input string representation
        /// </summary>
        /// <returns>string representation</returns>
        public string GetInputString()
        {
            var arrayDescription = ToArray();
            return IODefinition.GetDescription(arrayDescription[(int)KeyDefinitionIndices.Flags],
                arrayDescription[(int)KeyDefinitionIndices.Value]);
        }

        /// <summary>
        /// Returns the output string representation
        /// </summary>
        /// <returns>string representation</returns>
        public string GetOutputString()
        {
            var arrayDescription = ToArray();
            var zBuilder = new StringBuilder();
            var nIdx = (int)KeyDefinitionIndices.Count + 1; // start with the first output key definition
            while (nIdx < arrayDescription.Length)
            {
                zBuilder.Append(IODefinition.GetDescription(arrayDescription[(int)KeyDefinitionIndices.Flags + nIdx],
                    arrayDescription[(int)KeyDefinitionIndices.Value + nIdx]));
                if ((nIdx + (int)KeyDefinitionIndices.Count) < arrayDescription.Length)
                {
                    zBuilder.Append("+");
                }
                nIdx += (int)KeyDefinitionIndices.Count;
            }
            return zBuilder.ToString();
        }

        public override int GetHashCode()
        {
            return m_nHash;
        }

        /// <summary>
        /// Reads a single byte from the file stream
        /// </summary>
        /// <param name="zFileStream">The file stream to read from</param>
        /// <returns>The byte value read (throws exception otherwise)</returns>
        private static byte ReadByteFromFileStream(FileStream zFileStream)
        {
            var nReadByte = zFileStream.ReadByte();
            if (-1 == nReadByte)
            {
                throw new Exception("Hit the end of the stream unexpectedly. Invalid File!");
            }
            return (byte)nReadByte;
        }
    }
}
