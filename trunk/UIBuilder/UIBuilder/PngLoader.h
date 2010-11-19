
#ifndef __PNG_LOADER_H__
#define __PNG_LOADER_H__

typedef HBITMAP (*PNGDecode)(PBYTE pngdata, DWORD pnglen);

class PngLoader
{
public:
	static BOOL Initialize()
	{
		s_hModule = LoadLibrary(L"png_reader.dll");
		if (s_hModule != NULL)
		{
			s_fnPNGDecode = (PNGDecode)GetProcAddress(s_hModule, "DecodePNG");
			return s_fnPNGDecode != NULL;
		}
		return FALSE;
	}

	static HBITMAP Load(UINT nID)
	{
		ATLASSERT(s_fnPNGDecode);
		HRSRC hRes = ::FindResource ((HINSTANCE)&__ImageBase, MAKEINTRESOURCE(nID), _T("PNG")) ;
		if (hRes)
		{
			BYTE* pImgData = (BYTE*)::LockResource(::LoadResource ((HINSTANCE)&__ImageBase, hRes)) ;
			if (pImgData)
			{
				HBITMAP hBitmap = s_fnPNGDecode(pImgData, ::SizeofResource((HINSTANCE)&__ImageBase, hRes));
				return hBitmap;
			}
		}
		return NULL;
	}

	static HBITMAP Load(LPCWSTR fileName)
	{
		ATLASSERT(s_fnPNGDecode);
		HANDLE hImageFile = CreateFile(fileName, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_DELETE, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
		if (hImageFile != INVALID_HANDLE_VALUE)
		{
			DWORD dwSize2 = 0;
			DWORD dwSize = GetFileSize(hImageFile, &dwSize2);
			BYTE* pImgData = (BYTE*)malloc(dwSize);
			ReadFile(hImageFile, pImgData, dwSize, &dwSize2, NULL);
			CloseHandle(hImageFile);
			if (pImgData)
			{
				HBITMAP hBitmap = s_fnPNGDecode(pImgData, dwSize);
				return hBitmap;
			}
		}
		return NULL;
	}

	static BOOL Terminate()
	{
		return FreeLibrary(s_hModule);
	}
private:
	static HMODULE		s_hModule;
	static PNGDecode	s_fnPNGDecode;
};


__declspec(selectany) HMODULE PngLoader::s_hModule = NULL;
__declspec(selectany) PNGDecode	PngLoader::s_fnPNGDecode = NULL;

#endif