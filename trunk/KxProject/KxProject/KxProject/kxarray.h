#ifndef __KX_ARRAY_H__
#define __KX_ARRAY_H__

#include "ref.hpp"

#include <vector>

class KxArray : public KxObject
{
public:

	enum
	{
		ClassId = 10
	};


	typedef KxObject BaseType;
	typedef _NoneObjectClass ImplicitType;
	KxArray(){}


	void Add(RKxObject object)
	{
		m_vec.push_back(object);
	}

	RKxObject Get(int index)
	{
		return m_vec[index];
	}

	virtual Ref<KxString> ToString()
	{
		return m_arrayName;
	}




private:
	Ref<KxString> m_arrayName;
	std::vector<Ref<KxObject> > m_vec;
};

typedef Ref<KxArray> RKxArray;

#endif