
#include "stdafx.h"
#include <iKxLib\KxThread.h>


#include <new>

__KxLib_ThreadModule* __KxLib_ThreadModule::s_pKxLib_ThreadModule = NULL;

__KxLib_ThreadModule::__KxLib_ThreadModule(HMODULE hMod)
:m_hMod(hMod)
{
	m_pKxCreateThread = NULL;
}

BOOL __KxLib_ThreadModule::Initialize(HMODULE hMod)
{
	s_pKxLib_ThreadModule = new (std::nothrow) __KxLib_ThreadModule(hMod);
	if (NULL != s_pKxLib_ThreadModule)
	{
		s_pKxLib_ThreadModule->__KxLib_ThreadModule_Initialize();
	}
	return TRUE;
}

void __KxLib_ThreadModule::__KxLib_ThreadModule_Initialize()
{
	m_pKxCreateThread = (KX_CREATE_THREAD)GetProcAddress(m_hMod, "Kx_Create_Thread");
}

__KxLib_ThreadModule* __KxLib_ThreadModule::Instance()
{
	return s_pKxLib_ThreadModule;
}

HANDLE __KxLib_ThreadModule::Create(LPCWSTR lpName, IKxRunnable* pRunnable)
{
	HANDLE hThread = (*m_pKxCreateThread)(lpName, NULL, 0, pRunnable, 0);
	return hThread;
}


