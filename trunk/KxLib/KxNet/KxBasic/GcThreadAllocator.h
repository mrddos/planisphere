
#ifndef __GcThreadAllocator_H__
#define __GcThreadAllocator_H__

#include "GcMemoryPool.h"


class GcMemoryPool;
class GcMemoryProfile;

class GcThreadAllocator
{
public:
	GcThreadAllocator();


public:
	void* Alloc(size_t size);

	void Gc();

	DWORD GetAllocCount() const;
private:
	
private:
	
private:
	DWORD				m_dwCount;
	GcMemoryProfile*	m_pGCMemoryProfile;
	GcMemoryPool*		m_pCurrentMemoryPool;	// Current Memory Pool
	Generation			m_eCurrentGeneration;
};


#endif