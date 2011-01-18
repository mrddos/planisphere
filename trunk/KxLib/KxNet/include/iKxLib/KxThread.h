
#ifndef __KxThread_H__
#define __KxThread_H__


class __KxLib_ThreadLocal
{
public:
	__KxLib_ThreadLocal(const char* _function)
		: m_hThread(NULL), m_function(_function)
	{
		m_hThread = GetCurrentThread();

	}

	~__KxLib_ThreadLocal()
	{
		
	}

	HANDLE GetThreadHandle() const
	{
		return m_hThread;
	}

private:
	HANDLE m_hThread;
	const char* m_function;
};



class IKxRunnable
{
public:
	virtual LRESULT Run() = 0;
};

class __KxLib_ThreadModule
{
public:
	typedef HANDLE (*KX_CREATE_THREAD)(LPCWSTR, LPSECURITY_ATTRIBUTES, SIZE_T, IKxRunnable*, DWORD);

	__KxLib_ThreadModule(HMODULE hMod);
	static BOOL Initialize(HMODULE hMod);

	static __KxLib_ThreadModule* Instance();


	HANDLE Create(LPCWSTR, IKxRunnable*);
private:
	void __KxLib_ThreadModule_Initialize();
	static __KxLib_ThreadModule* s_pKxLib_ThreadModule;
	HMODULE	m_hMod;

private:
	KX_CREATE_THREAD	m_pKxCreateThread;
};



class KxThread
{
public:
	KxThread(IKxRunnable* pRunnable)
	{
		CString strName;
		strName.Format(L"THREAD-%d", (DWORD)this);
		__KxLib_ThreadModule::Instance()->Create(strName, pRunnable);
	}

	KxThread(LPCWSTR lpName, IKxRunnable* pRunnable)
	{
		__KxLib_ThreadModule::Instance()->Create(lpName, pRunnable);
	}private:

};

	


#endif