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

#include "mouseinput.h"
#include "keycaptureutil.h"

static const unsigned char g_MouseDownMap[] = { 0, MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_RIGHTDOWN, MOUSEEVENTF_MIDDLEDOWN };
static const unsigned char g_MouseUpMap[] = { 0, MOUSEEVENTF_LEFTUP, MOUSEEVENTF_RIGHTUP, MOUSEEVENTF_MIDDLEUP };

char* GetMouseDescription(unsigned char nVkKey);

/*
Sends the desired mouse input

pKeyDef: pointer to a key definition for a mouse input
*/
void SendInputMouse(RemapEntry* pRemapEntry, OutputConfig *pKeyDef)
{
	INPUT inputBuffer[MAX_KEY_INPUT_PER_STROKE];
	memset(&inputBuffer, 0, sizeof(INPUT) * MAX_KEY_INPUT_PER_STROKE);

	bool bSendMouseDown = IsButtonDownRequired(pRemapEntry, pKeyDef);
	bool bSendMouseUp = IsButtonUpRequired(pRemapEntry, pKeyDef);

	if (pKeyDef->virtualKey == MOUSE_NONE)
	{
		return;
	}

	int nIndex = 0;

	if (bSendMouseDown)
	{
		AppendSingleMouse(&inputBuffer[nIndex++], g_MouseDownMap[pKeyDef->virtualKey]);
	}

	if (bSendMouseUp)
	{
		AppendSingleMouse(&inputBuffer[nIndex++], g_MouseUpMap[pKeyDef->virtualKey]);
	}

	LogDebugMessage("Sending Mouse Inputs");
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		LogDebugMessage("%s", GetMouseDescription(inputBuffer[nTemp].mi.dwFlags));
	}

	UINT inputsSent = SendInput(nIndex, inputBuffer, sizeof(INPUT));
	LogDebugMessage("Sent Mouse Inputs (%d/%d)", inputsSent, nIndex);
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

	//LogDebugMessage("Append Mouse Action: %s", GetMouseDescription(nVkKey));
}

// NOT THREAD SAFE
char* GetMouseDescription(unsigned char nVkKey)
{
	static char mouseOutputString[1024];
	switch (nVkKey)
	{
	case MOUSEEVENTF_LEFTDOWN:
		snprintf(mouseOutputString, sizeof(mouseOutputString), "LeftMouseDown");
		break;
	case MOUSEEVENTF_RIGHTDOWN:
		snprintf(mouseOutputString, sizeof(mouseOutputString), "RightMouseDown");
		break;
	case MOUSEEVENTF_MIDDLEDOWN:
		snprintf(mouseOutputString, sizeof(mouseOutputString), "MiddleMouseDown");
		break;
	case MOUSEEVENTF_LEFTUP:
		snprintf(mouseOutputString, sizeof(mouseOutputString), "LeftMouseUp");
		break;
	case MOUSEEVENTF_RIGHTUP:
		snprintf(mouseOutputString, sizeof(mouseOutputString), "RightMouseUp");
		break;
	case MOUSEEVENTF_MIDDLEUP:
		snprintf(mouseOutputString, sizeof(mouseOutputString), "MiddleMouseUp");
		break;
	}
	return mouseOutputString;
}