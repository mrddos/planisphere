
#ifndef __PartialQSortTask_h__
#define __PartialQSortTask_h__

#include "Task.h"

#include <stack>
using std::stack;

template<class E, class F>
class PartialQSort
{
public:
	PartialQSort()
		:_a(NULL), _size(0),
		 _iMinPartLen(PartialContext::DefaultMinPartLength)
	{

	}

	PartialQSort(E a[], int size)
		:_a(a), _size(size),
		 _iMinPartLen(PartialContext::DefaultMinPartLength)
	{

	}

	~PartialQSort()
	{
		
	}

	void SetMinPartLength(int iMinPartLen)
	{
		_iMinPartLen = iMinPartLen;
	}


	// Recursion
	void _QuickSort(E a[], int size, int from, int to)
	{
		if (from >= to) return;

		F _c;
		E mv = a[to];
		int i1 = from;
		int i2 = to;

		i1 -= 1;
		do
		{
			while (_c(a[++i1], mv));
			while (_c(mv, a[--i2]))
			{
				if (i2 == from) break;
			}

			if (i1 >= i2) break;

			_Swap(a[i1], a[i2]);
			//print(a, size);
		}
		while(1);

		_Swap(a[i1], a[to]);
		//print(a, size);

		_QuickSort(a, size, from, i1 - 1);
		_QuickSort(a, size, i1 + 1, to);

	}

	class PartialContext
	{
	public:
		PartialContext(int from_, int to_)
			:from(from_), to(to_), finished(Unfinished)
		{
			//root = NULL;
			//left = NULL;
			//right = NULL;
		}

		enum{
			Unfinished = 0,
			Finished = 1,
			DefaultMinPartLength = 64
		};

		int from;
		int to;
		int finished;

	};

	// Context
	bool _QuickSortByContext(E a[], int size, PartialContext const& context)
	{
		if (context.from >= context.to)
			return false;

		F _c;
		E mv = a[context.to];
		int i1 = context.from;
		int i2 = context.to;

		i1 -= 1;
		do
		{
			while (_c(a[++i1], mv));
			while (_c(mv, a[--i2]))
			{
				if (i2 == context.from) break;
			}

			if (i1 >= i2) break;

			_Swap(a[i1], a[i2]);
			//print(a, size);
		}
		while(1);

		_Swap(a[i1], a[context.to]);
		//print(a, size);

		// ==> i1;

		PartialContext ctx1(i1 + 1, context.to);
		_context.push(ctx1);


		PartialContext ctx2(context.from, i1 - 1);
		_context.push(ctx2);
		return true;
	}


	// Sort all elements in one time;
	void QuickSort(E a[], int size)
	{
		ATLASSERT(NULL == _a);
		this->_a = a;
		this->_size = size;
		_QuickSort(a, size, 0, size - 1);
	}

	// Sort all elements step by step.
	void QuickSort()
	{
		
		if (_context.empty())
		{
			PartialContext context(0, _size - 1);
			_QuickSortByContext(_a, _size, context);
		}
		else
		{
			PartialContext context = _context.top();
			_context.pop();
			_QuickSortByContext(_a, _size, context);
		}

		
	}



private:
	void _Swap(E& a, E& b)
	{
		E t = a; a = b; b = t;
	}

private:
	E*		_a;
	int		_size;
	int		_iMinPartLen;
	stack<PartialContext>		_context; // Root Context;
};



template<class E, class F>
class PartialQSortTask : public ITask, protected PartialQSort<E, F>
{
public:
	PartialQSortTask()
		:m_TaskStatus(eTsUndefined)
	{

	}

	virtual void OnTaskBegin(DWORD)
	{

	}

	virtual void OnTaskPaused(DWORD)
	{

	}

	virtual void OnTaskResumed(DWORD)
	{

	}

	virtual void OnTaskFinished(DWORD)
	{

	}

	TaskStatus GetStatus() const
	{
		return m_TaskStatus;
	}

protected:
	TaskStatus	m_TaskStatus;

};


#endif