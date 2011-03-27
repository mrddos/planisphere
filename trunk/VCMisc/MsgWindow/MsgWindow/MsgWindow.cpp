// MsgWindow.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "Resource.h"
#include "MsgThread.h"

#include <algorithm>
using namespace std;

#include "PartialQSortTask.h"


#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE hInst;								// current instance
TCHAR szTitle[MAX_LOADSTRING];					// The title bar text
TCHAR szWindowClass[MAX_LOADSTRING];			// the main window class name

// Forward declarations of functions included in this code module:
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

class FileSizeComparator
{
public:
	bool operator()(CString const& a, CString const& b)
	{

		return a < b;
	}
private:
};

class IntegerComparator
{
public:
	bool operator()(int a, int b)
	{

		return a < b;
	}
private:
};

bool comp(int a, int b)
{
	return a < b;
}


class SomeObject
{
public:

private:
};

class SomeObjectComparator
{
public:
	bool operator()(SomeObject const& a, SomeObject const& b)
	{

		return true;
	}

private:
};

static bool operator != (SomeObject const& a, SomeObject const& b)
{

	return true;
}

typedef PartialQSortTask<CString, FileSizeComparator> FilesPartialQSortTask;



int APIENTRY _tWinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPTSTR    lpCmdLine,
                     int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

 	// TODO: Place code here.
	MSG msg;
	HACCEL hAccelTable;

	// Initialize global strings
	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadString(hInstance, IDC_MSGWINDOW, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance);

	
	CString strFileList[] = { L"1", L"9", L"2", L"4", L"9", L"6", L"8", L"7", L"5" };
	PartialQSort<CString, FileSizeComparator> partialQSort(strFileList, 9);
	partialQSort.QuickSort();
	partialQSort.QuickSort();
	partialQSort.QuickSort();
	partialQSort.QuickSort();
	partialQSort.QuickSort();
	partialQSort.QuickSort();
	partialQSort.QuickSort();
	partialQSort.QuickSort();
	partialQSort.QuickSort();
	partialQSort.QuickSort();
	

	

	//int array[] = { 1, 7, 5, 2, 5, 6, 2, 87, 9, 32, 23 ,32, 23 ,23 ,26, 35, 46, 3, 4, 5, 7, 8, 9, 23, 423, 4, 1};
	int array[] = { 1, 2, 3};

	//int array[] = { 1, 2, 3, 5, 4};
	PartialQSort<int, IntegerComparator> partialIntegerQSort(array, sizeof(array) / sizeof(int));
	int count = 0;
	for (int i = 0; i < 60000; ++i)
	{
		PartialQSortStep step = partialIntegerQSort.QuickSort();
		if (AllSorted == step)
		{
			break;
		}
		else if (PartialSorted == step)
		{
			count++;
		}
	}


	SomeObject so[1];
	PartialQSort<SomeObject, SomeObjectComparator> someObjectQSort(so, 1);
	someObjectQSort.QuickSort();



	// Perform application initialization:
	if (!InitInstance (hInstance, nCmdShow))
	{
		return FALSE;
	}

	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_MSGWINDOW));

	MsgThread msgThread;
	msgThread.Create();

	CString strTaskId;
	msgThread.StartTask(new FilesPartialQSortTask(), strTaskId);

	msgThread.PauseTask(strTaskId);
	// msgThread.SendMessage(WM_MSG_WINDOW, 0, 1);
	// Main message loop:
	while (GetMessage(&msg, NULL, 0, 0))
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	return (int) msg.wParam;
}



//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
//  COMMENTS:
//
//    This function and its usage are only necessary if you want this code
//    to be compatible with Win32 systems prior to the 'RegisterClassEx'
//    function that was added to Windows 95. It is important to call this function
//    so that the application will get 'well formed' small icons associated
//    with it.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
	WNDCLASSEX wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style			= CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc	= WndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hInstance;
	wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_MSGWINDOW));
	wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
	wcex.lpszMenuName	= MAKEINTRESOURCE(IDC_MSGWINDOW);
	wcex.lpszClassName	= szWindowClass;
	wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassEx(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   HWND hWnd;

   hInst = hInstance; // Store instance handle in our global variable

   hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   return TRUE;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the main window.
//
//  WM_COMMAND	- process the application menu
//  WM_PAINT	- Paint the main window
//  WM_DESTROY	- post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	int wmId, wmEvent;
	PAINTSTRUCT ps;
	HDC hdc;

	switch (message)
	{
	case WM_COMMAND:
		wmId    = LOWORD(wParam);
		wmEvent = HIWORD(wParam);
		// Parse the menu selections:
		switch (wmId)
		{
		case IDM_ABOUT:
			DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
			break;
		case IDM_EXIT:
			DestroyWindow(hWnd);
			break;
		default:
			return DefWindowProc(hWnd, message, wParam, lParam);
		}
		break;
	case WM_PAINT:
		hdc = BeginPaint(hWnd, &ps);
		// TODO: Add any drawing code here...
		EndPaint(hWnd, &ps);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(lParam);
	switch (message)
	{
	case WM_INITDIALOG:
		return (INT_PTR)TRUE;

	case WM_COMMAND:
		if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
		{
			EndDialog(hDlg, LOWORD(wParam));
			return (INT_PTR)TRUE;
		}
		break;
	}
	return (INT_PTR)FALSE;
}
