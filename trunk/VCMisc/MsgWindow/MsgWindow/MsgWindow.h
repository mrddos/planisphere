
#ifndef __MsgWindow_H__
#define __MsgWindow_H__
#pragma once

#define MSG_START_TASK				(WM_USER + 105)
#define MSG_PAUSE_TASK				(WM_USER + 106)
#define MSG_RESUME_TASK				(WM_USER + 107)
#define MSG_STOP_TASK				(WM_USER + 108)
#define MSG_QUERY_TASK_STATUS		(WM_USER + 109)

#include "Task.h"

class MsgWindow
{
public:
	MsgWindow()
		: m_lpClassName(L"Kx:MsgWindow")
	{

	}

	~MsgWindow()
	{

	}

	HWND Create(HINSTANCE hInstance = NULL)
	{
		BOOL bReg = _Register(hInstance);
		ATLASSERT(bReg);
		m_hWnd = CreateWindowEx(0, m_lpClassName, NULL, 0, 0, 0, 0, 0, HWND_MESSAGE, NULL, hInstance, NULL);
		if (NULL != m_hWnd)
		{
			return m_hWnd;
		}
		return NULL;
	}

private:
	BOOL _Register(HINSTANCE hInstance)
	{
		WNDCLASSEXW wc = {};
		wc.cbSize = sizeof(WNDCLASSEX);
		wc.hInstance = hInstance;
		wc.lpszClassName = m_lpClassName;
		wc.lpfnWndProc = _WndProc;
		if (!RegisterClassEx(&wc))
		{
			DWORD dwError = GetLastError();
		}
		return TRUE;
	}

	static LRESULT CALLBACK _WndProc(HWND hWnd, UINT msg, WPARAM w, LPARAM l)
	{
		ITask* pTask = (ITask*)w;
		if (MSG_START_TASK == msg)
		{
			pTask->OnTaskBegin(0);
			return 0;
		}
		else if (MSG_RESUME_TASK == msg)
		{
			pTask->OnTaskResumed(0);
			return 0;
		}
		else if (MSG_PAUSE_TASK == msg)
		{
			pTask->OnTaskPaused(0);
			return 0;
		}
		else if (MSG_STOP_TASK == msg)
		{
			pTask->OnTaskFinished(0);
			return 0;
		}
		else if (MSG_QUERY_TASK_STATUS == msg)
		{
			TaskStatus* pTaskStatus = (TaskStatus*)l;
			*pTaskStatus = pTask->GetStatus();
			return 0;
		}

		return DefWindowProc(hWnd, msg, w, l);
	}

private:
	LPCWSTR		m_lpClassName;
	HWND		m_hWnd;
};



#endif