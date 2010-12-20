
#include "stdafx.h"
#include "KxGc.h"

void* operator new(size_t size, KxGcModule* pKxGcModule)
{
	ATLASSERT(pKxGcModule);
	return pKxGcModule->Alloc(size);
	return 0;
}

KxGcModule::KxGcModule()
:m_hMod(NULL)
{
	m_pGcInitialize = NULL;
}

KxGcModule::~KxGcModule()
{

}


void* KxGcModule::Alloc(size_t size)
{
	return 0;
}


KxGcModule* KxGcModule::Instance()
{
	return &g_KxGcModule;
}

LRESULT KxGcModule::Initialize()
{
	ATLASSERT(NULL == m_hMod);

	m_hMod = LoadLibrary(L"KxBasic.dll");
	if (NULL != m_hMod)
	{
		m_pGcInitialize = (KX_GC_INITIALIZE)GetProcAddress(m_hMod, "Kx_Gc_Initialize");
		if (NULL != m_pGcInitialize)
		{

			return (*m_pGcInitialize)();
		}
	}
	return E_FAIL;
}