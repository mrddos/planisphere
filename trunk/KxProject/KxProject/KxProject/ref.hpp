/************************************************************************/
/*                                                                      */
/* C++ Code and Java Style                                              */
/* Gc                                                                   */
/*                                                                      */
/*                                                                      */
/*                                                                      */
/* healer_kx@163.com                                                    */
/************************************************************************/
#ifndef __REF_HPP__
#define __REF_HPP__

#include <iostream>
using namespace std;

class _NoneObjectClass
{
public:
	enum
	{
		ClassId = 0
	};
protected:
private:
};

template<class T1, class T2>
class _ClassEqual
{
public:
	enum
	{
		equal = false
	};
};

template<class T>
class _ClassEqual<T, T>
{
public:
	enum
	{
		equal = true
	};
};

template<class T1, class T2>
class _InhritFrom
{
public:
	enum
	{
		inherit = _InhritFrom<typename T1::BaseType, T2>::inherit
	};
};

template<class T>
class _InhritFrom<T, T>
{
public:
	enum
	{
		inherit = true
	};
};

template<class T>
class _InhritFrom<_NoneObjectClass, T>
{
public:
	enum
	{
		inherit = false
	};
};


// class IntegerFlyweight
// {
// public:
// private:
// };
// 
// class FloatFlyweight
// {
// public:
// private:
// };




template<class T>
class Ref
{
public:
	//friend T;
	typedef typename T::ImplicitType ImplicitType;

	Ref(){}

	template<class R>
	Ref(Ref<R> const& right)
	{
		if (_InhritFrom<T, R>::inherit)
		{
			cout<<typeid(T).name()<<" inherit from "<<typeid(R).name()<<endl;
		}

		if (_InhritFrom<R, T>::inherit)
		{
			cout<<typeid(R).name()<<" inherit from "<<typeid(T).name()<<endl;
		}
		m_p = right.Ptr();
		cout<<"OVER" <<endl;
	}

	Ref(ImplicitType implicitValue);
	
	Ref(T* pThis)
	{
		m_p = pThis;
	}

	template<class R>
	static Ref<T> downcast(Ref<R> const& right)
	{
		if (_InhritFrom<T, R>::inherit)
		{
			cout<<typeid(T).name()<<" inherit from "<<typeid(R).name()<<endl;
			Ref<T> r((T*)right.Ptr());
			return r;
		}
		ATLASSERT(FALSE);
	}


	inline T* Ptr() const
	{

		return m_p;
	}

	inline T* operator->() const
	{
		return m_p;
	}

	static Ref<T> GcNew()
	{
		T* p = new T();
		return Ref<T>(p);
	}



private:
	union
	{
		T* m_p;
		T::basic_type m_b;
	};

};



#endif