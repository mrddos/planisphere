
#ifndef __BASE64_H__
#define __BASE64_H__

#include "Wincrypt.h"

#pragma comment(lib, "Crypt32.lib")

class Base64
{
public:
	DWORD Encode(const BYTE* pData, DWORD dwDataLen, LPWSTR base64str, DWORD dwBufferLen)
	{
		DWORD dwBase63StrLen = dwBufferLen;
		BOOL bCrypt = CryptBinaryToString(pData, dwDataLen, CRYPT_STRING_BASE64, base64str, &dwBase63StrLen);
		if (bCrypt)
		{
			return dwBase63StrLen;
		}
		return 0;
	}

	void Decode(LPCWSTR base64str, )
	{
		CryptStringToBinary(base64str, wcs)


	}

private:

};

#endif