
#ifndef __KxManagedThread_h__
#define __KxManagedThread_h__


#include <iKxLib\KxDef.h>

enum
{
	NotAllocated	= 0,
	Allocated		= 1,
};

class IKxRunnable;

class KxManagedThread
{
public:
	KxManagedThread();

	typedef int MainThreadType;

	static void Initialize();
	static void Initialize(MainThreadType);

	//************************************
	// Method:    Create
	// FullName:  KxManagedThread::Create
	// Access:    public 
	// Returns:   HTHREAD
	// Qualifier:
	//************************************
	static KxManagedThread* GetManagedThread();
	static HTHREAD KxCreateMessageThread();
	static HTHREAD KxAttachThread(HANDLE, DWORD);


	void AttachRunnable(IKxRunnable* pRunnable);
	void SetThreadHandle(HANDLE hThread);
	HANDLE GetThreadHandle() const;
	IKxRunnable* GetRunnable() const;
private:
	static HTHREAD _KxCreateThread();
	BOOL _ClearManagedThread(HANDLE hThread);
public:
	static DWORD __stdcall KxManagedThreadProc(LPVOID);
	static DWORD __stdcall KxManagedMsgThreadProc(LPVOID);
private: // static members
	static KxManagedThread*		s_pManagedThreadArray;
	static DWORD				m_dwThreadCount;

private: // members
	DWORD			m_dwAlloc;
	IKxRunnable*	m_pRunnable;
	HANDLE			m_hThread;

};

#endif