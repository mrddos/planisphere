// KxProject.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "kxstr.h"
#include "kxtype.h"
#include "kxarray.h"
#include "kxnum.h"

//template<class T>


class A
{
public:
	A(){
		cout<<"1"<<endl;
	}
};

void* operator new(size_t s, int a, int b, A)
{
	return (void*)1;
}

#define gc_new(x) new (2, 4, x())

void test_new()
{
	//int* a = (new (3, 4, 5)) int(9);
	A* p = gc_new(A) A();
}



int _tmain(int argc, _TCHAR* argv[])
{
	test_new();
	RKxString s = L"Hello World";

	
	// ±àÒëÊ§°Ü£¬²»¿ÉÒÔCast£¬Good!
	//RKxType t = s; 

	RKxArray array = Ref<KxArray>::GcNew();
	array->Add(s);

	RKxObject o = array->Get(0);

	RKxString ds = Ref<KxString>::downcast(o);

	RKxInteger i1 = 4;
	RKxInteger i2 = 8;

	array->Add(i1);
	array->Add(i2);

	RKxObject w = array->Get(1);
	wcout<<w;

	wcout<<o->ToString();
	cout<<endl;
	wcout<<o;
	cout<<endl;
	return 0;
}

