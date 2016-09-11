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

using System.Windows.Forms;

namespace KeyCap.Format
{
    /// <summary>
    /// Object for storing the definition of an input action
    /// </summary>
    public class IODefinition
    {
        /// <summary>
        /// Flags applied to a given input/output key
        /// </summary>
        public enum IOFlags
        {
            Shift = 1 << 0,
            Control = 1 << 1,
            Alt = 1 << 2,
            // <---- flags below only apply as input to send to windows ---->
            DoNothing = 1 << 3,
            MouseOut = 1 << 4,
            Delay = 1 << 5,
            KeyDown = 1 << 6,
            KeyUp = 1 << 7
            //MAX         = 1 << 7, // it's a byte! (aka if you need 8 you'll need more space)
        }

        /// <summary>
        /// Mapping to windows necessary for firing mouse input
        /// </summary>
        public enum MouseEventId
        {
            // from winuser.h
            MouseLeftDown = 0x02, /* left button down */
            MouseLeftUp = 0x04, /* left button up */
            MouseRightDown = 0x08, /* right button down */
            MouseRightUp = 0x10, /* right button up */
            MouseMiddleDown = 0x20, /* middle button down */
            MouseMiddleUp = 0x40, /* middle button up */
        }

        public byte Flags { get; set; }

        public byte Value { get; }

        /// <summary>
        /// Constructor for an IODefinition
        /// </summary>
        /// <param name="byFlags">The flags defining the input</param>
        /// <param name="byValue">The value of the input</param>
        /// <param name="eKeyArgs">The input key arguments from user input</param>
        public IODefinition(byte byFlags, byte byValue, KeyEventArgs eKeyArgs)
        {
            Flags = byFlags;
            Value = byValue;

            if (null != eKeyArgs)
            {
                Flags |= (byte)(
                    (eKeyArgs.Shift ? (byte)IOFlags.Shift : (byte)0) |
                    (eKeyArgs.Alt ? (byte)IOFlags.Alt : (byte)0) |
                    (eKeyArgs.Control ? (byte)IOFlags.Control : (byte)0));
            }
        }

        /// <summary>
        /// Constructor for an IODefinition
        /// </summary>
        /// <param name="byFlags">The flags defining the input</param>
        /// <param name="byValue">The value of the input</param>
        public IODefinition(byte byFlags, byte byValue):this(byFlags, byValue, null) { }

        /// <summary>
        /// Gets the string description for this IODefinition
        /// </summary>
        /// <returns>String representation of this definition</returns>
        public string GetDescription()
        {
            return GetDescription(Flags, Value);
        }

        /// <summary>
        /// Determines if the input type is flagged (as the there are multiple flags)
        /// </summary>
        /// <param name="eFlag">Flag to check for</param>
        /// <returns>true if the flag bit is 1, false otherwise</returns>
        public bool IsFlaggedAs(IOFlags eFlag)
        {
            return IsFlaggedAs(eFlag, Flags);
        }

        /// <summary>
        /// Constructs a string representation based on the input
        /// </summary>
        /// <param name="byFlags">Flags value</param>
        /// <param name="byValue">Value definition</param>
        /// <returns>String representation</returns>
        public static string GetDescription(byte byFlags, byte byValue)
        {
            // mouse (every other flag ignored)
            if (IsFlaggedAs(IOFlags.MouseOut, byFlags))
            {
                return "[" + ((MouseEventId)byValue).ToString() + "]";
            }
            // delay (every other flag ignored)
            if (IsFlaggedAs(IOFlags.Delay, byFlags))
            {
                return "[Delay: " + (int)byValue + "]";
            }
            // keyboard 
            return "[" +
                ((Keys)byValue).ToString() +
                (((byte)IOFlags.Shift == (byFlags & (byte)IOFlags.Shift)) ? "+Shift" : string.Empty) +
                (((byte)IOFlags.Alt == (byFlags & (byte)IOFlags.Alt)) ? "+Alt" : string.Empty) +
                (((byte)IOFlags.Control == (byFlags & (byte)IOFlags.Control)) ? "+Control" : string.Empty)+
                (((byte)IOFlags.KeyDown == (byFlags & (byte)IOFlags.KeyDown)) ? "+Down" : string.Empty) +
                (((byte)IOFlags.KeyUp == (byFlags & (byte)IOFlags.KeyUp)) ? "+Up" : string.Empty) +
                    "]";
        }

        /// <summary>
        /// Determines if the input type is flagged (as the there are multiple flags)
        /// </summary>
        /// <param name="eFlag">Flag to check for</param>
        /// <param name="byFlags">byte to check for the flag within</param>
        /// <returns>true if the flag bit is 1, false otherwise</returns>
        private static bool IsFlaggedAs(IOFlags eFlag, byte byFlags)
        {
            return (byte)eFlag == (byFlags & (byte)eFlag);
        }
    }
}
