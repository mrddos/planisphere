
#ifndef __CSTRINGEX_H__
#define __CSTRINGEX_H__

#pragma once

#include <atlstr.h>


typedef unsigned char __byte;

#ifndef _ATL_CSTRING_NO_CRT
typedef CStringT<__byte, StrTraitATL<__byte, ChTraitsCRT<__byte > > > RString;
#else  // _ATL_CSTRING_NO_CRT
typedef CStringT<__byte, StrTraitATL<__byte > > RString;
#endif  // _ATL_CSTRING_NO_CRT


class CStringEx
{
public:
	static BOOL NullOrEmpty(LPCWSTR str)
	{
		return (str == NULL) || (str[0] == (WCHAR)0);
	}
public:
	static CString Load(UINT nID)
	{
		CString strRet;
		strRet.LoadString(nID);
		return strRet;
	}

	static CString Lower(CString const& str)
	{
		CString strRet = str;
		strRet.MakeLower();
		return strRet;
	}

	static CString Upper(CString const& str)
	{
		CString strRet = str;
		strRet.MakeUpper();
		return strRet;
	}

	static CString Trim(CString const& str)
	{
		CString strRet = str;
		strRet.Trim();
		return strRet;
	}

	// Python style slice
	static CString Slice(CString const& str, int p1, int p2)
	{
		if (p1 >= 0) // from
		{

		}
		else // 
		{

		}
		return L"";
	}

	static CString _cdecl Format(LPCWSTR pszFormat, ...)
	{
		va_list argList;
		CString strRet;
		va_start(argList, pszFormat);
		strRet.FormatV(pszFormat, argList);
		va_end(argList);
		return strRet;
	}

	static BOOL Equal(CString const& str1, CString const& str2, BOOL bCaseIgnore = FALSE)
	{
		if (bCaseIgnore)
		{
			return str1.CompareNoCase(str2) == 0;
		}
		else
		{
			return str1.Compare(str2) == 0;
		}
	}


};

















#endif