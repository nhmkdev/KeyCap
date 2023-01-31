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
using System.Windows.Forms;

namespace KeyCap.Util
{
    static class KeyUtil
    {
        public static byte GetKeyByte(char cInput, ref bool bShift)
        {
            bShift = char.IsUpper(cInput);
            try
            {
                return (byte)(Keys)Enum.Parse(typeof(Keys), cInput.ToString(), true);
            }
            catch (Exception)
            {
                // HACK
                // ignore and try the switch
            }
#warning HACK This is super limited to a very specific keyboard
            switch (cInput)
            {
                case ' ':
                    return (byte)Keys.Space;
                case '!':
                    bShift = true;
                    return (byte)Keys.D1;
                case '@':
                    bShift = true;
                    return (byte)Keys.D2;
                case '#':
                    bShift = true;
                    return (byte)Keys.D3;
                case '$':
                    bShift = true;
                    return (byte)Keys.D4;
                case '%':
                    bShift = true;
                    return (byte)Keys.D5;
                case '^':
                    bShift = true;
                    return (byte)Keys.D6;
                case '&':
                    bShift = true;
                    return (byte)Keys.D7;
                case '*':
                    bShift = true;
                    return (byte)Keys.D8;
                case '(':
                    bShift = true;
                    return (byte)Keys.D9;
                case ')':
                    bShift = true;
                    return (byte)Keys.D0;
                case '~':
                    bShift = true;
                    return (byte)Keys.Oemtilde;
                case '{':
                    bShift = true;
                    return (byte)Keys.OemOpenBrackets;
                case '}':
                    bShift = true;
                    return (byte)Keys.Oem6;
                case '|':
                    bShift = true;
                    return (byte)Keys.Oem5;
                case ':':
                    bShift = true;
                    return (byte)Keys.Oem1;
                case '"':
                    bShift = true;
                    return (byte)Keys.Oem7;
                case '<':
                    bShift = true;
                    return (byte)Keys.Oemcomma;
                case '>':
                    bShift = true;
                    return (byte)Keys.OemPeriod;
                case '?':
                    bShift = true;
                    return (byte)Keys.OemQuestion;
            }

            throw new Exception("Unsupported character: " + cInput);
        }

    }
}
