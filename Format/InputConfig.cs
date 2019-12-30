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
using System.Windows.Forms;

namespace KeyCap.Format
{
    /// <summary>
    /// Object for storing the definition of an input action
    /// </summary>
    public class InputConfig : BaseIOConfig
    {
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="config"></param>
        public InputConfig(InputConfig config) : base(config) { }

        /// <summary>
        /// Flags applied to a given input key (this is the same size as the OutputFlag (32bit)
        /// </summary>
        public enum InputFlag
        {
            Shift = 1 << 0,
            Control = 1 << 1,
            Alt = 1 << 2,
            // supports up to 32 entries
        }

        /// <summary>
        /// Constructor for an InputConfig
        /// </summary>
        /// <param name="byFlags">The flags defining the input</param>
        /// <param name="byVirtualKey">The value of the input</param>
        /// <param name="eKeyArgs">The input key arguments from user input</param>
        public InputConfig(byte byVirtualKey, KeyEventArgs eKeyArgs)
        {
            VirtualKey = byVirtualKey;

            if (null != eKeyArgs)
            {
                Flags |= (int)(
                    (eKeyArgs.Shift ? (int)InputFlag.Shift : (byte)0) |
                    (eKeyArgs.Alt ? (int)InputFlag.Alt : (byte)0) |
                    (eKeyArgs.Control ? (int)InputFlag.Control : (byte)0));
            }
        }

        public InputConfig(Stream zStream) : base(zStream) { }

        public override string GetDescription()
        {
            return "[" +
                   (Keys)VirtualKey +
                   (IsFlaggedAs(InputFlag.Shift) ? "+Shift" : string.Empty) +
                   (IsFlaggedAs(InputFlag.Alt) ? "+Alt" : string.Empty) +
                   (IsFlaggedAs(InputFlag.Control) ? "+Control" : string.Empty) +
                   "]";
        }
    }
}
