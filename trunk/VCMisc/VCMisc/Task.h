
#ifndef __Task_h__
#define __Task_h__

#include "Runnable.h"

class Task : public Runnable
{
public:
	int Run()
	{
		PreExecute();
		Execute();
		PostExecute();
	}
	virtual int PreExecute() = 0;
	virtual int Execute() = 0;
	virtual int PostExecute() = 0;

};


#endif