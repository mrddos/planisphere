// OAuthCpp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "oauth.h"

#include "Base64.h"


void Test_Base64()
{
	Base64 b64;

	BYTE data[32] = {};
	CopyMemory((BYTE*)data + 2, (BYTE*)L"ÄúºÃhello World!", 28);
	LPWSTR buffer[128] = {};
	DWORD dwLen = b64.Encode((BYTE*)L"ÄúºÃhello World!", 32, (LPWSTR)buffer, 128);
	buffer[dwLen] = 0;

	b64.Decode(buffer, )
}


int _tmain(int argc, _TCHAR* argv[])
{
	Test_Base64();
	return 0;
}

