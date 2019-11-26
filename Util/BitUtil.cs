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

namespace KeyCap.Util
{
    public static class BitUtil
    {
        /// <summary>
        /// Determines if the input type is flagged (as the there are multiple flags)
        /// </summary>
        /// <param name="eFlagBit">Flag to check for (assumes this is a bit index)</param>
        /// <param name="byFlags">byte to check for the flag within</param>
        /// <returns>true if the flag bit is 1, false otherwise</returns>
        public static bool IsFlagged(int nFlags, Enum eFlagBit)
        {
            var nEnumFlag = Convert.ToInt32(eFlagBit);
            return nEnumFlag == (nFlags & nEnumFlag);
        }

        public static int UpdateFlag(int nFlag, bool bFlagSetting, Enum eFlagBit)
        {
            return nFlag | (bFlagSetting ? Convert.ToInt32(eFlagBit) : 0);
        }
    }
}
