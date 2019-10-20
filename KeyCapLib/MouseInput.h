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

#ifndef MOUSE_INPUT_H_     // equivalently, #if !defined HEADER_H_
#define MOUSE_INPUT_H_

#include "stdafx.h"
#include "KeyCapture.h"

enum MOUSE_BUTTON
{
	MOUSE_NONE = 0x00,
	MOUSE_LEFT = 0x01,
	MOUSE_RIGHT = 0x02,
	MOUSE_MIDDLE = 0x03,
	MOUSE_BUTTON_COUNT
};

static bool g_MouseToggleHistory[MOUSE_BUTTON_COUNT];

extern unsigned char g_MouseDownMap[];
extern unsigned char g_MouseUpMap[];

void SendInputMouse(KeyDefinition *pKeyDef);
void AppendSingleMouse(INPUT* inputChar, unsigned char nVkKey);

#endif // MOUSE_INPUT_H_
