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

#include "keycapturestructs.h"
#include "keycaptureutil.h"
#include "sendinputthread.h"
#include "keyboardinput.h"
#include "mouseinput.h"

DWORD InitiateSendInput(RemapEntry* pRemapEntry, RemapEntryState* pRemapEntryState);

/*
Thread responsible for sending the desired inputs to the OS (see standard win32 definition)
*/
DWORD WINAPI SendInputThread(LPVOID lpParam)
{
	RemapEntryContainer* pRemapEntryContainer = static_cast<RemapEntryContainer*>(lpParam);
	RemapEntry* pRemapEntry = pRemapEntryContainer->pEntry;
	RemapEntryState* pRemapEntryState = pRemapEntryContainer->pEntryState;
	return InitiateSendInput(pRemapEntry, pRemapEntryState);
}
/*
Thread responsible for sending the desired inputs to the OS (see standard win32 definition)
*/
DWORD InitiateSendInput(RemapEntry* pRemapEntry, RemapEntryState* pRemapEntryState)
{
	// get a pointer to the OutputConfig data that is AFTER the RemapEntry field
	OutputConfig* pOutputConfig = (OutputConfig*)(pRemapEntry + 1);
	// get a pointer to the first byte AFTER all the OutputConfig entries
	OutputConfig* pTerminator = (OutputConfig*)(pOutputConfig + pRemapEntry->outputCount);
	if (pOutputConfig->outputFlag.bDoNothing)
	{
		return 0; // done!
	}

	// trigger any necessary release of shift/control/alt by passing the trigger key definition
	SendTriggerEndInputKeys(pRemapEntry);

	if (pRemapEntryState->bRepeating)
	{
		pRemapEntryState->bRepeating = false;
		LogDebugMessage("SendInputThread Completed (ended Repeat)");
		return 0;
	}

	// scan for repeating entries
	while (pOutputConfig < pTerminator)
	{
		if (pOutputConfig->outputFlag.bRepeat)
			pRemapEntryState->bRepeating = true;
		pOutputConfig++; // move the pointer forward one KeyDefinition
	}

	// reset output pointer
	pOutputConfig = (OutputConfig*)(pRemapEntry + 1);

	bool firstPassComplete = false;

	// always run once
	do
	{
		// iterate over the target inputs
		while (pOutputConfig < pTerminator)
		{
			if (!pOutputConfig->outputFlag.bRepeat && firstPassComplete)
			{
				// skip this output
				pOutputConfig++; // move the pointer forward one KeyDefinition
			}

			char* pOutputConfigDescription = GetOutputConfigDescription(*pOutputConfig);
			LogDebugMessage("Performing %s Output Action: %s",
				pOutputConfig->outputFlag.bMouseOut ? "Mouse" : "Keyboard",
				pOutputConfigDescription
			);
			free(pOutputConfigDescription);

			// mouse input
			if (pOutputConfig->outputFlag.bMouseOut)
			{
				SendInputMouse(pRemapEntryState, pOutputConfig);
			}
			// just a delay
			else if (pOutputConfig->outputFlag.bDelay)
			{
				Sleep(1000 * pOutputConfig->parameter);
			}
			// keyboard input
			else
			{
				SendInputKeys(pRemapEntryState, pOutputConfig);
			}
			pOutputConfig++; // move the pointer forward one KeyDefinition
		}

		// reset output pointer
		pOutputConfig = (OutputConfig*)(pRemapEntry + 1);
		firstPassComplete = true;
		Sleep(100);
		if (pRemapEntryState->bRepeating)
		{
			LogDebugMessage("SendInputThread Repeating...");
		}
	} while (pRemapEntryState->bRepeating);

	// flip the overall toggle state
	pRemapEntryState->bToggled = !pRemapEntryState->bToggled;

	LogDebugMessage("SendInputThread Completed");
	return 0;
}