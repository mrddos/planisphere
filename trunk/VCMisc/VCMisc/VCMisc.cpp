// VCMisc.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <atlpath.h>

#include <iostream>
using namespace std;

#include "CString/CStringEx.h"
#include "CPath/CPathEx.h"

#include "MessageThread.h"


wostream& operator<<(wostream& os, CString const& str)
{
	os<<(LPCWSTR)str;
	return os;
}

void TestMsgThread()
{
	MessageThread* mt = new MessageThread();
	mt->Create();
	mt->SendMessage(0, 0);
	
	MessageBox(NULL, NULL, NULL, NULL);
	mt->SendMessage(1, 2);
	MessageBox(NULL, NULL, NULL, NULL);
	mt->SendMessage(3, 4);

}



int _tmain(int argc, _TCHAR* argv[])
{
	TestMsgThread();

	CString a = L"Hello World";
	CString s1 = CStringEx::Lower(a);
	CString s2 = CStringEx::Upper(a);

	//CString
	if (CStringEx::Equal(s1, s2, TRUE))
	{

		wcout<<CStringEx::Format(L"%d: %s == %s", 2, (LPCWSTR)s1, (LPCWSTR)s2);
	}

	CString strPath = CPathEx::GetWindowsDirectory();

	CPath a3;
	
	RString b1 = "hello ÄãºÃ";
	CString b2 = L"hello ÄãºÃ";
	
	
	
	MessageBox(NULL, NULL, NULL, NULL);
	return 0;
}

