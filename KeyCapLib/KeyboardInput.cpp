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

#include "KeyboardInput.h"

bool g_KeyToggleHistory[MAX_VKEY];

/*
Sends the necessary inputs to complete the trigger (modifier keys)

pTriggerDefinition: pointer to a key definition for the trigger
*/
void SendTriggerEndInputKeys(InputConfig* pInputConfig)
{
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
		AppendSingleKey(VK_SHIFT, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
	}

	if (pInputConfig->inputFlag.bControl)
	{
		AppendSingleKey(VK_CONTROL, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
	}

	if (pInputConfig->inputFlag.bAlt)
	{
		// Note: Application menus become active with ALT is pressed, to get out of the menu alt must be "pressed" a second time
		AppendSingleKey(VK_MENU, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
		AppendSingleKey(VK_MENU, &g_inputBuffer[nIndex++], 0);
		AppendSingleKey(VK_MENU, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
	}

	if (nIndex == 0)
	{
		// no keys to send nothing to do
		return;
	}

#ifdef _DEBUG
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		LogDebugMessage("Sending Trigger End: (flags)%s VKey: %d\n", GetFlagsString(g_inputBuffer[nTemp].ki.dwFlags), g_inputBuffer[nTemp].ki.wVk);
		//sprintf_s(outputchar, "Sending Trigger End: (flags)%s VKey: %d\n", getFlagsString(g_inputBuffer[nTemp].ki.dwFlags), g_inputBuffer[nTemp].ki.wVk);
		//OutputDebugStringA(outputchar);
	}
#endif
	SendInput(nIndex, g_inputBuffer, sizeof(INPUT));
}

/*
Sends the desired keyboard input in the necessary order to achieve the desired input (or at least try)

pKeyDef: pointer to a key definition for a keyboard input to send
pTriggerDefinition: The definition of the triggering key. Modifiers like shift/alt/ctrl require special handling under
certain circumstances
*/
void SendInputKeys(OutputConfig* pKeyDef)
{
	int nIndex = 0;

	bool bSendKeyDown = true;
	bool bSendKeyUp = true;

	if (pKeyDef->outputFlag.bToggle)
	{
		if (pKeyDef->virtualKey >= MAX_VKEY)
		{
			LogDebugMessage("---- ERROR Cannot have a vkey value over 255: %d\n", pKeyDef->virtualKey);
			return;
		}

		if (g_KeyToggleHistory[pKeyDef->virtualKey])
		{
			bSendKeyDown = false;
		}
		else
		{
			bSendKeyUp = false;
		}
		g_KeyToggleHistory[pKeyDef->virtualKey] = !g_KeyToggleHistory[pKeyDef->virtualKey];
	}

	if (bSendKeyDown)
	{
		// flags first then key
		if (pKeyDef->outputFlag.bShift)
		{
			AppendSingleKey(VK_SHIFT, &g_inputBuffer[nIndex++], 0);
		}

		if (pKeyDef->outputFlag.bControl)
		{
			AppendSingleKey(VK_CONTROL, &g_inputBuffer[nIndex++], 0);
		}

		if (pKeyDef->outputFlag.bAlt)
		{
			AppendSingleKey(VK_MENU, &g_inputBuffer[nIndex++], 0);
		}

		AppendSingleKey(pKeyDef->virtualKey, &g_inputBuffer[nIndex++], 0);
	}

	if (bSendKeyUp)
	{
		// key first then flags
		AppendSingleKey(pKeyDef->virtualKey, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);

		if (pKeyDef->outputFlag.bShift)
		{
			AppendSingleKey(VK_SHIFT, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
		}
		if (pKeyDef->outputFlag.bControl)
		{
			AppendSingleKey(VK_CONTROL, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
		}
		if (pKeyDef->outputFlag.bAlt)
		{
			AppendSingleKey(VK_MENU, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
		}
	}
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		// TODO: support Shift+W toggle (the toggle ends up requiring detection of the shift key)
		LogDebugMessage("Sending Input Keys: (flags)%s VKey: %d\n", GetFlagsString(g_inputBuffer[nTemp].ki.dwFlags), g_inputBuffer[nTemp].ki.wVk);
	}
	// DOES this free up the g_inputBuffer if requested? Could a stack pointer to memory work? this would eliminate the dumb static
	SendInput(nIndex, g_inputBuffer, sizeof(INPUT));
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
	LogDebugMessage("Append Action: %d 0x%02x (%s)", 
		inputChar->ki.wVk,
		inputChar->ki.wVk,
		GetFlagsString(dwFlags)
	);
}

char* GetFlagsString(DWORD dwFlags)
{
	if ((dwFlags & KEYEVENTF_KEYUP) == KEYEVENTF_KEYUP)
	{
		return "KeyUp";
	}
	return "KeyDown";
}