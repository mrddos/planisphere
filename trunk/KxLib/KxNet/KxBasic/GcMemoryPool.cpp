
#include "stdafx.h"
#include "GcMemoryPool.h"

#include <algorithm>

GcMemoryPool::GcMemoryPool(Generation eGeneration)
:m_eGeneration(eGeneration)
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
	vector<int>::const_iterator iter = std::find(m_ranges.begin(), m_ranges.end(), size);
	if (m_ranges.end() != iter)
	{
		vector<int>::difference_type dist =  iter - m_ranges.begin();
		
	}
	
	
	return 0;
}

void GcMemoryPool::AttachProfile( GcMemoryProfile* pGCMemoryProfile )
{
	
}