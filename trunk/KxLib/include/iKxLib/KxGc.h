
#ifndef ___KxGc_H__
#define ___KxGc_H__


class KxGcModule
{
public:
	KxGcModule();
	~KxGcModule();

	LRESULT Initialize();

	static KxGcModule* Instance();

	void* Alloc(size_t size);

private:
	typedef LRESULT (*KX_GC_INITIALIZE)();
private:
	HMODULE	m_hMod;
	KX_GC_INITIALIZE	m_pGcInitialize;
};

__declspec(selectany) KxGcModule g_KxGcModule;

void* operator new(size_t size, KxGcModule*);


class KxGcNode
{
public:
	
};

#define Gc_New new(&g_KxGcModule)



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