// KxBasicTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"



#include <iKxLib/KxLib.h>
#include <iKxLib/KxGc.h>

#pragma comment(lib, "KxLib.lib")


class GcObject
{
public:
	GcObject()
	{

	}
};


class MyTask : public IKxRunnable
{
public:
	LRESULT Run()
	{
		return 0;
	}
};

int _tmain(int argc, _TCHAR* argv[])
{
	KxLib_Initialize();
	Gc_New_Function

	GcObject* pObject = gc_new GcObject();


	KxThread* thread = new KxThread(new MyTask());
	

	return 0;
}

