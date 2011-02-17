// KxBasic.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "KxBasic.h"
#include "Raven\raven_gc_head.h"
#include "Raven\block.h"
#include "KxThreads.h"

#include "GcModule.h"




class IKxRunnable;


LRESULT Kx_Initialize(HMODULE hMod)
{
	GcModule* pGcModule = GcModule::GetGcModule();
	if (pGcModule)
	{
		LRESULT hr = pGcModule->Initialize();
		
		KxManagedThread::Initialize(KxManagedThread::MainThreadType());
		
		return hr;
	}
	return E_FAIL;
}

LRESULT Kx_Log(LPCWSTR lpMsg, BOOL bToDbgView, HANDLE hFile)
{

	return S_OK;
}

//////////////////////////////////////////////////////////////////////////
// 
void* Kx_Gc_Alloc(size_t size, HANDLE hThread)
{
	GcModule* pGcModule = GcModule::GetGcModule();
	ATLASSERT(NULL != pGcModule);
	void* pMemory = pGcModule->Alloc(size, hThread);
	return pMemory;
}


void* Kx_Gc_Recycle(void* pMemory, HANDLE hThread)
{
	
	return 0;
}

BOOL Kx_Gc_RunGC()
{

	return TRUE;
}

LRESULT Kx_Block_Initialize(UINT nObjectSize, UINT nCapacity, HBLOCK* pBlock)
{
	_Block* block = new _Block(nObjectSize, nCapacity);
	block->Initialize();
	__BLOCK* p = new __BLOCK();
	p->_block = block;

	

	
	
	*pBlock = p;

	return S_OK;
}


LRESULT Kx_Block_Alloc(HBLOCK hBlock, UINT nSize, void** p)
{
	unsigned char* h = (unsigned char*)((_Block*)hBlock->_block)->Alloc();
	*p = h + sizeof(Raven_GC_Head);
	return S_OK;
}


HSTRING Kx_GC_Alloc_String(LPCWSTR lpString)
{
	HSTRING hString = NULL;
	return hString;
}

HANDLE Kx_Create_Thread(LPCWSTR lpName, LPSECURITY_ATTRIBUTES lpThreadAttributes, SIZE_T dwStackSize, IKxRunnable* pRunnable, DWORD dwCreationFlags)
{
	DWORD dwThreadId = 0;
	KxManagedThread* pManagedThread = KxManagedThread::GetManagedThread();
	pManagedThread->AttachRunnable(pRunnable);
	HANDLE hThread = CreateThread(lpThreadAttributes, dwStackSize, (LPTHREAD_START_ROUTINE)KxManagedThread::KxManagedThreadProc, pManagedThread, dwCreationFlags, &dwThreadId);
	return hThread;
}


HANDLE Kx_Create_Message_Thread(LPCWSTR lpName, LPSECURITY_ATTRIBUTES lpThreadAttributes, SIZE_T dwStackSize, IKxRunnable* pRunnable, DWORD dwCreationFlags)
{
	DWORD dwThreadId = 0;
	KxManagedThread* pManagedThread = KxManagedThread::GetManagedThread();
	pManagedThread->AttachRunnable(pRunnable);
	HANDLE hThread = CreateThread(lpThreadAttributes, dwStackSize, (LPTHREAD_START_ROUTINE)KxManagedThread::KxManagedMsgThreadProc, pManagedThread, dwCreationFlags, &dwThreadId);
	return hThread;
}

LRESULT Kx_Join_Thread(HANDLE hThread)
{

	return S_OK;
}