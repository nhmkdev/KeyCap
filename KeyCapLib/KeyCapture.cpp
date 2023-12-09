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

// KeyCapture.cpp : Defines the entry point for key capture and re-map
//

#include "keycapture.h"
#include "keyboardproc.h"
#include "mouseinput.h"
#include "configfile.h"

// === prototypes
// avoid mangling the function names (necessary for export and usage in C#)
extern "C"
{
	__declspec(dllexport) int LoadAndCaptureFromFile(HINSTANCE hInstance, char* sFile);
	__declspec(dllexport) void ShutdownCapture();
}

// non extern functions
void InitiallizeEntryContainerListItem(RemapEntryContainerListItem* pKeyItem, RemapEntry* pEntry);

// sweet globals
HHOOK g_hookKeyboard = NULL, g_hookMouse = NULL;
RemapEntryContainerListItem* g_KeyTranslationTable[WIN_KEY_COUNT];
RemapEntry* g_KeyTranslationHead = NULL;
void* g_KeyTranslationEnd = NULL; // pointer indicating the end of the input file data

/*
Performs the load file operation and initiates the keyboard capture process

hInstance: The HINSTANCE of the KeyCapture application
sFile: The file to load

returns: A HOOK_RESULT value depending on the outcome of the file load and keyboard hook intiailization
*/
__declspec(dllexport) int LoadAndCaptureFromFile(HINSTANCE hInstance, char* sFile)
{
	ValidateStructs();

	int nLoadResult = LoadFile(sFile, &g_KeyTranslationHead, &g_KeyTranslationEnd);
	switch (nLoadResult)
	{
		case INPUT_MISSING:
		case INPUT_ZERO:
		case INPUT_BAD:
			ShutdownCapture();
			return nLoadResult;
	}

	// validate file (and compile the "hash" table g_KeyTranslationTable)
	memset(g_KeyTranslationTable, 0, WIN_KEY_COUNT * sizeof(RemapEntryContainerListItem*));

	RemapEntry* pEntry = g_KeyTranslationHead;
	bool bValidTranslationSet = false;
		
	// The translation table contains linked lists of all the output sets for a given key due to the flag keys (shift/alt/ctrl)
	// Example: input 'a' will be in the same list as 'shift+a' 'alt+a' 'ctrl+a' (or any combos like 'alt+shift+a')
	while(NULL != pEntry)
	{
		if(0 == pEntry->outputCount)
		{
			ShutdownCapture();
			return INPUT_BAD;
		}
		// TODO: just get a pointer to g_KeyTranslationTable[pKey->inputConfig.virtualKey] ?
#ifdef _DEBUG
		char* pInputConfigDescription = GetInputConfigDescription(pEntry->inputConfig);
		LogDebugMessage("Loading %s Outputs: %d", pInputConfigDescription, pEntry->outputCount);
		free(pInputConfigDescription);
#endif
		// if the entry doesn't exist yet for the given input vkey create a new one with a null next pointer
		if(NULL == g_KeyTranslationTable[pEntry->inputConfig.virtualKey])
		{
			g_KeyTranslationTable[pEntry->inputConfig.virtualKey] = (RemapEntryContainerListItem*)malloc(sizeof(RemapEntryContainerListItem));
			InitiallizeEntryContainerListItem(g_KeyTranslationTable[pEntry->inputConfig.virtualKey], pEntry);
			g_KeyTranslationTable[pEntry->inputConfig.virtualKey]->pNext = NULL;
		}
		
		// if the entry does exist create a new entry and append it to the existing linked list
		else
		{
			RemapEntryContainerListItem* pKeyItem = g_KeyTranslationTable[pEntry->inputConfig.virtualKey];
			while(NULL != pKeyItem->pNext)
			{
				pKeyItem = pKeyItem->pNext;
			}
			pKeyItem->pNext = (RemapEntryContainerListItem*)malloc(sizeof(RemapEntryContainerListItem));
			pKeyItem = pKeyItem->pNext;
			InitiallizeEntryContainerListItem(pKeyItem, pEntry);
			pKeyItem->pNext = NULL;
		}
		// jump to the next entry
		pEntry = (RemapEntry*)(&pEntry->outputCount + (sizeof(unsigned int) + (pEntry->outputCount * sizeof(OutputConfig))));
		if(pEntry > g_KeyTranslationEnd)
		{
			LogDebugMessage("Load Failed. Attempted to read bytes beyond file boundary.");
			ShutdownCapture();
			return INPUT_BAD;
		}
		else if(pEntry == g_KeyTranslationEnd)
		{
			bValidTranslationSet = true;
			break;
		}
	}

	if(bValidTranslationSet)
	{
		// Note: This fails in VisualStudio if managed debugging is NOT enabled in the project(!)
		g_hookKeyboard = SetWindowsHookEx( WH_KEYBOARD_LL, LowLevelKeyboardProc, hInstance, NULL);
		g_hookMouse = SetWindowsHookEx(WH_MOUSE_LL, LowLevelKeyboardProc, hInstance, NULL);
		if(g_hookKeyboard && g_hookMouse)
		{
			return HOOK_CREATION_SUCCESS;
		}
		ShutdownCapture();
		return HOOK_CREATION_FAILURE;
	}

	ShutdownCapture();
	return INPUT_BAD;
}

void InitiallizeEntryContainerListItem(RemapEntryContainerListItem* pKeyItem, RemapEntry* pEntry)
{
	pKeyItem->pEntryContainer = (RemapEntryContainer*)malloc(sizeof(RemapEntryContainer));
	pKeyItem->pEntryContainer->pEntryState = (RemapEntryState*)calloc(1, sizeof(RemapEntryState));
	pKeyItem->pEntryContainer->pEntry = pEntry;
}

void ShutdownInputThreads(bool forceShutdown)
{
	// signal shutdown for all entries with a thread handle
	for (int nIdx = 0; nIdx < WIN_KEY_COUNT; nIdx++)
	{
		if (NULL != g_KeyTranslationTable[nIdx])
		{
			RemapEntryContainerListItem* pListItem = g_KeyTranslationTable[nIdx];
			RemapEntryContainerListItem* pNextItem = NULL;
			while (NULL != pListItem)
			{
				pNextItem = pListItem->pNext;
				if (NULL != pListItem->pEntryContainer->pEntryState->threadHandle)
				{
					pListItem->pEntryContainer->pEntryState->bShutdown = true;
				}
				pListItem = pNextItem;
			}
		}
	}

	// a forced shutdown will kill threads (mostly for application exit)
	if(!forceShutdown) return;

	// monitor and terminate threads for all entries with a thread handle
	for (int nIdx = 0; nIdx < WIN_KEY_COUNT; nIdx++)
	{
		if (NULL != g_KeyTranslationTable[nIdx])
		{
			RemapEntryContainerListItem* pListItem = g_KeyTranslationTable[nIdx];
			RemapEntryContainerListItem* pNextItem = NULL;
			while (NULL != pListItem)
			{
				pNextItem = pListItem->pNext;
				if (NULL != pListItem->pEntryContainer->pEntryState->threadHandle)
				{
					DWORD dwExitCode = WAIT_TIMEOUT;
					for (int shutdownIteration = 0; shutdownIteration < THREAD_SHUTDOWN_MAX_ATTEMPTS && dwExitCode != WAIT_OBJECT_0; shutdownIteration++)
					{
						// check on the state of the thread (for 100ms) (MS says not to use GetExitCodeThread unless the thread is known to be exited)
						dwExitCode = WaitForSingleObject(pListItem->pEntryContainer->pEntryState->threadHandle, THREAD_SHUTDOWN_ATTEMPT_DELAY_MS);
					}
					if (dwExitCode != WAIT_OBJECT_0)
					{
						LogDebugMessage("Force terminating thread!");
						TerminateThread(pListItem->pEntryContainer->pEntryState->threadHandle, 1);
					}
				}
				pListItem = pNextItem;
			}
		}
	}
}

/*
Shuts down the key capture hook and frees any allocated memory
*/
__declspec(dllexport) void ShutdownCapture()
{
	// stop any active threads
	ShutdownInputThreads(true);
	
	// disable the hooks
	if(g_hookKeyboard)
	{
		LogDebugMessage("Unhooked keyboard");
		UnhookWindowsHookEx(g_hookKeyboard);
		g_hookKeyboard = NULL;
	}
	if (g_hookMouse)
	{
		LogDebugMessage("Unhooked mouse");
		UnhookWindowsHookEx(g_hookMouse);
		g_hookMouse = NULL;
	}

	// memory clean up
	if(NULL != g_KeyTranslationHead)
	{
		LogDebugMessage("Cleared Memory!");
		free(g_KeyTranslationHead);
		g_KeyTranslationHead = NULL;
	}

	// clean up "hash" table
	for(int nIdx = 0; nIdx < WIN_KEY_COUNT; nIdx++)
	{
		if(NULL != g_KeyTranslationTable[nIdx])
		{
			RemapEntryContainerListItem* pListItem = g_KeyTranslationTable[nIdx];
			RemapEntryContainerListItem* pNextItem = NULL;
			while(NULL != pListItem)
			{
				pNextItem = pListItem->pNext;
				free(pListItem->pEntryContainer->pEntryState);
				free(pListItem->pEntryContainer);
				// NOTE: the entry itself was freed above
				free(pListItem);
				pListItem = pNextItem;
			}
			g_KeyTranslationTable[nIdx] = NULL;
		}
	}

	g_KeyTranslationEnd = NULL; 

	LogDebugMessage("Capture Shutdown");
}
