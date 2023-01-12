////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2023 Tim Stair
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
using System.Windows.Forms;
using Support.UI;

namespace KeyCap.Format
{
    /// <summary>
    /// Object for storing the definition of an input action
    /// </summary>
    public class OutputConfig : BaseIOConfig
    {
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="config"></param>
        public OutputConfig(OutputConfig config) : base(config) { }

        /// <summary>
        /// Flags applied to a given input/output key
        /// </summary>
        public enum OutputFlag
        {
            Shift = 1 << 0,
            Control = 1 << 1,
            Alt = 1 << 2,
            DoNothing = 1 << 3, // no action whatsoever
            MouseOut = 1 << 4, // mouse output -- see MouseButton
            Delay = 1 << 5,
            Toggle = 1 << 6,
            Down = 1 << 7,
            Up = 1 << 8,
            Repeat = 1 << 9,
            CancelActiveOutputs = 1 << 10,
            // supports up to 32 entries
        }

        public enum MouseButton
        {
            MouseNone = 0x00,
            MouseLeft = 0x01,
            MouseRight = 0x02,
            MouseMiddle = 0x03
        }

        /// <summary>
        /// Constructor for an InputConfig
        /// </summary>
        /// <param name="nFlags">The flags defining the input</param>
        /// <param name="byVirtualKey">The value of the input</param>
        /// <param name="eKeyArgs">The input key arguments from user input</param>
        public OutputConfig(int nFlags, byte byVirtualKey, int nParameter, KeyEventArgs eKeyArgs)
        {
            Flags = nFlags;
            VirtualKey = byVirtualKey;
            Parameter = nParameter;

            if (null != eKeyArgs)
            {
                Flags |= 
                    (eKeyArgs.Shift ? (int)OutputFlag.Shift : 0) 
                    | (eKeyArgs.Alt ? (int)OutputFlag.Alt : 0) 
                    | (eKeyArgs.Control ? (int)OutputFlag.Control : 0);
            }
        }

        /// <summary>
        /// Constructor for an InputConfig
        /// </summary>
        /// <param name="nFlags">The flags defining the input</param>
        /// <param name="byVirtualKey">The value of the input</param>
        /// <param name="nParameter"></param>
        public OutputConfig(int nFlags, byte byVirtualKey, int nParameter) : this(nFlags, byVirtualKey, nParameter, null) { }

        /// <summary>
        /// Constructor for an InputConfig
        /// </summary>
        /// <param name="nFlags">The flags defining the input</param>
        /// <param name="byVirtualKey">The value of the input</param>
        public OutputConfig(int nFlags, byte byVirtualKey):this(nFlags, byVirtualKey, 0, null) { }

        public OutputConfig(Stream zStream) : base(zStream) { }

        /// <summary>
        /// Gets the string description for this InputConfig
        /// </summary>
        /// <returns>String representation of this definition</returns>
        public override string GetDescription()
        {
            // delay (every other flag ignored)
            if (IsFlaggedAs(OutputFlag.Delay))
            {
                return "[Delay({0}ms)]".FormatString(Parameter);
            }

            if (IsFlaggedAs(OutputFlag.DoNothing))
            {
                return "[Do Nothing]".FormatString(Parameter);
            }

            if (IsFlaggedAs(OutputFlag.CancelActiveOutputs))
            {
                return "[Cancel Active Outputs]".FormatString(Parameter);
            }

            if (IsFlaggedAs(OutputFlag.MouseOut))
            {
                return GetOutputDescriptionText((MouseButton)VirtualKey, "Mouse");
            }

            // keyboard 
            return GetOutputDescriptionText((Keys)VirtualKey, "Key");
        }

        /// <summary>
        /// Indicates if the config is assigned a valid action
        /// </summary>
        /// <returns></returns>
        public bool IsAssignedAction()
        {
            return IsFlaggedAs(OutputFlag.Down)
                   || IsFlaggedAs(OutputFlag.Up)
                   || IsFlaggedAs(OutputFlag.MouseOut)
                   || IsFlaggedAs(OutputFlag.Delay)
                    || IsFlaggedAs(OutputFlag.DoNothing);
        }

        /// <summary>
        /// Gets the output description for the up/down/press based on the state of the flags
        /// </summary>
        /// <param name="sActionPrefix">The prefix indicating the type of input</param>
        /// <returns></returns>
        private string GetOutputDescriptionText(Enum eInputId, string sActionPrefix)
        {
            return "[" +
                   eInputId +
                   (IsFlaggedAs(OutputFlag.Down) && IsFlaggedAs(OutputFlag.Up)
                       ? ":Press"
                       : ((IsFlaggedAs(OutputFlag.Down) ? ":{0}Down".FormatString(sActionPrefix) : string.Empty) +
                          (IsFlaggedAs(OutputFlag.Up) ? ":{0}Up".FormatString(sActionPrefix) : string.Empty))
                   ) +
                   (IsFlaggedAs(OutputFlag.Shift) ? "+Shift" : string.Empty) +
                   (IsFlaggedAs(OutputFlag.Alt) ? "+Alt" : string.Empty) +
                   (IsFlaggedAs(OutputFlag.Control) ? "+Control" : string.Empty) +
                   (IsFlaggedAs(OutputFlag.Toggle) ? "+Toggle" : string.Empty) +
                   (IsFlaggedAs(OutputFlag.Repeat) ? "+Repeat"
                       + (Parameter > 0 ? "({0}ms)".FormatString(Parameter) : "")
                       : string.Empty) +
                   "]";
        }
    }
}
