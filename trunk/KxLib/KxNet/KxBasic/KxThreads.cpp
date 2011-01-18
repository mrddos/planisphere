
#include "stdafx.h"
#include "KxThreads.h"

#include <iKxLib/KxThread.h>

KxManagedThread*	KxManagedThread::s_pManagedThreadArray = NULL;
DWORD				KxManagedThread::m_dwThreadCount = 0;



KxManagedThread::KxManagedThread()
: m_dwAlloc(NotAllocated), m_pRunnable(NULL)
{
	
}

HTHREAD KxManagedThread::KxCreateMessageThread()
{
	return 0;
}


HTHREAD KxManagedThread::KxAttachThread(HANDLE, DWORD)
{
	return HTHREAD(0);
}

KxManagedThread* KxManagedThread::GetManagedThread()
{
	for (int i = 0; i < (int)m_dwThreadCount; ++i)
	{
		KxManagedThread* pManagedThread = (KxManagedThread*)&s_pManagedThreadArray[i];
		if (NotAllocated == pManagedThread->m_dwAlloc)
		{
			pManagedThread->m_dwAlloc = Allocated;
			return pManagedThread;
		}
	}
	return NULL;
}

void KxManagedThread::Initialize()
{
	const int DefaultThreadMaxCount = 256;

	m_dwThreadCount = DefaultThreadMaxCount;
	s_pManagedThreadArray = new KxManagedThread[m_dwThreadCount];
}

void KxManagedThread::Initialize(MainThreadType)
{
	KxManagedThread* pManagedThread = KxManagedThread::GetManagedThread();
	ATLASSERT(NULL != pManagedThread);
	
}

DWORD __stdcall KxManagedThread::KxManagedThreadProc(LPVOID lpThis)
{
	KxManagedThread* pManagedThread = (KxManagedThread*)lpThis;
	ATLASSERT(pManagedThread);
	if (NULL != pManagedThread)
	{
		HANDLE hThread = GetCurrentThread();
		pManagedThread->SetThreadHandle(hThread);
		IKxRunnable* pRunnable = pManagedThread->GetRunnable();
		if (NULL != pRunnable)
		{
			LRESULT hr = pRunnable->Run();
		}
	}


	return 0;
}

DWORD __stdcall KxManagedThread::KxManagedMsgThreadProc( LPVOID )
{

	return 0;
}

void KxManagedThread::AttachRunnable(IKxRunnable* pRunnable)
{
	m_pRunnable = pRunnable;
}

IKxRunnable* KxManagedThread::GetRunnable() const
{
	return m_pRunnable;
}

void KxManagedThread::SetThreadHandle(HANDLE hThread)
{
	m_hThread = hThread;
}

HANDLE KxManagedThread::GetThreadHandle() const
{
	return m_hThread;
}


BOOL KxManagedThread::_ClearManagedThread(HANDLE hThread)
{
	for (int i = 0; i < (int)m_dwThreadCount; ++i)
	{
		KxManagedThread* pManagedThread = (KxManagedThread*)&s_pManagedThreadArray[i];
		if (Allocated == pManagedThread->m_dwAlloc &&
			pManagedThread->GetThreadHandle() == hThread)
		{
			pManagedThread->m_dwAlloc = NotAllocated;
			pManagedThread->SetThreadHandle(NULL);
			return TRUE;
		}
	}
	return FALSE;
}

