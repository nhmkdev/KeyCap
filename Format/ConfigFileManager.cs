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
using System.Windows.Forms;
using Support.UI;

namespace KeyCap.Format
{
    public class ConfigFileManager
    {
        private static readonly int FILE_DATA_PREFIX = (int)0x0E0CA000;
        private static readonly int DATA_FORMAT_VERSION = (int)0x1;

        /// <summary>
        /// Saves the remap entries to a versioned file format
        /// </summary>
        /// <param name="listRemapEntries">The entries to persist</param>
        /// <param name="sFileName">The name of the file to save to</param>
        public void SaveFile(List<RemapEntry> listRemapEntries, string sFileName)
        {
            var zFileStream = new FileStream(sFileName, FileMode.Create, FileAccess.Write, FileShare.None);
            StreamUtil.WriteIntToStream(zFileStream, FILE_DATA_PREFIX);
            StreamUtil.WriteIntToStream(zFileStream, DATA_FORMAT_VERSION);
            listRemapEntries.ForEach(zEntry =>
            {
                var arrayBytes = zEntry.SerializeToBytes();
                zFileStream.Write(arrayBytes, 0, arrayBytes.Length);
            });
            zFileStream.Close();
        }

        /// <summary>
        /// Loads the remap entries from the specified file
        /// </summary>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        public List<RemapEntry> LoadFile(string sFileName)
        {
            FileStream zFileStream = null;
            var listConfigs = new List<RemapEntry>();
            try
            {
                zFileStream = new FileStream(sFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                var nPrefix = StreamUtil.ReadIntFromStream(zFileStream);
                if (nPrefix != FILE_DATA_PREFIX)
                {
                    throw new Exception("{} does not have the correct data prefix. This is likely an unsupported format.".FormatString(sFileName));
                }

                var nDataFormatVersion = StreamUtil.ReadIntFromStream(zFileStream);
                if (nDataFormatVersion != DATA_FORMAT_VERSION)
                {
                    throw new Exception("{} indicates an unsupported data format.".FormatString(sFileName));
                }

                while (zFileStream.Position < zFileStream.Length)
                {
                    listConfigs.Add(new RemapEntry(zFileStream));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                zFileStream?.Close();
            }

            return listConfigs;
        }
    }
}
