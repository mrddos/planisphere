
#include "stdafx.h"
#include <iKxLib\KxLib.h>
#include <iKxLib\KxGc.h>

#include <shlwapi.h>


BOOL KxLib::Initialize()
{
	WCHAR szModulePath[MAX_PATH] = {};
	GetModuleFileName(NULL, szModulePath, MAX_PATH);
	PathAppend(szModulePath, L"..\\KxBasic.dll");

	HMODULE hMod = LoadLibrary(szModulePath);

	BOOL bGcModuleInit = __KxLib_GcModule::Initialize(hMod);
	BOOL bThreadModuleInit = __KxLib_ThreadModule::Initialize(hMod);

	return bGcModuleInit && bThreadModuleInit;
}

BOOL KxLib_Initialize()
{
	return KxLib::Initialize();
}