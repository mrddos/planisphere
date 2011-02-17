
#ifndef __GcModule_H__
#define __GcModule_H__

#pragma once

#include <map>
using std::map;


class GcThreadAllocator;

class GcModule
{
public:
	LRESULT Initialize();
	void* Alloc(size_t, HANDLE);
	void Gc();


	GcThreadAllocator* GetGcThreadAllocator(HANDLE hThread);

	static GcModule* GetGcModule();
protected:
	GcModule();

private:
	static GcModule* s_pGcModule;

private:
	map<HANDLE, GcThreadAllocator*> m_mapGcThreadAllocator;

	map<HANDLE, DWORD>				m_mapHandleFrequency;

	DWORD						m_dwAllocMaxCount;
	DWORD						m_dwAllocSecCount;
	// cache
	BOOL						m_bCacheWithFrequency;
	UINT						m_nUpdateCacheCounter;
	struct _CacheItem
	{
		HANDLE					hThread;
		GcThreadAllocator*		pThreadAlloc;
	};

	typedef _CacheItem _CacheType[3];

	_CacheType _cache;

};





#endif