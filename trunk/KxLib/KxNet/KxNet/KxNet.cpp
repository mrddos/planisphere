// KxNet.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "KxNet.h"


// This is an example of an exported variable
KXNET_API int nKxNet=0;

// This is an example of an exported function.
KXNET_API int fnKxNet(void)
{
	return 42;
}

// This is the constructor of a class that has been exported.
// see KxNet.h for the class definition
CKxNet::CKxNet()
{
	return;
}
