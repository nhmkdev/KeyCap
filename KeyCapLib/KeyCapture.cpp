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

// KeyCapture.cpp : Defines the entry point for key capture and re-map
//

#include "stdafx.h"
#include "KeyCapture.h"
#include "MouseInput.h"
#include "KeyboardInput.h"

// === enums

enum HOOK_RESULT
{
	HOOK_CREATION_SUCCESS = 0,
	HOOK_CREATION_FAILURE,
	INPUT_MISSING,
	INPUT_ZERO,
	INPUT_BAD
};

// === consts and defines
const int WIN_KEY_COUNT = 256;

// === prototypes
// avoid mangling the function names (necessary for export and usage in C#)
extern "C"
{
	__declspec(dllexport) int LoadAndCaptureFromFile(HINSTANCE hInstance, char* sFile);
	__declspec(dllexport) void ShutdownCapture();
}

LRESULT CALLBACK LowLevelKeyboardProc(int nCode,WPARAM wParam,LPARAM lParam);
DWORD WINAPI SendInputThread( LPVOID lpParam );
void SendTriggerEndInputKeys(InputConfig* pTriggerDefinition);
bool CheckKeyFlags(char nFlags, bool bAlt, bool bControl, bool bShift);

// === sweet globals
RemapEntryListItem* g_KeyTranslationTable[WIN_KEY_COUNT];

// TODO: this is strange
RemapEntry* g_KeyTranslationHead = NULL;
void* g_KeyTranslationEnd = NULL; // pointer indicating the end of the input file data
HHOOK g_hookMain = NULL;

/*
Performs the load file operation and initiates the keyboard capture process

hInstance: The HINSTANCE of the KeyCapture application
sFile: The file to load

returns: A HOOK_RESULT value depending on the outcome of the file load and keyboard hook intiailization
*/
__declspec(dllexport) int LoadAndCaptureFromFile(HINSTANCE hInstance, char* sFile)
{
	// TODO: need to have a constant somewhere...
	LogDebugMessage("InputConfig: %d", sizeof(InputConfig));
	LogDebugMessage("OutputConfig: %d", sizeof(OutputConfig));
	LogDebugMessage("InputFlag: %d", sizeof(InputFlag));
	LogDebugMessage("OutputFlag: %d", sizeof(OutputFlag));
	LogDebugMessage("RemapEntry: %d", sizeof(RemapEntry));
	assert(12 == sizeof(InputConfig)); // if this is invalid the configuration tool and kfg files will not be valid
	assert(12 == sizeof(OutputConfig)); // if this is invalid the configuration tool and kfg files will not be valid
	assert(4 == sizeof(InputFlag));
	assert(4 == sizeof(OutputFlag));
	assert(16 == sizeof(RemapEntry));

	// convert to wide string for CreateFile
	wchar_t wPath[MAX_PATH + 1];
	size_t nConvertedCount = 0;
	mbstowcs_s(&nConvertedCount, wPath, sFile, MAX_PATH + 1);

	if (0 == nConvertedCount)
	{
		return INPUT_MISSING;
	}

	// check file exists
    if (0xFFFFFFFF == GetFileAttributes(wPath))
	{
        return INPUT_MISSING;
	}

	// Read Settings File
    HANDLE hFile = CreateFile(wPath, GENERIC_READ, 0, 0, OPEN_ALWAYS, 0, 0);
	if(INVALID_HANDLE_VALUE == hFile)
	{
		return INPUT_BAD;
	}

	BY_HANDLE_FILE_INFORMATION lpFileInformation;
	GetFileInformationByHandle(hFile, &lpFileInformation);
	g_KeyTranslationEnd = NULL;

	if(lpFileInformation.nFileSizeLow)
	{
		// TODO: read only the non-header
		g_KeyTranslationHead = (RemapEntry*)malloc(lpFileInformation.nFileSizeLow);
		DWORD dwBytesRead = 0;
		unsigned int headerBuffer[2];
		// READ THE HEADER
		if (ReadFile(hFile, headerBuffer, sizeof(headerBuffer), &dwBytesRead, NULL))
		{
			if (dwBytesRead == sizeof(headerBuffer))
			{
				LogDebugMessage("Header read...");
			}
			else
			{
				LogDebugMessage("Header read failed...");
			}
		}

		const DWORD dwBytesRemaining = lpFileInformation.nFileSizeLow - sizeof(headerBuffer);

		// read the entire file into memory
		// NOTE: This assumes the file is less than DWORD max in size(!)
		if(ReadFile(hFile, g_KeyTranslationHead, dwBytesRemaining, &dwBytesRead, NULL))
		{
			if(dwBytesRead == dwBytesRemaining) // verify everything was read in...
			{
				LogDebugMessage("Loaded file: %s [%d]", sFile, dwBytesRemaining);
				g_KeyTranslationEnd = (char*)g_KeyTranslationHead + dwBytesRemaining;
			}
		}
	} // 0 < file size
	CloseHandle(hFile);

	// validate a proper end was set
	if(!g_KeyTranslationEnd)
	{
		ShutdownCapture();
		return INPUT_BAD;
	}
	// validate file (and compile the "hash" table g_KeyTranslationTable)
	memset(g_KeyTranslationTable, 0, WIN_KEY_COUNT * sizeof(RemapEntryListItem*));

	// wipe the histories
	memset(g_MouseToggleHistory, FALSE, MOUSE_BUTTON_COUNT);
	memset(g_KeyToggleHistory, FALSE, MAX_VKEY);

	RemapEntry* pEntry = g_KeyTranslationHead;
	bool bValidTranslationSet = false;
		
	// The translation table contains linked lists of all the output sets for a given key due to the flag keys (shift/alt/ctrl)
	while(NULL != pEntry)
	{
		if(0 == pEntry->outputCount)
		{
			ShutdownCapture();
			return INPUT_BAD;
		}
		// TODO: just get a pointer to g_KeyTranslationTable[pKey->inputConfig.virtualKey] ?

		LogDebugMessage("Loading %s Outputs: %d", GetInputConfigDescription(pEntry->inputConfig), pEntry->outputCount);

		if(NULL == g_KeyTranslationTable[pEntry->inputConfig.virtualKey])
		{
			g_KeyTranslationTable[pEntry->inputConfig.virtualKey] = (RemapEntryListItem*)malloc(sizeof(RemapEntryListItem));
			g_KeyTranslationTable[pEntry->inputConfig.virtualKey]->pEntry = pEntry;
			g_KeyTranslationTable[pEntry->inputConfig.virtualKey]->pNext = NULL;
		}
		else
		{
			RemapEntryListItem* pKeyItem = g_KeyTranslationTable[pEntry->inputConfig.virtualKey];
			while(NULL != pKeyItem->pNext)
			{
				pKeyItem = pKeyItem->pNext;
			}
			pKeyItem->pNext = (RemapEntryListItem*)malloc(sizeof(RemapEntryListItem));
			pKeyItem = pKeyItem->pNext;
			pKeyItem->pEntry = pEntry;
			pKeyItem->pNext = NULL;
		}
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
		HHOOK g_hookMain = SetWindowsHookEx( WH_KEYBOARD_LL, LowLevelKeyboardProc, hInstance, NULL);
		if(NULL == g_hookMain)
		{
			ShutdownCapture();
			return HOOK_CREATION_FAILURE;
		}
		else
		{
			return HOOK_CREATION_SUCCESS;
		}
	}
	else
	{
		ShutdownCapture();
		return INPUT_BAD;
	}
}

/*
Shuts down the key capture hook and frees any allocated memory
*/
__declspec(dllexport) void ShutdownCapture()
{
	// disable the hook
	if(NULL != g_hookMain)
	{
		LogDebugMessage("Unhooked\n");
		UnhookWindowsHookEx(g_hookMain);
		g_hookMain = NULL;
	}

	// memory clean up
	if(NULL != g_KeyTranslationHead)
	{
		LogDebugMessage("Cleared Memory!\n");
		free(g_KeyTranslationHead);
		g_KeyTranslationHead = NULL;
	}

	// clean up "hash" table
	for(int nIdx = 0; nIdx < WIN_KEY_COUNT; nIdx++)
	{
		if(NULL != g_KeyTranslationTable[nIdx])
		{
			RemapEntryListItem* pKeyItem = g_KeyTranslationTable[nIdx];
			RemapEntryListItem* pKeyNextItem = NULL;
			while(NULL != pKeyItem)
			{
				pKeyNextItem = pKeyItem->pNext;
				free(pKeyItem);
				pKeyItem = pKeyNextItem;
			}
			g_KeyTranslationTable[nIdx] = NULL;
		}
	}

	g_KeyTranslationEnd = NULL; 

	LogDebugMessage("Capture Shutdown\n");
}

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
	
	//bool bAlt = 0 != (pHook->flags & LLKHF_ALTDOWN);
	bool bAlt = 0 != (GetAsyncKeyState(VK_MENU) & 0x8000);
	bool bControl = 0 != (GetAsyncKeyState(VK_CONTROL) & 0x8000);
	bool bShift = 0 != (GetAsyncKeyState(VK_SHIFT) & 0x8000);

	// don't catch injected keys
	if(!(pHook->flags & LLKHF_INJECTED))
	{
		// NOTE: these are in while loops because a key may have multiple mappings due to the flag keys (shift/alt/ctrl)
		if (HC_ACTION == nCode)  
		{  
			switch (wParam)  
			{
				case WM_KEYDOWN:
				case WM_SYSKEYDOWN:
					// keys being captured never send the keydown
					{
						RemapEntryListItem* pKeyListItem = g_KeyTranslationTable[pHook->vkCode];
						while(NULL != pKeyListItem)
						{
							InputConfig* pKeyDef = &pKeyListItem->pEntry->inputConfig;
							if((bAlt == pKeyDef->inputFlag.bAlt) &&
								(bControl == pKeyDef->inputFlag.bControl) &&
								(bShift == pKeyDef->inputFlag.bShift))
							{
								LogDebugMessage("Detected Key Press: %s Outputs: %d", GetInputConfigDescription(*pKeyDef), pKeyListItem->pEntry->outputCount);
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
					// keys being captured never send the keyup
					{
						RemapEntryListItem* pKeyListItem = g_KeyTranslationTable[pHook->vkCode];
						while(NULL != pKeyListItem)
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
		}
	}
	return bSentInput ? 1 : CallNextHookEx( NULL, nCode, wParam, lParam );  
}

/*
Thread responsible for sending the desired inputs to the OS (see standard win32 definition)
*/
DWORD WINAPI SendInputThread( LPVOID lpParam ) 
{

	/*
struct RemapEntry
{
	InputConfig inputConfig;
	BYTE outputCount;
	// KeyDefintion[outputCount]
};
	*/

	RemapEntry* pItem = static_cast<RemapEntry*>(lpParam);
	// get a pointer to the OutputConfig data that is AFTER the RemapEntry fields
	OutputConfig* pOutputConfig = (OutputConfig*)((BYTE*)pItem + sizeof(RemapEntry));
	// get a pointer to the first byte AFTER all the OutputConfig entries
	OutputConfig* pTerminator = (OutputConfig*)((BYTE*)pOutputConfig + (pItem->outputCount * sizeof(OutputConfig)));
	if(pOutputConfig->outputFlag.bDoNothing)
	{
		return 0; // done!
	}

	// trigger any necessary release of shift/control/alt by passing the trigger key definition
	SendTriggerEndInputKeys(&pItem->inputConfig);

	// iterate over the target inputs
	while(pOutputConfig < pTerminator)
	{
		LogDebugMessage("Performing Output Action: %s", GetOutputConfigDescription(*pOutputConfig));
		// mouse input
		if (pOutputConfig->outputFlag.bMouseOut)
		{
			SendInputMouse(pOutputConfig);
		}
		// just a delay
		else if (pOutputConfig->outputFlag.bDelay)
		{
			Sleep(1000 * pOutputConfig->parameter);
		}
		// keyboard input
		else
		{
			SendInputKeys(pOutputConfig);
		}
		pOutputConfig++; // move the pointer forward one KeyDefinition
	}

	LogDebugMessage("SendInputThread: DEAD");
	return 0;
}

char* GetBoolString(BYTE nValue)
{
	return nValue == 0 ? "False" : "True";
}

// NOT THREAD SAFE
char* GetInputConfigDescription(InputConfig inputConfig)
{
	static char buffer[256];
	sprintf_s(buffer, 256, "InputConfig [Key: %d 0x%02x] [Alt: %s] [Ctrl: %s] [Shift: %s]",
		inputConfig.virtualKey,
		inputConfig.virtualKey,
		GetBoolString(inputConfig.inputFlag.bAlt),
		GetBoolString(inputConfig.inputFlag.bControl),
		GetBoolString(inputConfig.inputFlag.bShift));
	return buffer;
}

char* GetOutputConfigDescription(OutputConfig outputConfig)
{
	static char buffer[256];
	sprintf_s(buffer, 256, "OutputConfig [Key: %d 0x%02x] [Alt: %s] [Ctrl: %s] [Shift: %s]",
		outputConfig.virtualKey,
		outputConfig.virtualKey,
		GetBoolString(outputConfig.outputFlag.bAlt),
		GetBoolString(outputConfig.outputFlag.bControl),
		GetBoolString(outputConfig.outputFlag.bShift));
	return buffer;
}

