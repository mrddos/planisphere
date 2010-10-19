// KxProject.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "kxstr.h"
#include "kxtype.h"
#include "kxarray.h"
#include "kxnum.h"



int _tmain(int argc, _TCHAR* argv[])
{
	RKxString s = L"Hello World";

	
	// ±‡“Î ß∞‹£¨≤ªø…“‘Cast£¨Good!
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

