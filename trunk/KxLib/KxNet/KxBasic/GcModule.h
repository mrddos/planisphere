
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
};





#endif