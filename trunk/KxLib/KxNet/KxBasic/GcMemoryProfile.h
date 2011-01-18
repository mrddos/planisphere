
#ifndef __GcMemoryProfle_H__
#define __GcMemoryProfle_H__

#include <vector>
using std::vector;

class GcMemoryProfile
{
public:

	void Initialize()
	{
		m_vecSection.reserve(100);
	}
public:

	class _Section
	{
	public:
		_Section()
			:m_dwBlockSize(0), m_dwBlockCount(0)
		{

		}

		_Section(DWORD dwBlockSize, DWORD dwBlockCount)
			:m_dwBlockSize(dwBlockSize), m_dwBlockCount(dwBlockCount)
		{

		}

		_Section(_Section const& other)
		{

		}


	private:
		DWORD m_dwBlockSize;
		DWORD m_dwBlockCount;

	};

private:
	vector<_Section> m_vecSection;

};

#endif