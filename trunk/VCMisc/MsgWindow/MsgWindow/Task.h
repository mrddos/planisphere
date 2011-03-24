
#ifndef __Task_h__
#define __Task_h__
#pragma once


enum TaskStatus
{
	eTsUndefined,
	eTsRunning,
	eTsPaused,
	eTsFinished,
};

class ITask
{
public:


	virtual void OnTaskBegin(DWORD) = 0;
	virtual void OnTaskPaused(DWORD) = 0;
	virtual void OnTaskResumed(DWORD) = 0;
	virtual void OnTaskFinished(DWORD) = 0;

	virtual TaskStatus GetStatus() const = 0;
	
private:

};

#endif