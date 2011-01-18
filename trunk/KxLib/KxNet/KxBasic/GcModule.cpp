
#include "stdafx.h"
#include "GcModule.h"
#include "GcThreadAllocator.h"

GcModule* GcModule::s_pGcModule = NULL;

LRESULT GcModule::Initialize()
{
	
	return S_OK;
}

GcModule* GcModule::GetGcModule()
{
	if (s_pGcModule)
		return s_pGcModule;
	s_pGcModule = new (std::nothrow) GcModule();
	return s_pGcModule;
}

GcModule::GcModule()
{
	
}

void* GcModule::Alloc(size_t size, HANDLE hThread)
{
	GcThreadAllocator* pThreadAllocator = GetGcThreadAllocator(hThread);
	void* p = pThreadAllocator->Alloc(size);
	return p;
}

void GcModule::Gc()
{

}



GcThreadAllocator* GcModule::GetGcThreadAllocator(HANDLE hThread)
{
	map<HANDLE, GcThreadAllocator*>::const_iterator const_iter = m_mapGcThreadAllocator.find(hThread);
	if (m_mapGcThreadAllocator.end() != const_iter)
	{
		GcThreadAllocator* pThreadAllocator = (GcThreadAllocator*)const_iter->second;
		ATLASSERT(NULL != pThreadAllocator);
		return pThreadAllocator;
	}
	else
	{
		GcThreadAllocator* pThreadAllocator = new (std::nothrow) GcThreadAllocator();
		m_mapGcThreadAllocator[hThread] = pThreadAllocator;
		return pThreadAllocator;
	}
}