////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2022 Tim Stair
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
using System.Runtime.InteropServices;
using System.Reflection;
using KeyCap.Settings;

namespace KeyCap.Wrapper
{
    /// <summary>
    /// This is a proxy class that wraps the dll accessed methods providing keyboard hook functionality via win32
	/// </summary>
	public static class KeyCaptureLib
	{
        [DllImport("KeyCapLib.dll", CallingConvention=CallingConvention.Cdecl)]
        private static extern int LoadAndCaptureFromFile(IntPtr nIntstance, string sFile);

        [DllImport("KeyCapLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ShutdownCapture();

        /// <summary>
        /// Loads the specified file and initiates the windows keyboard hook
        /// </summary>
        /// <param name="sFile">The file to load</param>
        /// <returns>Result of the attempt to setup the keyboard hook</returns>
        public static CaptureMessage LoadFileAndCapture(string sFile)
        {
            var arrayModules = Assembly.GetExecutingAssembly().GetModules();
            // Skipping the length check because if there is no loaded module there is no application
            var nHInstance = Marshal.GetHINSTANCE(arrayModules[0]);
            return (CaptureMessage)LoadAndCaptureFromFile(nHInstance, sFile);
        }

        /// <summary>
        /// Disables the keyboard hook
        /// </summary>
        public static void Shutdown()
        {
            ShutdownCapture();
        }
	}
}
