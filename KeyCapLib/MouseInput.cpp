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

#include "MouseInput.h"

unsigned char g_MouseDownMap[] = { 0, MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_RIGHTDOWN, MOUSEEVENTF_MIDDLEDOWN };
unsigned char g_MouseUpMap[] = { 0, MOUSEEVENTF_LEFTUP, MOUSEEVENTF_RIGHTUP, MOUSEEVENTF_MIDDLEUP };

/*
Sends the desired mouse input

pKeyDef: pointer to a key definition for a mouse input
*/
void SendInputMouse(OutputConfig *pKeyDef)
{
	if (pKeyDef->virtualKey == MOUSE_NONE)
	{
		return;
	}

	int nIndex = 0;

	if (pKeyDef->outputFlag.bToggle)
	{
		if (g_MouseToggleHistory[pKeyDef->virtualKey])
		{
			AppendSingleMouse(&g_inputBuffer[nIndex++], g_MouseUpMap[pKeyDef->virtualKey]);
		}
		else
		{
			AppendSingleMouse(&g_inputBuffer[nIndex++], g_MouseDownMap[pKeyDef->virtualKey]);
		}
		g_MouseToggleHistory[pKeyDef->virtualKey] = !g_MouseToggleHistory[pKeyDef->virtualKey];
	}
	else
	{
		AppendSingleMouse(&g_inputBuffer[nIndex++], g_MouseDownMap[pKeyDef->virtualKey]);
		AppendSingleMouse(&g_inputBuffer[nIndex++], g_MouseUpMap[pKeyDef->virtualKey]);
		g_MouseToggleHistory[pKeyDef->virtualKey] = false;
	}
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		LogDebugMessage("Sending Mouse: (flags)0x%02x %d\n", g_inputBuffer[nTemp].ki.dwFlags, g_inputBuffer[0].mi.dwFlags);
	}
	SendInput(nIndex, g_inputBuffer, sizeof(INPUT));
}

/*
Assigns the desired mouse input to send (see win32 INPUT documentation as it relates to SendInput)

inputChar: The scan value to set
nVkKey: The key to set/send as input
*/
void AppendSingleMouse(INPUT* inputChar, unsigned char nVkKey)
{
	memset(inputChar, 0, sizeof(INPUT));

	inputChar->type = INPUT_MOUSE;
	inputChar->mi.dwExtraInfo = 0;
	inputChar->mi.mouseData = 0;
	inputChar->mi.dwExtraInfo = 0;
	inputChar->mi.dwFlags = nVkKey;
	LogDebugMessage("Append Mouse Action: %d 0x%02x",
		inputChar->ki.wVk,
		inputChar->ki.wVk
	);
}