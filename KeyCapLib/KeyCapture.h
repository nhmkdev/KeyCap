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

#ifndef KEY_CAPTURE_H_     // equivalently, #if !defined HEADER_H_
#define KEY_CAPTURE_H_

#include "KeyCaptureUtil.h"

// === structs
struct InputFlag
{
#if 0
	const int KEYCAP_SHIFT = 1 << 0;
	const int KEYCAP_CONTROL = 1 << 1;
	const int KEYCAP_ALT = 1 << 2;
#endif
	BYTE bShift : 1;
	BYTE bControl : 1;
	BYTE bAlt : 1;
	BYTE padOne : 5;
	BYTE padTwo : 8;
	BYTE padThree : 8;
	BYTE padFour : 8;
};

struct OutputFlag
{
#if 0
	const int KEYCAP_SHIFT = 1 << 0;
	const int KEYCAP_CONTROL = 1 << 1;
	const int KEYCAP_ALT = 1 << 2;
	const int KEYCAP_DONOTHING = 1 << 3;
	const int KEYCAP_MOUSEOUT = 1 << 4;
	const int KEYCAP_DELAY = 1 << 5;
	const int KEYCAP_TOGGLE = 1 << 6;
#endif
	BYTE bShift : 1;
	BYTE bControl : 1;
	BYTE bAlt : 1;
	BYTE bDoNothing : 1;
	BYTE bMouseOut : 1;
	BYTE bDelay : 1;
	BYTE bToggle : 1;
	BYTE padOne : 1;
	BYTE padTwo : 8;
	BYTE padThree : 8;
	BYTE padFour : 8;
};

struct InputConfig
{
	InputFlag inputFlag;
	BYTE virtualKey;
	BYTE padOne : 8;
	BYTE padTwo : 8;
	BYTE padThree : 8;
	unsigned int parameter;
};

struct OutputConfig
{
	OutputFlag outputFlag;
	BYTE virtualKey; // also stores mouse output
	BYTE padOne : 8;
	BYTE padTwo : 8;
	BYTE padThree : 8;
	unsigned int parameter;
};

struct RemapEntry
{
	InputConfig inputConfig;
	BYTE outputCount;
	// KeyDefintion[outputCount]
};

struct RemapEntryListItem
{
	RemapEntry* pEntry;
	RemapEntryListItem* pNext;
};

// TODO: this is no longer applicable...
const int MAX_KEY_INPUT_PER_STROKE = 9; // control, alt (OR alt-up alt-down alt-up), shift, key, key-up, shift-up,  alt-up, control-up

// TODO: completely NOT thread safe
static INPUT g_inputBuffer[MAX_KEY_INPUT_PER_STROKE]; // keyboard output table (named input as it has the INPUT structs)

char* GetInputConfigDescription(InputConfig inputConfig);
char* GetOutputConfigDescription(OutputConfig outputConfig);

char* GetFlagsString(DWORD dwFlags);

#endif // KEY_CAPTURE_H_
