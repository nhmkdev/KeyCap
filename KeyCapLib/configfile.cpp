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

#include "configfile.h"
#include "keycaputil.h"

const int FILE_DATA_PREFIX = (int)0x0E0CA000;
const int DATA_FORMAT_VERSION = (int)0x1;

int LoadFile(char* sFile, RemapEntry** ppKeyTranslationHead, void** ppKeyTranslationEnd) // TODO: pass in pointers to fill in
{
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
	if (INVALID_HANDLE_VALUE == hFile)
	{
		return INPUT_BAD;
	}

	BY_HANDLE_FILE_INFORMATION lpFileInformation;
	GetFileInformationByHandle(hFile, &lpFileInformation);

	if (lpFileInformation.nFileSizeLow)
	{
		DWORD dwBytesRead = 0;
		unsigned int headerBuffer[2];
		// READ THE HEADER
		if (ReadFile(hFile, headerBuffer, sizeof(headerBuffer), &dwBytesRead, nullptr))
		{
			if (dwBytesRead == sizeof(headerBuffer))
			{
				LogDebugMessage("Header read...");
				if (headerBuffer[0] != FILE_DATA_PREFIX || headerBuffer[1] != DATA_FORMAT_VERSION)
				{
					// TODO: add new enum?
					return INPUT_BAD;
				}
			}
			else
			{
				LogDebugMessage("Header read failed...");
				return INPUT_BAD;
			}
		}

		const DWORD dwBytesRemaining = lpFileInformation.nFileSizeLow - sizeof(headerBuffer);
		*ppKeyTranslationHead = (RemapEntry*)malloc(dwBytesRemaining);
		memset(*ppKeyTranslationHead, 0, dwBytesRemaining);

		// read the entire file into memory
		// NOTE: This assumes the file is less than DWORD max in size(!)
		if (ReadFile(hFile, *ppKeyTranslationHead, dwBytesRemaining, &dwBytesRead, nullptr))
		{
			if (dwBytesRead == dwBytesRemaining) // verify everything was read in...
			{
				LogDebugMessage("Loaded file: %s [%d]", sFile, dwBytesRemaining);
				*ppKeyTranslationEnd = (BYTE*)*ppKeyTranslationHead + dwBytesRemaining;
			}
		}
	} // 0 < file size
	CloseHandle(hFile);

	// validate a proper end was set
	if (!*ppKeyTranslationEnd)
	{
		return INPUT_BAD;
	}
	return INPUT_VALID;
}