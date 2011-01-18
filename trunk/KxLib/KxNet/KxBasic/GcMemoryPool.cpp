
#include "stdafx.h"
#include "GcMemoryPool.h"

GcMemoryPool::GcMemoryPool()
:m_eGeneration(Generation_Zero)
{
	
}

void GcMemoryPool::UpdateGeneration()
{
	if (Generation_Zero == m_eGeneration)
	{
		m_eGeneration = Generation_One;
	}
	else if (Generation_One == m_eGeneration)
	{
		m_eGeneration = Generation_Two;
	}
}

void* GcMemoryPool::Alloc(size_t size)
{

	return 0;
}

void GcMemoryPool::AttachProfile( GcMemoryProfile* pGCMemoryProfile )
{

}