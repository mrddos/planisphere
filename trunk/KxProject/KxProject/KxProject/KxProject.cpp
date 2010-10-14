// KxProject.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include "kxstr.h"
#include "kxtype.h"
#include "kxarray.h"




int _tmain(int argc, _TCHAR* argv[])
{
	RKxString s = L"Hello World, 您好";

	
	// 编译失败，不可以Cast，Good!
	//RKxType t = s; 

	RKxArray array = Ref<KxArray>::GcNew();
	array->Add(s);

	RKxObject o = array->Get(0);

	RKxString ds = Ref<KxString>::downcast(o);

	wcout<<o->ToString();
	cout<<endl;
	wcout<<o;
	cout<<endl;
	return 0;
}

