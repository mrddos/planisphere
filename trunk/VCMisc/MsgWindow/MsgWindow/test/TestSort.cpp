
#include "stdafx.h"
#include "TestSort.h"



#include <random>
#include <ctime>

#include <iostream>
using std::cout;

BOOL TestSort::testSortLargeIntArray()
{
	srand((unsigned int)time(0));
	const int size = 1024 * 1024;
	int* array = new int[size];

	for (int i = 0; i < size; ++i)
	{
		array[i] = rand() % 1024;
	}



	return TRUE;
}

BOOL TestSort::checkResult( int* array, int size )
{
	enum relation
	{
		unknown = 0,
		gt, lt,
	};

	int diff = 0;
	relation r = unknown;
	for (int i = 1; i < size; ++i)
	{
		diff = array[i] - array[i - 1];
		if (diff > 0)
		{
			if (unknown == r)
			{
				r = gt;
			}
			else if (lt == r)
			{
				return FALSE;
			}
		}
		else if (diff < 0)
		{
			if (unknown == r)
			{
				r = lt;
			}
			else if (gt == r)
			{
				return FALSE;
			}
		}

	}
	return TRUE;
}

BOOL TestSort::testCheckResult()
{
	int a1[] = {1, 2, 3, 4, 5 ,6, 7, 8, 9};
	int a2[] = {1, 2, 3, 4, 5 ,6, 7, 3, 9};
	int a3[] = {9, 8, 7, 6, 5, 4, 3, 2, 1};
	cout<< checkResult(a1, sizeof(a1) / sizeof(int));
	cout<< checkResult(a2, sizeof(a2) / sizeof(int));
	cout<< checkResult(a3, sizeof(a3) / sizeof(int));

	return TRUE;
}