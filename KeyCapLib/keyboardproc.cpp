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

extern RemapEntryContainerListItem* g_KeyTranslationTable[WIN_KEY_COUNT];

const int LONG_PRESS_MIN = 100;
const int KEY_DOWN_UNSET = 0; // key not being held
const int KEY_DOWN_EVENT_FIRED = 1; // key is held and already sent output
// other values stored in g_KeyDownTime are the original key down time as returned by GetTickCount64
ULONGLONG g_KeyDownTime[WIN_KEY_COUNT];

void ProcessLongPressKeyDown(const InputConfig* pKeyDef, const RemapEntryContainerListItem* pKeyListItem);
void ProcessLongPressKeyUp(const InputConfig* pKeyDef);

/*
Implementation of the win32 LowLevelKeyboardProc (see docs for information)

This is a special implementation to avoid sending the actual keyup/keydown
messages (as those are the keys being captured). A separate thread is
created to send out the key(s) to send to the os.
*/
LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	KBDLLHOOKSTRUCT *pHook = reinterpret_cast<KBDLLHOOKSTRUCT*>(lParam);
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
				// NOTE: key down events will come in while a key is being held

				const RemapEntryContainerListItem* pKeyListItem = g_KeyTranslationTable[pHook->vkCode];
				while (NULL != pKeyListItem)
				{
					const InputConfig* pKeyDef = &pKeyListItem->pEntryContainer->pEntry->inputConfig;
					if ((bAlt == pKeyDef->inputFlag.bAlt) &&
						(bControl == pKeyDef->inputFlag.bControl) &&
						(bShift == pKeyDef->inputFlag.bShift)
						&& !pKeyDef->inputFlag.bLongPress)
					{
#ifdef _DEBUG
						char* pInputConfigDescription = GetInputConfigDescription(*pKeyDef);
						LogDebugMessage("Detected Key Press: %s Outputs: %d", pInputConfigDescription, pKeyListItem->pEntryContainer->pEntry->outputCount);
						free(pInputConfigDescription);
#endif
						// If there is NOT an existing thread OR the existing thread is running a repeat another key press is allowed
						if (NULL == pKeyListItem->pEntryContainer->pEntryState->threadHandle || pKeyListItem->pEntryContainer->pEntryState->bRepeating)
						{
							pKeyListItem->pEntryContainer->pEntryState->threadHandle = CreateThread(NULL, 0, SendInputThread, pKeyListItem->pEntryContainer, 0, NULL);
						}
						// block further processing with the inputs
						bSentInput = true;
						break;
					}
					if(!bAlt
						&& !bControl
						&& !bShift
						&& pKeyDef->inputFlag.bLongPress)
					{
						ProcessLongPressKeyDown(pKeyDef, pKeyListItem);
						// block further processing with the inputs
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
				RemapEntryContainerListItem* pKeyListItem = g_KeyTranslationTable[pHook->vkCode];
				while (NULL != pKeyListItem)
				{
					InputConfig* pKeyDef = &pKeyListItem->pEntryContainer->pEntry->inputConfig;
					if (bAlt == pKeyDef->inputFlag.bAlt
						&& bControl == pKeyDef->inputFlag.bControl 
						&& bShift == pKeyDef->inputFlag.bShift
						&& !pKeyDef->inputFlag.bLongPress)
					{
						// block further processing with the inputs
						bSentInput = true;
						break;
					}
					if (!bAlt 
						&& !bControl 
						&& !bShift
						&& pKeyDef->inputFlag.bLongPress)
					{
						ProcessLongPressKeyUp(pKeyDef);
						// block further processing with the inputs
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

void ProcessLongPressKeyDown(const InputConfig* pKeyDef, const RemapEntryContainerListItem* pKeyListItem)
{
	switch (g_KeyDownTime[pKeyDef->virtualKey])
	{
	case KEY_DOWN_EVENT_FIRED:
		// nothing for now
		break;
	case KEY_DOWN_UNSET:
		{
			const ULONGLONG tickCount = GetTickCount64();
			g_KeyDownTime[pKeyDef->virtualKey] = tickCount;
#ifdef _DEBUG
			LogDebugMessage("Detected LONGPRESS Key Down: %d Tick: %d", pKeyDef->virtualKey, tickCount);
#endif
		}
		break;
	default:
		{
			const ULONGLONG tickCount = GetTickCount64();
			const unsigned int longPressMinimum = max(LONG_PRESS_MIN, pKeyDef->parameter);
#if false
			// windows has a built repeat rate limiter so the shortest time is 250ms
			const ULONGLONG diff = tickCount - g_KeyDownTime[pKeyDef->virtualKey];
			LogDebugMessage("diff: %d", diff);
#endif
			if (tickCount - g_KeyDownTime[pKeyDef->virtualKey] >= longPressMinimum)
			{
#ifdef _DEBUG
				char* pInputConfigDescription = GetInputConfigDescription(*pKeyDef);
				LogDebugMessage("Detected LONGPRESS [Key Down: %s] [Outputs: %d][longPressMinimum: %d]",
					pInputConfigDescription,
					pKeyListItem->pEntryContainer->pEntry->outputCount,
					longPressMinimum);
				free(pInputConfigDescription);
#endif
				// If there is NOT an existing thread OR the existing thread is running a repeat another key press is allowed
				if (NULL == pKeyListItem->pEntryContainer->pEntryState->threadHandle || pKeyListItem->pEntryContainer->pEntryState->bRepeating)
				{
					pKeyListItem->pEntryContainer->pEntryState->threadHandle = CreateThread(NULL, 0, SendInputThread, pKeyListItem->pEntryContainer, 0, NULL);
				}
				g_KeyDownTime[pKeyDef->virtualKey] = KEY_DOWN_EVENT_FIRED;
			}
		}
		break;
	}
}

void ProcessLongPressKeyUp(const InputConfig* pKeyDef)
{
	if (g_KeyDownTime[pKeyDef->virtualKey] != KEY_DOWN_EVENT_FIRED &&
		g_KeyDownTime[pKeyDef->virtualKey] != KEY_DOWN_UNSET)
	{
		const ULONGLONG tickCount = GetTickCount64();
		LogDebugMessage("Detected LONGPRESS Key UP (too short): %d Tick: %d (Old Tick: %d)", pKeyDef->virtualKey, tickCount, g_KeyDownTime[pKeyDef->virtualKey]);
		// when a longpress "fails" just send the normal keypress
		SendInputKeypress(pKeyDef);
	}
	g_KeyDownTime[pKeyDef->virtualKey] = KEY_DOWN_UNSET;
}
