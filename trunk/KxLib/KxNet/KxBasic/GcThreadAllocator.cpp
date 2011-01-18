
#include "stdafx.h"
#include "GcModule.h"


#include "GcThreadAllocator.h"
#include "GcMemoryPool.h"
#include "GcMemoryProfile.h"



GcThreadAllocator::GcThreadAllocator()
:m_pG0MempryPool(NULL), m_pG1MempryPool(NULL), m_pG2MempryPool(NULL)
{

}


void* GcThreadAllocator::Alloc(size_t size)
{
	ATLASSERT(NULL != m_pG0MempryPool);
	if (NULL != m_pG0MempryPool)
	{
		return m_pG0MempryPool->Alloc(size);
	}
	return 0;
}