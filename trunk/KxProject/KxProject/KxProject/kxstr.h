#ifndef __KX_STR_H__
#define __KX_STR_H__

#include "kxobj.h"

#include <strsafe.h>

class KxString : public KxObject
{
public:

	enum
	{
		ClassId = KxObject::ClassId + 1
	};
	typedef KxObject BaseType;
	typedef const wchar_t* ImplicitType;

	virtual Ref<KxString> ToString()
	{
		return Ref<KxString>(this);
	}

	KxString(const wchar_t* value)
	{
		size_t len = wcslen(value);
		m_pStr = new wchar_t[len + 1];
		StringCchCopy(m_pStr, (len + 1) * 2, value);
	}

	wchar_t* ToWideChars() const
	{
		return m_pStr;
	}

private:
	wchar_t* m_pStr;
};


typedef Ref<KxString> RKxString;

template<>
Ref<KxString>::Ref(ImplicitType implicitValue)
{
	KxString* p = new KxString(implicitValue);
	m_p = p;
	//wcout<<implicitValue<<endl;
}


wostream& operator<<(wostream& os, Ref<KxString> const& kxStr)
{
	os<< kxStr->ToWideChars();
	return os;
}

wostream& operator<<(wostream& os, Ref<KxObject> const& kxObject)
{
	os<< kxObject->ToString();
	return os;
}

#include "kxnum.h"

wostream& operator<<(wostream& os, Ref<KxInteger> const& kxInt)
{
	//os<< kxInt.m_lValue;
	return os;
}


wostream& operator<<(wostream& os, Ref<KxFloat> const& kxFloat)
{
	//os<< kxObject->ToString();
	return os;
}



#endif