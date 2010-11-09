
#ifndef __CPATHEX_H__
#define __CPATHEX_H__

#pragma once

#include <atlpath.h>


#include <shlobj.h>

class CPathEx
{
public:
	static BOOL Create(LPCWSTR path)
	{

		return TRUE;
	}

public:
	static CPath GetWindowsDirectory()
	{
		WCHAR szPath[MAX_PATH] = {};
		::GetWindowsDirectory(szPath, MAX_PATH);
		CPath path(szPath);
		return path;
	}

	static CPath GetSpecialPath(int csidl)
	{
		WCHAR szPath[MAX_PATH] = {};
		SHGetSpecialFolderPath(NULL, szPath, csidl, FALSE);
		return CPath(szPath);
	}

	static BOOL RemoveFiles(LPCWSTR path, BOOL bRecursion = FALSE)
	{
		
		return TRUE;
	}
};


#endif