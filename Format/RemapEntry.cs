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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KeyCap.Format
{
    /// <summary>
    /// Definition of a pair of inputs, the captured input and actual input to pass to the os
    /// </summary>
    public class RemapEntry
    {
#warning remove the memory stream usage... just confusing
        /// <summary>
        /// Format is a byte[] [flags][value][count of outputs]([flags][value])([flags][value])([flags][value])...
        /// </summary>
//        private readonly MemoryStream m_zStream = new MemoryStream();

        private InputConfig InputConfig { get; set; }
        private List<OutputConfig> OutputConfigs { get; set; }

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
        /// <param name="zInputConfig">the input definition</param>
        /// <param name="zOutputConfig">the output definition</param>
        public RemapEntry(InputConfig zInputConfig, OutputConfig zOutputConfig)
        {
            InputConfig = zInputConfig;
            OutputConfigs = new List<OutputConfig>(new [] {zOutputConfig});
#warning move hash calc to method
            m_nHash = (int)(zInputConfig.Flags & 0xFF) + (int)((zInputConfig.VirtualKey & 0xFF) << 8);
#if false
            zInputConfig.SerializeToStream(m_zStream);
            m_zStream.WriteByte(1);
            zOutputConfig.SerializeToStream(m_zStream);
#endif
        }

        /// <summary>
        /// Constructor based on an input stream
        /// </summary>
        /// <param name="zFileStream">The file stream to read the config from</param>
        public RemapEntry(FileStream zFileStream)
        {
            InputConfig = new InputConfig(zFileStream);
            m_nHash = (int)(InputConfig.Flags & 0xFF) + (int)((InputConfig.VirtualKey & 0xFF) << 8);

            var nOutputConfigs = zFileStream.ReadByte();
            if (0 >= nOutputConfigs)
            {
                throw new Exception("Output definition length must be > 0. Invalid File!");
            }

            zFileStream.ReadByte();
            zFileStream.ReadByte();
            zFileStream.ReadByte();

            try
            {
                OutputConfigs = new List<OutputConfig>(nOutputConfigs);
                for (int nIdx = 0; nIdx < nOutputConfigs; nIdx++)
                {
                    OutputConfigs.Add(new OutputConfig(zFileStream));
                }
            }
            catch (Exception e)
            {
                throw new Exception("Invalid File! The number of outputs does not match the data in the file.", e);
            }
        }

        /// <summary>
        /// Appends an output definition
        /// </summary>
        /// <param name="zOutputConfig"></param>
        /// <returns></returns>
        public bool AppendOutputConfig(OutputConfig zOutputConfig)
        {
            if (OutputConfigs.Count == 0xff)
            {
                return false;
            }
            OutputConfigs.Add(zOutputConfig);
            return true;
        }

        public byte[] SerializeToBytes()
        {
            var zStream = new MemoryStream();
            InputConfig.SerializeToStream(zStream);
            zStream.WriteByte((byte)OutputConfigs.Count);
            zStream.WriteByte(0);
            zStream.WriteByte(0);
            zStream.WriteByte(0);
            OutputConfigs.ForEach(oc => oc.SerializeToStream(zStream));
            return zStream.ToArray();
        }

        /// <summary>
        /// Returns the input string representation
        /// </summary>
        /// <returns>string representation</returns>
        public string GetInputString()
        {
            return InputConfig.GetDescription();
        }

        /// <summary>
        /// Returns the output string representation
        /// </summary>
        /// <returns>string representation</returns>
        public string GetOutputString()
        {
            var zBuilder = new StringBuilder();
            for (var nIdx = 0; nIdx < OutputConfigs.Count - 1; nIdx++)
            {
                zBuilder.Append(OutputConfigs[nIdx].GetDescription());
                zBuilder.Append("+");
            }

            zBuilder.Append(OutputConfigs[OutputConfigs.Count - 1].GetDescription());
            return zBuilder.ToString();
        }

#warning why is this necessary? (double definition detection it appears... so the same input isn't reused)
        public override int GetHashCode()
        {
            return m_nHash;
        }
    }
}
