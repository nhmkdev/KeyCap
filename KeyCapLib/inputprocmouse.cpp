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

#include "inputproc.h"
#include "keycapstructs.h"
#include "keycaputil.h"

/*
Wrapper for LowLevelInputProc
*/
LRESULT CALLBACK LowLevelMouseProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	DWORD vkCode = 0;

	MSLLHOOKSTRUCT* pMSHook = reinterpret_cast<MSLLHOOKSTRUCT*>(lParam);

	// map the mouse input to the KeyCap mouse representation
	switch (wParam)
	{
	case WM_MBUTTONDOWN:
	case WM_MBUTTONUP:
		{
			// https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-mbuttondown
			// mouse mid
			vkCode = VK_MBUTTON;
			break;
		}
	case WM_XBUTTONDOWN:
	case WM_XBUTTONUP:
		{
			// mouse x
			UINT xBtn = GET_XBUTTON_WPARAM(pMSHook->mouseData);
			vkCode = xBtn == XBUTTON1 ? VK_XBUTTON1 : VK_XBUTTON2;
			break;

		}
	default:
		{
			// some other mouse activity (like move)
			return CallNextHookEx(nullptr, nCode, wParam, lParam);
		}
	}

// probably need to do something about this... detect already pressed?
// There's already the thread per input tracking...
#if false // mouse management apps already are using low-level injection so this would block KeyCap
	if(pMSHook->flags & LLMHF_INJECTED)
	{
		LogDebugMessage("LowLevelMouseProc Complete - SKIP Processing Click [Injected]");
		return CallNextHookEx(nullptr, nCode, wParam, lParam); // invalid or unsupported event
	}
#else
	if (pMSHook->flags & LLMHF_INJECTED)
	{
		LogDebugMessage("LowLevelMouseProc Running - Processing Click [Injected]");
	}
#endif

	if(HC_ACTION == nCode
		&& vkCode)
	{
		LogDebugMessage("LowLevelMouseProc Complete - Processing Click");
		return LowLevelInputProc(nCode, wParam, lParam, vkCode);
	}

	LogDebugMessage("LowLevelMouseProc Complete - SKIP Processing Click");
	return CallNextHookEx(nullptr, nCode, wParam, lParam); // invalid or unsupported event

}