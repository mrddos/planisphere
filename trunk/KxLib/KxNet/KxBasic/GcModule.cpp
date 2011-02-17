
#include "stdafx.h"
#include "GcModule.h"
#include "GcThreadAllocator.h"

GcModule* GcModule::s_pGcModule = NULL;

LRESULT GcModule::Initialize()
{
	m_bCacheWithFrequency = FALSE;
	m_nUpdateCacheCounter = 0;

	m_dwAllocMaxCount = 0;
	m_dwAllocSecCount = 0;
	ZeroMemory(&_cache, sizeof(_CacheType));
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
	GcThreadAllocator* pThreadAllocator = NULL;
	if (_cache[0].hThread == hThread)
	{
		pThreadAllocator = _cache[0].pThreadAlloc;
	}
	else if (m_bCacheWithFrequency)
	{
		if (_cache[1].hThread == hThread)
		{
			pThreadAllocator = _cache[1].pThreadAlloc;
		}
		else if (_cache[2].hThread == hThread)
		{
			pThreadAllocator = _cache[2].pThreadAlloc;
		}
	}
	else
	{
		pThreadAllocator = GetGcThreadAllocator(hThread);
	}
	
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

		// Update Cache
		_cache[0].hThread = hThread;
		_cache[0].pThreadAlloc = pThreadAllocator;

		if (m_bCacheWithFrequency)
		{
			m_nUpdateCacheCounter = ++m_nUpdateCacheCounter % 10;
			if (0 == m_nUpdateCacheCounter)
			{
				const DWORD dwCount = pThreadAllocator->GetAllocCount();
				if (dwCount > m_dwAllocMaxCount)
				{
					m_dwAllocMaxCount = dwCount;

					if (NULL != _cache[1].hThread)
					{
						_cache[2].hThread		= _cache[1].hThread;
						_cache[2].pThreadAlloc	= _cache[1].pThreadAlloc;
					}
					_cache[1].hThread		= hThread;
					_cache[1].pThreadAlloc	= pThreadAllocator;
				}

			}
		}
		

		return pThreadAllocator;
	}
	else
	{
		GcThreadAllocator* pThreadAllocator = new (std::nothrow) GcThreadAllocator();
		m_mapGcThreadAllocator[hThread] = pThreadAllocator;

		// Update Cache
		_cache[0].hThread = hThread;
		_cache[0].pThreadAlloc = pThreadAllocator;
		return pThreadAllocator;
	}
}