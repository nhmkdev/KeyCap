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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KeyCap.Format
{
    /// <summary>
    /// Definition of a pair of inputs, the captured input and actual input (OutputConfigs) to pass to the os
    /// </summary>
    public class RemapEntry
    {
        private InputConfig InputConfig { get; set; }
        private List<OutputConfig> OutputConfigs { get; set; }

        public int OutputConfigCount
        {
            get { return OutputConfigs == null ? 0 : OutputConfigs.Count; }
        }

        private readonly int m_nHash;

        /// <summary>
        /// Constructor based on input/output definitions
        /// </summary>
        /// <param name="zInputConfig">the input definition</param>
        /// <param name="zOutputConfig">the output definition</param>
        public RemapEntry(InputConfig zInputConfig, OutputConfig zOutputConfig)
        {
            InputConfig = zInputConfig;
            OutputConfigs = new List<OutputConfig>(new [] {zOutputConfig});
            m_nHash = CalculateHashCode(InputConfig);
        }

        /// <summary>
        /// Constructor based on an input stream
        /// </summary>
        /// <param name="zFileStream">The file stream to read the config from</param>
        public RemapEntry(FileStream zFileStream)
        {
            InputConfig = new InputConfig(zFileStream);
            m_nHash = CalculateHashCode(InputConfig);

            var nOutputConfigs = zFileStream.ReadByte();
            if (0 >= nOutputConfigs)
            {
                throw new Exception("Output definition length must be > 0. Invalid File!");
            }

            // read the 3 additional bytes representing the 32bit int containing the nOutputConfigs above (limited to 255)
            // See RemapEntry struct inkeycapturestructs.h for the padding bytes
            zFileStream.Read(new byte[3], 0, 3);

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
            OutputConfigs.Add(zOutputConfig);
            return true;
        }

        public byte[] SerializeToBytes()
        {
            var zStream = new MemoryStream();
            InputConfig.SerializeToStream(zStream);
            zStream.WriteByte((byte)OutputConfigs.Count);
            // output count is only 1 byte, pad out to a 32bit int
            zStream.Write(new byte[3], 0, 3);
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

        /// <summary>
        /// Returns the hash code of this remap entry
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return m_nHash;
        }

        protected static int CalculateHashCode(InputConfig zInputConfig)
        {
            return (int)(zInputConfig.Flags & 0xFF) + (int)((zInputConfig.VirtualKey & 0xFF) << 8);
        }
    }
}
