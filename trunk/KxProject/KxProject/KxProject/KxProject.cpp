// KxProject.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "kxstr.h"
#include "kxtype.h"
#include "kxarray.h"




int _tmain(int argc, _TCHAR* argv[])
{
	RKxString s = L"Hello World, ฤ๚บร";

	//RKxType t = s;
	//RKxString b = o;

	RKxArray array = Ref<KxArray>::GcNew();
	array->Add(s);

	RKxObject o = array->Get(0);

	wcout<<o->ToString();
	cout<<endl;
	wcout<<o;
	cout<<endl;
	return 0;
}

