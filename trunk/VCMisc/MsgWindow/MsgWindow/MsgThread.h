
#ifndef __MsgThread_H__
#define __MsgThread_H__
#pragma once

#include "MsgWindow.h"
#include "Task.h"
#include "TaskManager.h"

class MsgThread
{
public:
	MsgThread()
	{

	}

	BOOL Create()
	{
		m_hEvent = CreateEvent(NULL, TRUE, FALSE, L"Kx:MsgThread");
		m_hThread = CreateThread(NULL, 0, _ThreadProc, this, 0, &m_dwThreadId);
		WaitForSingleObject(m_hEvent, INFINITE);
		CloseHandle(m_hEvent);
		return TRUE;
	}

	BOOL SendMessage(UINT msg, WPARAM w, LPARAM l)
	{
		::SendMessage(m_hWnd, msg, w, l);
		return TRUE;
	}


	ITask* StartTask(ITask* pNewTask, ATL::CString& strTaskId)
	{
		ATLASSERT(NULL != pNewTask);
		ITask* pPrevTask = m_taskMgr.GetCurrentTask();
		m_taskMgr.AddTask(pNewTask);

		if (NULL != pPrevTask)
		{
			SendMessage(MSG_PAUSE_TASK, (WPARAM)pPrevTask, 0);
		}
		SendMessage(MSG_START_TASK, (WPARAM)pNewTask, 0);
		return pPrevTask;
	}

	ITask* PauseTask(CString const& strTaskId)
	{
		ITask* pTask = m_taskMgr.GetTask(strTaskId);
		if (NULL != pTask)
		{
			SendMessage(MSG_PAUSE_TASK, (WPARAM)pTask, 0);
		}
		return NULL;
	}

	ITask* ResumeTask(CString const& strTaskId)
	{
		ITask* pTask = m_taskMgr.GetTask(strTaskId);
		if (NULL != pTask)
		{
			SendMessage(MSG_RESUME_TASK, (WPARAM)pTask, 0);
		}
		return NULL;
	}

	BOOL StopTask(CString const& strTaskId)
	{
		ITask* pTask = m_taskMgr.GetTask(strTaskId);
		if (NULL != pTask)
		{
			SendMessage(MSG_STOP_TASK, (WPARAM)pTask, 0);
		}
		return TRUE;
	}

	BOOL QueryTaskStatus(CString const& strTaskId, TaskStatus& taskStatus)
	{
		ITask* pTask = m_taskMgr.GetTask(strTaskId);
		if (NULL != pTask)
		{
			taskStatus = eTsUndefined;
			SendMessage(MSG_QUERY_TASK_STATUS, (WPARAM)pTask, (LPARAM)&taskStatus);
		}
		return TRUE;
	}
private:
	void _Attach(HWND hWnd)
	{
		m_hWnd = hWnd;
		SetEvent(m_hEvent);
	}

	static DWORD CALLBACK _ThreadProc(LPVOID lpVoid)
	{
		MsgWindow msgWnd;
		HWND hWnd = msgWnd.Create();
		if (NULL != hWnd)
		{
			((MsgThread*)lpVoid)->_Attach(hWnd);

			MSG msg;
			while (GetMessage(&msg, NULL, 0, 0))
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
			}
		}
		return 0;
	}
private:
	HWND		m_hWnd;
	HANDLE		m_hThread;
	DWORD		m_dwThreadId;
	HANDLE		m_hEvent;
	TaskManager m_taskMgr;
};

#endif