#ifndef __KX_OBJ_H__
#define __KX_OBJ_H__

#include "ref.hpp"

class KxString;

class KxObject
{
public:

	enum
	{
		ClassId = 1
	};


	typedef _NoneObjectClass BaseType;
	typedef _NoneObjectClass ImplicitType;
	KxObject(){}


	virtual Ref<KxString> ToString() = 0;


private:
};

typedef Ref<KxObject> RKxObject;




#endif