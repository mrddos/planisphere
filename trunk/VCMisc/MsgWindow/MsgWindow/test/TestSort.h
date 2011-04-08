
#ifndef ___TEST_SORT_H
#define ___TEST_SORT_H
#pragma once

class TestSort
{
public:
	static BOOL testSortLargeIntArray();
	static BOOL testCheckResult();

private:
	static BOOL checkResult(int* array, int size);
};

#endif