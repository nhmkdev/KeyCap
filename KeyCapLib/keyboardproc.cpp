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

#include "keycapturestructs.h"
#include "keycaptureutil.h"
#include "keyboardproc.h"
#include "sendinputthread.h"

extern RemapEntryListItem* g_KeyTranslationTable[WIN_KEY_COUNT];

/*
Implementation of the win32 LowLevelKeyboardProc (see docs for information)

This is a special implementation to avoid sending the actual keyup/keydown
messages (as those are the keys being captured). A separate thread is
created to send out the key(s) to send to the os.
*/
LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	KBDLLHOOKSTRUCT  *pHook = reinterpret_cast<KBDLLHOOKSTRUCT*>(lParam);
	bool bSentInput = false;

	bool bAlt = 0 != (GetAsyncKeyState(VK_MENU) & 0x8000);
	bool bControl = 0 != (GetAsyncKeyState(VK_CONTROL) & 0x8000);
	bool bShift = 0 != (GetAsyncKeyState(VK_SHIFT) & 0x8000);

	// don't catch injected keys
	if (!(pHook->flags & LLKHF_INJECTED) 
		&& HC_ACTION == nCode)
	{
		// NOTE: these are in while loops because a key may have multiple mappings due to the flag keys (shift/alt/ctrl)
		switch (wParam)
		{
		case WM_KEYDOWN:
		case WM_SYSKEYDOWN:
			// detect a keydown that matches a remaped key (and start the input thread to respond accordingly then indicate to the OS that the key has been handled)
			{
				RemapEntryListItem* pKeyListItem = g_KeyTranslationTable[pHook->vkCode];
				while (NULL != pKeyListItem)
				{
					InputConfig* pKeyDef = &pKeyListItem->pEntry->inputConfig;
					if ((bAlt == pKeyDef->inputFlag.bAlt) &&
						(bControl == pKeyDef->inputFlag.bControl) &&
						(bShift == pKeyDef->inputFlag.bShift))
					{
						char* pInputConfigDescription = GetInputConfigDescription(*pKeyDef);
						LogDebugMessage("Detected Key Press: %s Outputs: %d", pInputConfigDescription, pKeyListItem->pEntry->outputCount);
						free(pInputConfigDescription);
						// TODO: get thread handle, also check for null
						CreateThread(NULL, 0, SendInputThread, pKeyListItem->pEntry, 0, NULL);
						bSentInput = true;
						break;
					}
					pKeyListItem = pKeyListItem->pNext;
				}
			}
		break;
		case WM_KEYUP:
		case WM_SYSKEYUP:
			// detect a keyup that matches a remaped key (and indicate to the OS that the key has been handled)
			{
				RemapEntryListItem* pKeyListItem = g_KeyTranslationTable[pHook->vkCode];
				while (NULL != pKeyListItem)
				{
					InputConfig* pKeyDef = &pKeyListItem->pEntry->inputConfig;
					if ((bAlt == pKeyDef->inputFlag.bAlt) &&
						(bControl == pKeyDef->inputFlag.bControl) &&
						(bShift == pKeyDef->inputFlag.bShift))
					{
						bSentInput = true;
						break;
					}
					pKeyListItem = pKeyListItem->pNext;
				}
			}
		break;
		}
		LogDebugMessage("LowLevelKeyboardProc Complete");
	}
	return bSentInput ? 1 : CallNextHookEx(NULL, nCode, wParam, lParam);
}
