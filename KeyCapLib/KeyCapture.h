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

#ifndef KEY_CAPTURE_H_     // equivalently, #if !defined HEADER_H_
#define KEY_CAPTURE_H_

// === structs
struct KeyDefinition
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
	unsigned char bShift : 1;
	unsigned char bControl : 1;
	unsigned char bAlt : 1;
	unsigned char bDoNothing : 1;
	unsigned char bMouseOut : 1;
	unsigned char bDelay : 1;
	unsigned char bToggle : 1;
	unsigned char pad : 1;
	unsigned char nVkKey : 8; // this stores mouse input too... TODO better name
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

// TODO: this is no longer applicable...
const int MAX_KEY_INPUT_PER_STROKE = 9; // control, alt (OR alt-up alt-down alt-up), shift, key, key-up, shift-up,  alt-up, control-up

// TODO: completely NOT thread safe
static INPUT g_inputBuffer[MAX_KEY_INPUT_PER_STROKE]; // keyboard output table (named input as it has the INPUT structs)

#endif // KEY_CAPTURE_H_
