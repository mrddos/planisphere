
#ifndef __KxDef_H__
#define __KxDef_H__


struct __STRING
{
	LPWSTR	_str;
	DWORD	_dwLen;
};

typedef __STRING FAR*	HSTRING;

struct __BLOCK
{
	void*	_block;
};

typedef __BLOCK FAR*	HBLOCK;


struct __THREAD
{
	HANDLE	_hThread;
	DWORD	_dwThreadId;
};


typedef __THREAD FAR*	HTHREAD;



struct __TIMER
{
	UINT	_nTimer;
};

typedef __TIMER FAR*	HTIMER;









#endif