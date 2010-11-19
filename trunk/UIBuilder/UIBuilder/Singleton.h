
#ifndef __SINGLETON_H__
#define __SINGLETON_H__

#include "Lock.h"

class _SingleThreadLock
{
public:
	_SingleThreadLock(){}
	inline void Lock(){}
	inline void Unlock(){}
private:
};

class _MultiThreadLock
{
public:
	_MultiThreadLock(){}
	void Lock()
	{
		m_lock.Lock();
	}
	void Unlock()
	{
		m_lock.Unlock();
	}

private:
	ACSLock m_lock;
};

template<typename OT, bool MT = false>
class _ThreadModel
{
public:
	typedef OT* ObjectPtrT;
	typedef _SingleThreadLock LockT;
};

template<typename OT>
class _ThreadModel<OT, true>
{
public:
	typedef OT* volatile ObjectPtrT;
	typedef _MultiThreadLock LockT;
};


template<typename OT, bool MT = false>
class Singleton
{
private:
	typedef typename _ThreadModel<OT, MT>::ObjectPtrT ObjectPtrT;
	typedef typename _ThreadModel<OT, MT>::LockT LockType;

public:
	static OT* Instance()
	{
		return _GetInstancePtr();
	}

private:
	// DCL
	static OT* _GetInstancePtr()
	{
		static ObjectPtrT p = NULL;
		if (NULL == p)
		{
			_lock.Lock();
			if (NULL == p)
			{
				p = new OT();
			}
			_lock.Unlock();
		}
		return p;
	}

private:
	static LockType _lock; //__declspec(selectany) 

};

template<class OT, bool MT>
__declspec(selectany) typename _ThreadModel<OT, MT>::LockT Singleton<OT, MT>::_lock;



#endif
