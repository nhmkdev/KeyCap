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

using System.IO;
using System.Text;

namespace KeyCap.Format
{
    /// <summary>
    /// Definition of a pair of inputs, the captured input and actual input to pass to the os
    /// </summary>
    public class IOPairDefinition
    {
        private readonly MemoryStream m_zStream = new MemoryStream();

        public enum KeyDefinitionIndices // not inclusive out the output
        {
            Flags,
            Value,
            Count
        }

        public IOPairDefinition(IODefinition zInputDef, IODefinition zOutputDef)
        {
            m_zStream.WriteByte(zInputDef.Flags);
            m_zStream.WriteByte(zInputDef.Value);
            m_zStream.WriteByte(0x01);
            m_zStream.WriteByte(zOutputDef.Flags);
            m_zStream.WriteByte(zOutputDef.Value);
        }

        public IOPairDefinition(byte[] arrayKeyDefinition)
        {
            m_zStream.Write(arrayKeyDefinition, 0, arrayKeyDefinition.Length);
        }

        public bool AddOutputDefinition(IODefinition zOutputDef)
        {
            m_zStream.Seek((int)KeyDefinitionIndices.Count, SeekOrigin.Begin);
            byte byCount = (byte)m_zStream.ReadByte();
            if (byCount == 0xFF) // 255 output max!
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

        public byte[] ToArray()
        {
            return m_zStream.ToArray();
        }

        public string GetInputString()
        {
            byte[] arrayDescription = ToArray();
            return IODefinition.GetDescription(arrayDescription[(int)KeyDefinitionIndices.Flags],
                arrayDescription[(int)KeyDefinitionIndices.Value]);
        }

        public string GetOutputString()
        {
            byte[] arrayDescription = ToArray();
            StringBuilder zBuilder = new StringBuilder();
            int nIdx = (int)KeyDefinitionIndices.Count + 1; // start with the first output key definition
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

        public bool InputMatches(IOPairDefinition zKeyDef)
        {
            byte[] arrayInputThis = ToArray();
            byte[] arrayInputThat = zKeyDef.ToArray();
            return (arrayInputThis[(int)KeyDefinitionIndices.Flags] == arrayInputThat[(int)KeyDefinitionIndices.Flags]) &&
                (arrayInputThis[(int)KeyDefinitionIndices.Value] == arrayInputThat[(int)KeyDefinitionIndices.Value]);
        }
    }
}
