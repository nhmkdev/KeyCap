////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2016 Tim Stair
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

// KeyCapture.cpp : Defines the entirety of the key capture... (probably should be broken up a bit)
//

#include "stdafx.h"
#include <stdlib.h>
#include <stdio.h>
#include <assert.h>

// === consts and defines

const int MAX_KEY_INPUT_PER_STROKE = 9; // control, alt (OR alt-up alt-down alt-up), shift, key, key-up, shift-up,  alt-up, control-up

const int KEYCAP_SHIFT		= 1 << 0;
const int KEYCAP_CONTROL	= 1 << 1;
const int KEYCAP_ALT		= 1 << 2;
const int KEYCAP_DONOTHING	= 1 << 3;
const int KEYCAP_MOUSEOUT	= 1 << 4;
const int KEYCAP_DELAY		= 1 << 5;
const int KEYCAP_KEYDOWN	= 1 << 6;
const int KEYCAP_KEYUP		= 1 << 7; // maxed out, need more than a byte if more are needed

const int HASH_TABLE_SIZE	= 256;

// === enums

enum HOOK_RESULT
{
	HOOK_CREATION_SUCCESS = 0,
	HOOK_CREATION_FAILURE,
	INPUT_MISSING,
	INPUT_ZERO,
	INPUT_BAD
};

// === structs
struct KeyDefinition
{
	unsigned char bShift		: 1;
	unsigned char bControl		: 1;
	unsigned char bAlt			: 1;
	unsigned char bDoNothing	: 1;
	unsigned char bMouseOut		: 1;
	unsigned char bDelay		: 1;
	unsigned char bKeyDown		: 1;
	unsigned char bKeyUp		: 1;
	//unsigned char pad			: 2;
	unsigned char nVkKey		: 8;
};

struct KeyTranslation
{
	KeyDefinition kDef;

	char nKDefOutput; // count of output key definitions
	// KeyDefintion[nKDefOutput]
};

struct KeyTranslationListItem
{
	KeyTranslation* pTrans;
	KeyTranslationListItem* pNext;
};

// === prototypes
// avoid mangling the function names (necessary for export and usage in C#)
extern "C"
{
	__declspec(dllexport) int LoadAndCaptureFromFile(HINSTANCE hInstance, char* sFile);
	__declspec(dllexport) void ShutdownCapture();
}
LRESULT CALLBACK LowLevelKeyboardProc(int nCode,WPARAM wParam,LPARAM lParam);
DWORD WINAPI SendInputThread( LPVOID lpParam );
void SendInputMouse(KeyDefinition *pKeyDef);
void SendTriggerEndInputKeys(KeyDefinition* pTriggerDefinition);
void SendInputKeys(KeyDefinition* pKeyDef);
void AppendSingleKey(short keyScan, INPUT* inputChar, DWORD dwFlags); 
bool CheckKeyFlags(char nFlags, bool bAlt, bool bControl, bool bShift);

// === sweet globals
KeyTranslationListItem* g_KeyTranslationTable[HASH_TABLE_SIZE];
INPUT g_keyboardInput[MAX_KEY_INPUT_PER_STROKE]; // keyboard output table (named input as it has the INPUT structs)
KeyTranslation* g_KeyTranslationHead = NULL;
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
	assert(2 == sizeof(KeyDefinition)); // if this is invalid the configuration tool and kfg files will not be valid

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
		g_KeyTranslationHead = (KeyTranslation*)malloc(lpFileInformation.nFileSizeLow);
		DWORD dwBytesRead = 0;
		// read the entire file into memory
		// NOTE: This assumes the file is less than DWORD max in size(!)
		if(ReadFile(hFile, g_KeyTranslationHead, lpFileInformation.nFileSizeLow, &dwBytesRead, NULL)) 
		{
			if(dwBytesRead == lpFileInformation.nFileSizeLow) // verify everything was read in...
			{
				g_KeyTranslationEnd = (char*)g_KeyTranslationHead + lpFileInformation.nFileSizeLow;
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
	memset(g_KeyTranslationTable, 0, HASH_TABLE_SIZE * sizeof(KeyTranslationListItem*));
	KeyTranslation* pKey = g_KeyTranslationHead;
	bool bValidTranslationSet = false;
		
	while(NULL != pKey)
	{
		if(0 == pKey->nKDefOutput)
		{
			ShutdownCapture();
			return INPUT_BAD;
		}
			
		if(NULL == g_KeyTranslationTable[pKey->kDef.nVkKey])
		{
			g_KeyTranslationTable[pKey->kDef.nVkKey] = (KeyTranslationListItem*)malloc(sizeof(KeyTranslationListItem));
			g_KeyTranslationTable[pKey->kDef.nVkKey]->pTrans = pKey;
			g_KeyTranslationTable[pKey->kDef.nVkKey]->pNext = NULL;
		}
		else
		{
			KeyTranslationListItem* pKeyItem = g_KeyTranslationTable[pKey->kDef.nVkKey];
			while(NULL != pKeyItem->pNext)
			{
				pKeyItem = pKeyItem->pNext;
			}
			pKeyItem->pNext = (KeyTranslationListItem*)malloc(sizeof(KeyTranslationListItem));
			pKeyItem = pKeyItem->pNext;
			pKeyItem->pTrans = pKey;
			pKeyItem->pNext = NULL;
		}
		pKey = (KeyTranslation*)(&pKey->nKDefOutput + (1 + (pKey->nKDefOutput * sizeof(KeyDefinition))));
		if(pKey > g_KeyTranslationEnd)
		{
			ShutdownCapture();
			return INPUT_BAD;
		}
		else if(pKey == g_KeyTranslationEnd)
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
		printf("Unhooked\n");
		UnhookWindowsHookEx(g_hookMain);
		g_hookMain = NULL;
	}

	// memory clean up
	if(NULL != g_KeyTranslationHead)
	{
		printf("Cleared Memory!\n");
		free(g_KeyTranslationHead);
		g_KeyTranslationHead = NULL;
	}

	// clean up "hash" table
	for(int nIdx = 0; nIdx < HASH_TABLE_SIZE; nIdx++)
	{
		if(NULL != g_KeyTranslationTable[nIdx])
		{
			KeyTranslationListItem* pKeyItem = g_KeyTranslationTable[nIdx];
			KeyTranslationListItem* pKeyNextItem = NULL;
			while(NULL != pKeyItem)
			{
				pKeyNextItem = pKeyItem->pNext;
				free(pKeyItem);
				pKeyItem = pKeyNextItem;
			}
			// NULL entries out
			g_KeyTranslationTable[nIdx] = NULL;
		}
	}

	g_KeyTranslationEnd = NULL; 

	printf("Capture Shutdown\n");
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
		if (HC_ACTION == nCode)  
		{  
			switch (wParam)  
			{
				case WM_KEYDOWN:
				case WM_SYSKEYDOWN:
					// keys being captured never send the keydown
					{
						KeyTranslationListItem* pKeyListItem = g_KeyTranslationTable[pHook->vkCode];
						while(NULL != pKeyListItem)
						{
							KeyDefinition* pKeyDef = &pKeyListItem->pTrans->kDef;
							if((bAlt == pKeyDef->bAlt) &&
								(bControl == pKeyDef->bControl) &&
								(bShift == pKeyDef->bShift))
							{
								CreateThread(NULL, 0, SendInputThread, pKeyListItem->pTrans, 0, NULL);
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
						KeyTranslationListItem* pKeyListItem = g_KeyTranslationTable[pHook->vkCode];
						while(NULL != pKeyListItem)
						{
							KeyDefinition* pKeyDef = &pKeyListItem->pTrans->kDef;
							if((bAlt == pKeyDef->bAlt) &&
								(bControl == pKeyDef->bControl) &&
								(bShift == pKeyDef->bShift))
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
	KeyTranslation* pItem = static_cast<KeyTranslation*>(lpParam);
	KeyDefinition* pKeyDef = (KeyDefinition*)((char*)pItem + sizeof(KeyTranslation));
	KeyDefinition* pEndDef = (KeyDefinition*)(&pItem->nKDefOutput + (1 + (pItem->nKDefOutput * sizeof(KeyDefinition))));
	if(pKeyDef->bDoNothing)
	{
		return 0; // done!
	}

	// trigger any necessary release of shift/control/alt by passing the trigger key definition
	SendTriggerEndInputKeys(&pItem->kDef);

	// iterate over the target inputs
	while(pKeyDef < pEndDef)
	{
		// mouse input
		if (pKeyDef->bMouseOut)
		{
			SendInputMouse(pKeyDef);
		}
		// just a delay
		else if (pKeyDef->bDelay)
		{
			Sleep(1000 * pKeyDef->nVkKey);
		}
		// keyboard input
		else
		{
			SendInputKeys(pKeyDef);
		}
		pKeyDef++; // move the pointer forward one KeyDefinition
	}

#if 0
	if(pKeyDef->bMouseOut)
	{
		// TODO: Support ctrl/alt/shift flags
		while(pKeyDef < pEndDef)
		{
			SendInputMouse(pKeyDef);
			pKeyDef++; // move the pointer forward one KeyDefinition
		}
	}
	else
	{
		// trigger any necessary release of shift/control/alt by passing the trigger key definition
		SendInputKey(pKeyDef, &pItem->kDef);
		pKeyDef++; // move the pointer forward one KeyDefinition
		while(pKeyDef < pEndDef)
		{
			SendInputKey(pKeyDef, NULL);
			pKeyDef++; // move the pointer forward one KeyDefinition
		}
	}
#endif
#ifdef _DEBUG
	OutputDebugStringA("SendInputThread: DEAD");
#endif
	return 0;
}

/*
Sends the desired mouse input

pKeyDef: pointer to a key definition for a mouse input
*/
void SendInputMouse(KeyDefinition *pKeyDef)
{
	memset(&g_keyboardInput, 0, sizeof(INPUT)); // only clear the one that will be used
	g_keyboardInput[0].type = INPUT_MOUSE;
	g_keyboardInput[0].mi.dwExtraInfo = 0;
	g_keyboardInput[0].mi.mouseData = 0;
	g_keyboardInput[0].mi.dwExtraInfo = 0;
	g_keyboardInput[0].mi.dwFlags = pKeyDef->nVkKey;
#ifdef _DEBUG
	char outputchar[256];
	sprintf_s(outputchar, "SendingMouse: (flags)%x %d\n", g_keyboardInput[0].ki.dwFlags, g_keyboardInput[0].mi.dwFlags);
	OutputDebugStringA(outputchar);
#endif
	SendInput(1, g_keyboardInput, sizeof(INPUT));
}

void SendTriggerEndInputKeys(KeyDefinition* pTriggerDefinition)
{
	if(!pTriggerDefinition)
	{
		return;
	}
	int nIndex = 0;
	// max of 5 inputs may exist
	memset(&g_keyboardInput, 0, sizeof(INPUT) * 5);

	if (pTriggerDefinition->bShift)
	{
		// check the required input key to see if this was pressed and force a keyup to eliminate it being passed on
		// example: Shift + S >> f (without the keyup Shift + S >> F because shift is technically down)
		// problem: multiple key up messages (?) ... does it matter?
		// yes: slightly, Control + S >> x requires Control to be pressed again due to the keyup message (possible new flag?)
		AppendSingleKey(VK_SHIFT, &g_keyboardInput[nIndex++], KEYEVENTF_KEYUP);
	}

	if (pTriggerDefinition->bControl)
	{
		AppendSingleKey(VK_CONTROL, &g_keyboardInput[nIndex++], KEYEVENTF_KEYUP);
	}

	if (pTriggerDefinition->bAlt)
	{
		// Note: Application menus become active with ALT is pressed, to get out of the menu alt must be "pressed" a second time
		AppendSingleKey(VK_MENU, &g_keyboardInput[nIndex++], KEYEVENTF_KEYUP);
		AppendSingleKey(VK_MENU, &g_keyboardInput[nIndex++], 0);
		AppendSingleKey(VK_MENU, &g_keyboardInput[nIndex++], KEYEVENTF_KEYUP);
	}
#ifdef _DEBUG
	char outputchar[256];
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		sprintf_s(outputchar, "Sending: (flags)%x %d\n", g_keyboardInput[nTemp].ki.dwFlags, g_keyboardInput[nTemp].ki.wVk);
		OutputDebugStringA(outputchar);
	}
#endif
	SendInput(nIndex, g_keyboardInput, sizeof(INPUT));
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

	memset(&g_keyboardInput, 0, sizeof(INPUT) * MAX_KEY_INPUT_PER_STROKE);
	int nIndex = 0;
	if (pKeyDef->bKeyDown)
	{
		if (pKeyDef->bShift)
		{
			AppendSingleKey(VK_SHIFT, &g_keyboardInput[nIndex++], 0);
		}

		if (pKeyDef->bControl)
		{
			AppendSingleKey(VK_CONTROL, &g_keyboardInput[nIndex++], 0);
		}

		if (pKeyDef->bAlt)
		{
			AppendSingleKey(VK_MENU, &g_keyboardInput[nIndex++], 0);
		}

		// output the actual key
#ifdef _DEBUG
		sprintf_s(outputchar, "---- Appended (down) %d\n", pKeyDef->nVkKey);
		OutputDebugStringA(outputchar);
#endif
		AppendSingleKey(pKeyDef->nVkKey, &g_keyboardInput[nIndex++], 0);
	}

	if (pKeyDef->bKeyUp)
	{
#ifdef _DEBUG
		sprintf_s(outputchar, "---- Appended (up) %d\n", pKeyDef->nVkKey);
		OutputDebugStringA(outputchar);
#endif

		AppendSingleKey(pKeyDef->nVkKey, &g_keyboardInput[nIndex++], KEYEVENTF_KEYUP);

		// setup any necessary key event up messages
		if (pKeyDef->bShift)
		{
			AppendSingleKey(VK_SHIFT, &g_keyboardInput[nIndex++], KEYEVENTF_KEYUP);
		}
		if (pKeyDef->bControl)
		{
			AppendSingleKey(VK_CONTROL, &g_keyboardInput[nIndex++], KEYEVENTF_KEYUP);
		}
		if (pKeyDef->bAlt)
		{
			AppendSingleKey(VK_MENU, &g_keyboardInput[nIndex++], KEYEVENTF_KEYUP);
		}
	}
#ifdef _DEBUG
	for (int nTemp = 0; nTemp < nIndex; nTemp++)
	{
		sprintf_s(outputchar, "Sending: (flags)%x %d\n", g_keyboardInput[nTemp].ki.dwFlags, g_keyboardInput[nTemp].ki.wVk);
		OutputDebugStringA(outputchar);
	}
#endif
	SendInput(nIndex, g_keyboardInput, sizeof(INPUT));
}

/*
Assigns the desired keyboard input to send (see win32 INPUT documentation as it relates to SendInput)

keyScan: the virtual key value to set
inputChar: The scan value to set
dwFlags: The flags value to set
*/
void AppendSingleKey(short keyScan, INPUT* inputChar, DWORD dwFlags)
{
	inputChar->type = INPUT_KEYBOARD;
	inputChar->ki.wVk = LOBYTE(keyScan); 
	inputChar->ki.wScan = MapVirtualKey(LOBYTE(keyScan), 0); 
	inputChar->ki.dwFlags = dwFlags;
}
