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
void SendTriggerEndInputKeys(KeyDefinition* pTriggerDefinition)
{
	if (!pTriggerDefinition)
	{
		return;
	}
	int nIndex = 0;

	if (pTriggerDefinition->bShift)
	{
		// check the required input key to see if this was pressed and force a keyup to eliminate it being passed on
		// example: Shift + S >> f (without the keyup Shift + S >> F because shift is technically down)
		// problem: multiple key up messages (?) ... does it matter?
		// yes: slightly, Control + S >> x requires Control to be pressed again due to the keyup message (possible new flag?)
		AppendSingleKey(VK_SHIFT, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
	}

	if (pTriggerDefinition->bControl)
	{
		AppendSingleKey(VK_CONTROL, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
	}

	if (pTriggerDefinition->bAlt)
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
	char outputchar[256];
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		sprintf_s(outputchar, "Sending: (flags)%x %d\n", g_inputBuffer[nTemp].ki.dwFlags, g_inputBuffer[nTemp].ki.wVk);
		OutputDebugStringA(outputchar);
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
void SendInputKeys(KeyDefinition* pKeyDef)
{
#ifdef _DEBUG
	char outputchar[256];
#endif

	int nIndex = 0;

	bool bSendKeyDown = true;
	bool bSendKeyUp = true;

	if (pKeyDef->bToggle)
	{
		if (pKeyDef->nVkKey >= MAX_VKEY)
		{
#ifdef _DEBUG
			sprintf_s(outputchar, "---- ERROR Cannot have a vkey value over 255: %d\n", pKeyDef->nVkKey);
			OutputDebugStringA(outputchar);
#endif
			return;
		}

		if (g_KeyToggleHistory[pKeyDef->nVkKey])
		{
			bSendKeyDown = false;
		}
		else
		{
			bSendKeyUp = false;
		}
		g_KeyToggleHistory[pKeyDef->nVkKey] = !g_KeyToggleHistory[pKeyDef->nVkKey];
	}

	if (bSendKeyDown)
	{
		if (pKeyDef->bShift)
		{
			AppendSingleKey(VK_SHIFT, &g_inputBuffer[nIndex++], 0);
		}

		if (pKeyDef->bControl)
		{
			AppendSingleKey(VK_CONTROL, &g_inputBuffer[nIndex++], 0);
		}

		if (pKeyDef->bAlt)
		{
			AppendSingleKey(VK_MENU, &g_inputBuffer[nIndex++], 0);
		}

		// output the actual key
#ifdef _DEBUG
		sprintf_s(outputchar, "---- Appended (down) %d\n", pKeyDef->nVkKey);
		OutputDebugStringA(outputchar);
#endif
		AppendSingleKey(pKeyDef->nVkKey, &g_inputBuffer[nIndex++], 0);
	}

	if (bSendKeyUp)
	{
#ifdef _DEBUG
		sprintf_s(outputchar, "---- Appended (up) %d\n", pKeyDef->nVkKey);
		OutputDebugStringA(outputchar);
#endif

		AppendSingleKey(pKeyDef->nVkKey, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);

		// setup any necessary key event up messages
		if (pKeyDef->bShift)
		{
			AppendSingleKey(VK_SHIFT, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
		}
		if (pKeyDef->bControl)
		{
			AppendSingleKey(VK_CONTROL, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
		}
		if (pKeyDef->bAlt)
		{
			AppendSingleKey(VK_MENU, &g_inputBuffer[nIndex++], KEYEVENTF_KEYUP);
		}
	}
#ifdef _DEBUG
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		sprintf_s(outputchar, "Sending: (flags)%x %d\n", g_inputBuffer[nTemp].ki.dwFlags, g_inputBuffer[nTemp].ki.wVk);
		OutputDebugStringA(outputchar);
	}
#endif
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
}