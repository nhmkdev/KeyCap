////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2022 Tim Stair
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
#pragma once
#ifndef KEY_CAPTURE_UTIL_H_     // equivalently, #if !defined HEADER_H_
#define KEY_CAPTURE_UTIL_H_

#include "stdafx.h"
#include "keycapturestructs.h"

const int DESCRIPTION_BUFFER_SIZE = 256;

void ResetRemapEntryState(RemapEntryState* pRemapEntryState, BYTE bToggled);
bool IsButtonDownRequired(RemapEntryState* pRemapEntryState, OutputConfig* pKeyDef);
bool IsButtonUpRequired(RemapEntryState* pRemapEntryState, OutputConfig* pKeyDef);

void LogDebugMessage(const char *format, ...);
void ValidateStructs();

char* GetBoolString(BYTE nValue);
char* GetInputConfigDescription(InputConfig inputConfig);
char* GetOutputConfigDescription(OutputConfig outputConfig);
#endif
