
#ifndef __TaskManager_h__
#define __TaskManager_h__
#pragma once

#include "Task.h"

#include <map>
using std::map;

class TaskManager
{
public:
	TaskManager()
		: m_pCurrentTask(NULL)
	{

	}

	void AddTask(ITask* pTask)
	{
		m_pCurrentTask = pTask;
	}

	ITask* GetCurrentTask() const
	{
		return m_pCurrentTask;
	}

	ITask* GetTask(CString const& strTaskId) const
	{
		map<CString, ITask*>::const_iterator iter = m_taskMap.find(strTaskId);
		if (m_taskMap.end() != iter)
		{
			return (ITask*)iter->second;
		}
		return NULL;
	}

private:
	ITask* m_pCurrentTask;

	map<CString, ITask*> m_taskMap;
};

#endif