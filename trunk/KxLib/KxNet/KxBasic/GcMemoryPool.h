
#ifndef __GcMemoryPool_H__
#define __GcMemoryPool_H__

#include <vector>
using std::vector;

class GcMemoryProfile;

enum Generation
{
	Generation_Zero = 0,
	Generation_One = 1,
	Generation_Two = 2,
};


class GcMemoryPool
{
public:
	GcMemoryPool();

public:
	inline Generation GetGeneration()
	{
		return m_eGeneration;
	}

	void UpdateGeneration();
	void AttachProfile(GcMemoryProfile* pGCMemoryProfile);


	void* Alloc(size_t size);
	

private:
	Generation m_eGeneration;

private:
	//////////////////////////////////////////////////////////////////////////
	// (0, 4], (4, 8], ...(s, e]
	vector<int>			m_vecRanges;
	
	GcMemoryProfile*	m_pGCMemoryProfile;

};

#endif