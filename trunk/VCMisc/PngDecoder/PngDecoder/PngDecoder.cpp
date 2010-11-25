
#include "stdafx.h"
#include "LibPng.h"


extern "C" __declspec(dllexport) HBITMAP DecodePNG (PBYTE pngdata, DWORD pnglen)
{
	return Decode_PNG (pngdata, pnglen);
}
