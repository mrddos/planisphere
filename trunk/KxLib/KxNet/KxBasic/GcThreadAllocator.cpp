
#include "stdafx.h"
#include "GcModule.h"


#include "GcThreadAllocator.h"
#include "GcMemoryProfile.h"



GcThreadAllocator::GcThreadAllocator()
:m_dwCount(0), m_eCurrentGeneration(Generation_Zero)
{
	m_pCurrentMemoryPool = new GcMemoryPool(Generation_Zero);
}


void* GcThreadAllocator::Alloc(size_t size)
{
	ATLASSERT(NULL != m_pCurrentMemoryPool);
	if (NULL != m_pCurrentMemoryPool)
	{
		m_dwCount++;
		return m_pCurrentMemoryPool->Alloc(size);
	}
	return 0;
}

void GcThreadAllocator::Gc()
{
	if (Generation_Zero == m_eCurrentGeneration)
	{
		m_pCurrentMemoryPool = new GcMemoryPool(m_eCurrentGeneration = Generation_One);
		return;
	}
	else if (Generation_One == m_eCurrentGeneration)
	{
		m_pCurrentMemoryPool = new GcMemoryPool(m_eCurrentGeneration = Generation_Two);
		return;
	}
}

DWORD GcThreadAllocator::GetAllocCount() const
{
	return m_dwCount;
}