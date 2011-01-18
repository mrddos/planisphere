
#ifndef __GcThreadAllocator_H__
#define __GcThreadAllocator_H__

class GcMemoryPool;
class GcMemoryProfile;

class GcThreadAllocator
{
public:
	GcThreadAllocator();


public:
	void* Alloc(size_t size);

private:
	
private:
	
private:
	GcMemoryProfile*	m_pGCMemoryProfile;
	GcMemoryPool*		m_pG0MempryPool;
	GcMemoryPool*		m_pG1MempryPool;
	GcMemoryPool*		m_pG2MempryPool;
};


#endif