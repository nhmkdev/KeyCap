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

#include "keycapstructs.h"
#include "keycaputil.h"
#include "keycap.h"
#include "inputsenderthread.h"
#include "inputsenderkeyboard.h"
#include "inputsendermouse.h"

DWORD InitiateSendInput(RemapEntry* pRemapEntry, RemapEntryState* pRemapEntryState);

/*
Thread entry point so the parameter can be opened and evaluated (not-quite-critical wrapper method)
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
	DWORD repeatDelayMillis = MIN_REPEAT_DELAY_MS;
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
		pRemapEntryState->bShutdown = true;
		LogDebugMessage("SendInputThread Completed (ended Repeat)");
		return 0;
	}

	// scan for repeating entries (and the highest repeat delay)
	while (pOutputConfig < pTerminator)
	{
		if (pOutputConfig->outputFlag.bRepeat)
		{
			pRemapEntryState->bRepeating = true;
			repeatDelayMillis = max(pOutputConfig->parameter, repeatDelayMillis);
		}
		pOutputConfig++; // move the pointer forward one KeyDefinition
	}

	// reset output pointer
	pOutputConfig = (OutputConfig*)(pRemapEntry + 1);

	bool firstPassComplete = false;

	// always run once
	do
	{
		// iterate over the target inputs
		while (pOutputConfig < pTerminator && !pRemapEntryState->bShutdown)
		{
			if (!pOutputConfig->outputFlag.bRepeat && firstPassComplete)
			{
				// skip this output
				pOutputConfig++; // move the pointer forward one KeyDefinition
			}
#ifdef _DEBUG
			char* pOutputConfigDescription = GetOutputConfigDescription(*pOutputConfig);
			LogDebugMessage("Performing %s Output Action: %s",
				pOutputConfig->outputFlag.bMouseOut ? "Mouse" : "Keyboard (or other)",
				pOutputConfigDescription
			);
			free(pOutputConfigDescription);
#endif
			// mouse input
			if(pOutputConfig->outputFlag.bDoNothing)
			{
				// nothing
			}
			if(pOutputConfig->outputFlag.bCancelActiveOutputs)
			{
				// NOTE: even this thread might be affected
				ShutdownInputThreads(false);
				break;
			}
			else if (pOutputConfig->outputFlag.bMouseOut)
			{
				SendInputMouse(pRemapEntryState, pOutputConfig);
			}
			else if (pOutputConfig->outputFlag.bDelay)
			{
				//Want to delay break...
				int iterations = pOutputConfig->parameter / 100;
				for (int nDelayCount = 0; nDelayCount < iterations; nDelayCount++)
				{
					Sleep(100);
					if (pRemapEntryState->bShutdown)
					{
#if _DEBUG
						LogDebugMessage("Exiting delay loop due to shutdown.");
#endif
						break;
					}
				}
			}
			else
			{
				// default to keyboard input
				SendInputKeys(pRemapEntryState, pOutputConfig);
			}
			pOutputConfig++; // move the pointer forward one KeyDefinition
		}

		// break out now
		if (pRemapEntryState->bShutdown)
		{
			break;
		}
		
		// reset output pointer
		pOutputConfig = (OutputConfig*)(pRemapEntry + 1);
		firstPassComplete = true;
		if (pRemapEntryState->bRepeating)
		{
			int iterations = repeatDelayMillis / MIN_REPEAT_DELAY_MS;
			for (int repeatDelayCount = 0; repeatDelayCount < iterations; repeatDelayCount++)
			{
				Sleep(100);
				if (pRemapEntryState->bShutdown)
				{
#if _DEBUG
					LogDebugMessage("Exiting repeat loop due to shutdown.");
#endif
					break;
				}
			}
#if _DEBUG
			if (!pRemapEntryState->bShutdown)
			{
				LogDebugMessage("SendInputThread Repeating...");
			}
#endif
		}
	} while (pRemapEntryState->bRepeating);

	// flip the overall toggle state (even when a thread is canceled this should flip back, arguably to false...)
	pRemapEntryState->bToggled = !pRemapEntryState->bToggled;

	// reset things on the remap entry
	pRemapEntryState->bShutdown = false;
	pRemapEntryState->bRepeating = false;
	pRemapEntryState->threadHandle = nullptr;
#ifdef _DEBUG
	char* pInputConfigDescription = GetInputConfigDescription(pRemapEntry->inputConfig);
	LogDebugMessage("SendInputThread Completed: %s", pInputConfigDescription);
	free(pInputConfigDescription);
#endif
	return 0;
}