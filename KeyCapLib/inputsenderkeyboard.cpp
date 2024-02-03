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

#include "inputsenderkeyboard.h"
#include "keycaputil.h"

/*
Sends the necessary inputs to complete the trigger (modifier keys)

pTriggerDefinition: pointer to a key definition for the trigger
*/
void SendTriggerEndInputKeys(RemapEntry* pRemapEntry)
{
	InputConfig* pInputConfig = &pRemapEntry->inputConfig;

	INPUT inputBuffer[MAX_KEY_INPUT_PER_STROKE];
	memset(&inputBuffer, 0, sizeof(INPUT) * MAX_KEY_INPUT_PER_STROKE);

	if (!pInputConfig)
	{
		return;
	}
	int nIndex = 0;

	if (pInputConfig->inputFlag.bShift)
	{
		// check the required input key to see if this was pressed and force a keyup to eliminate it being passed on
		// example: Shift + S >> f (without the keyup Shift + S >> F because shift is technically down)
		// problem: multiple key up messages (?) ... does it matter?
		// yes: slightly, Control + S >> x requires Control to be pressed again due to the keyup message (possible new flag?)
		AppendSingleKey(VK_SHIFT, &inputBuffer[nIndex++], KEYEVENTF_KEYUP);
	}

	if (pInputConfig->inputFlag.bControl)
	{
		AppendSingleKey(VK_CONTROL, &inputBuffer[nIndex++], KEYEVENTF_KEYUP);
	}

	if (pInputConfig->inputFlag.bAlt)
	{
		// Note: Application menus become active with ALT is pressed, to get out of the menu alt must be "pressed" a second time
		AppendSingleKey(VK_MENU, &inputBuffer[nIndex++], KEYEVENTF_KEYUP);
		AppendSingleKey(VK_MENU, &inputBuffer[nIndex++], 0);
		AppendSingleKey(VK_MENU, &inputBuffer[nIndex++], KEYEVENTF_KEYUP);
	}

	if (nIndex == 0)
	{
		// no keys to send nothing to do
		return;
	}

	LogDebugMessage("Sending Trigger End Inputs");
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		LogDebugMessage("Type:%s VKey:%d", GetKeyFlagsString(inputBuffer[nTemp].ki.dwFlags), inputBuffer[nTemp].ki.wVk);
	}

	UINT inputsSent = SendInput(nIndex, inputBuffer, sizeof(INPUT));
	LogDebugMessage("Sent Trigger End Inputs (%d/%d)", inputsSent, nIndex);
}

/*
Sends the desired keyboard input in the necessary order to achieve the desired input (or at least try)

pKeyDef: pointer to a key definition for a keyboard input to send
pTriggerDefinition: The definition of the triggering key. Modifiers like shift/alt/ctrl require special handling under
certain circumstances
*/
void SendInputKeys(RemapEntryState* pRemapEntryState, OutputConfig* pKeyDef)
{
	int nIndex = 0;
	bool bSendKeyDown = IsButtonDownRequired(pRemapEntryState, pKeyDef);
	bool bSendKeyUp = IsButtonUpRequired(pRemapEntryState, pKeyDef);
	INPUT inputBuffer[MAX_KEY_INPUT_PER_STROKE];
	memset(&inputBuffer, 0, sizeof(INPUT) * MAX_KEY_INPUT_PER_STROKE);

	if (pKeyDef->outputFlag.bToggle && pKeyDef->virtualKey >= MAX_VKEY)
	{
#if _DEBUG
		LogDebugMessage("---- ERROR Cannot have a vkey value over 255: %d", pKeyDef->virtualKey);
#endif
		return;
	}

	// NOTE: in all cases the key and all modifiers are sent (up or down)

	if (bSendKeyDown)
	{
		// flags first then key
		ProcessModifierKeys(pKeyDef, &inputBuffer[nIndex], &nIndex, 0);

		AppendSingleKey(pKeyDef->virtualKey, &inputBuffer[nIndex++], 0);
	}

	if (bSendKeyUp)
	{
		// key first then flags
		AppendSingleKey(pKeyDef->virtualKey, &inputBuffer[nIndex++], KEYEVENTF_KEYUP);

		ProcessModifierKeys(pKeyDef, &inputBuffer[nIndex], &nIndex, KEYEVENTF_KEYUP);
	}
	if (!bSendKeyDown && !bSendKeyUp)
	{
#if _DEBUG
		LogDebugMessage("Nothing to send for this keyout. Misconfigured output.");
#endif
		return;
	}

#if _DEBUG
	LogDebugMessage("[Sending Inputs]");
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		// TODO: support Shift+W toggle (the toggle ends up requiring detection of the shift key)
		LogDebugMessage("%s: %d", GetKeyFlagsString(inputBuffer[nTemp].ki.dwFlags), inputBuffer[nTemp].ki.wVk);
	}
#endif

	UINT inputsSent = SendInput(nIndex, inputBuffer, sizeof(INPUT));
#if _DEBUG
	LogDebugMessage("[Sent Inputs] (%d/%d)", inputsSent, nIndex);
#endif
}

/*
Assigns the desired keyboard input to send (see win32 INPUT documentation as it relates to SendInput)

keyScan: the virtual key value to set
inputChar: The scan value to set
dwFlags: The flags value to set
*/
void AppendSingleKey(short keyScan, INPUT* inputChar, DWORD dwFlags)
{
	memset(inputChar, 0, sizeof(INPUT));

	inputChar->type = INPUT_KEYBOARD;
	inputChar->ki.wVk = LOBYTE(keyScan);
	inputChar->ki.wScan = MapVirtualKey(LOBYTE(keyScan), 0);
	inputChar->ki.dwFlags = dwFlags;
	/*
	LogDebugMessage("Append Input: %d 0x%02x (%s)", 
		inputChar->ki.wVk,
		inputChar->ki.wVk,
		GetFlagsString(dwFlags)
	);*/
}

void ProcessModifierKeys(OutputConfig* pKeyDef, INPUT* pInput, int* nIndex, DWORD dwFlags)
{
	if (pKeyDef->outputFlag.bShift)
	{
		AppendSingleKey(VK_SHIFT, pInput++, dwFlags);
		*nIndex+=1;
	}

	if (pKeyDef->outputFlag.bControl)
	{
		AppendSingleKey(VK_CONTROL, pInput++, dwFlags);
		*nIndex += 1;
	}

	if (pKeyDef->outputFlag.bAlt)
	{
		AppendSingleKey(VK_MENU, pInput++, dwFlags);
		*nIndex += 1;
	}
}

char* GetKeyFlagsString(DWORD dwFlags)
{
	if ((dwFlags & KEYEVENTF_KEYUP) == KEYEVENTF_KEYUP)
	{
		return "KeyUp";
	}
	return "KeyDown";
}