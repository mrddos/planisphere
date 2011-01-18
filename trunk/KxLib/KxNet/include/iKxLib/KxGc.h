
#ifndef ___KxGc_H__
#define ___KxGc_H__

#include "KxThread.h"

#ifdef __cplusplus
extern"C" {
#endif




class __KxLib_GcModule
{
public:
	__KxLib_GcModule(HMODULE hMod);
	~__KxLib_GcModule();

	static BOOL Initialize(HMODULE hMod);

	static __KxLib_GcModule* Instance();

	void* Alloc(size_t size, HANDLE hThread);

private:
	void __KxLib_GcModule_Initialize();
	typedef void* (*KX_GC_ALLOC)(size_t, HANDLE);
private:
	static __KxLib_GcModule* s_pKxLib_GcModule;
private:

	HMODULE	m_hMod;
	KX_GC_ALLOC	m_pGcAlloc;
};



void* operator new(size_t size, __KxLib_GcModule*, __KxLib_ThreadLocal*);


class KxGcNode
{
public:
	
};





__KxLib_GcModule* __KxLib_GcModule_Instance();



#define gc_new new((__KxLib_GcModule*)__KxLib_GcModule_Instance(), (__KxLib_ThreadLocal*)&___ILOCAL0)

#define ___ILOCAL0			__iLocal0
#define Gc_New_Function		__KxLib_ThreadLocal	___ILOCAL0(__FUNCTION__);



#ifdef __cplusplus
}
#endif



template<class T>
class _gc_member_ptr
{
public:
	_gc_member_ptr(KxGcNode* pGcNode)
	{
		pGcNode->AddMember(this);
	}

private:
};

template<class T>
class _gc_static_ptr
{

};

template<class T>
class _gc_local_ptr
{

};

#define __gc_member(x)			_gc_member_ptr<x>
#define __gc_static_member		_gc_static_ptr<x>
#define __gc_local				_gc_local_ptr<x>





#endif