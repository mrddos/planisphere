
#include "stdafx.h"
#include <iKxLib\KxGc.h>

#include <new>

// Lib Export
__KxLib_GcModule* __KxLib_GcModule_Instance()
{
	return __KxLib_GcModule::Instance();
}





void* operator new(size_t size, __KxLib_GcModule* pKxGcModule, __KxLib_ThreadLocal* pThreadLocal)
{
	ATLASSERT(pKxGcModule && pThreadLocal);
	return pKxGcModule->Alloc(size, pThreadLocal->GetThreadHandle());
	return 0;
}



__KxLib_GcModule* __KxLib_GcModule::s_pKxLib_GcModule = NULL;

__KxLib_GcModule::__KxLib_GcModule(HMODULE hMod)
:m_hMod(hMod)
{
	s_pKxLib_GcModule = this;
	m_pGcAlloc = NULL;
}

__KxLib_GcModule::~__KxLib_GcModule()
{
	s_pKxLib_GcModule = NULL;
}


void* __KxLib_GcModule::Alloc(size_t size, HANDLE hThread)
{
	ATLASSERT(NULL != m_pGcAlloc);
	void* p = (*m_pGcAlloc)(size, hThread);
	return 0;
}


__KxLib_GcModule* __KxLib_GcModule::Instance()
{
	ATLASSERT(NULL != s_pKxLib_GcModule);
	return s_pKxLib_GcModule;
}

BOOL __KxLib_GcModule::Initialize(HMODULE hMod)
{
	__KxLib_GcModule* pKxLib_GcModule = new (std::nothrow) __KxLib_GcModule(hMod);
	if (pKxLib_GcModule)
	{
		pKxLib_GcModule->__KxLib_GcModule_Initialize();
	}
	return (NULL != pKxLib_GcModule);
}

void __KxLib_GcModule::__KxLib_GcModule_Initialize()
{
	m_pGcAlloc = (KX_GC_ALLOC)GetProcAddress(m_hMod, "Kx_Gc_Alloc");
}

