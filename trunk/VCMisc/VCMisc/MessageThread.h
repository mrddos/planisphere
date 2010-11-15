
#ifndef __MESSAGETHREAD_H__
#define __MESSAGETHREAD_H__


#define MTW_MSG L"MTW_MSG"

class MessageThread
{
public:
	MessageThread()
		:m_hEvent(NULL)
	{

	}

	BOOL Create()
	{
		m_hEvent = CreateEvent(NULL, TRUE, FALSE, L"MessageThreadCreated");
		m_hThread = ::CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)MessageThreadProc, this, NULL, &m_dwThreadId);
		WaitForSingleObject(m_hEvent, INFINITE);
		return TRUE;
	}

	static BOOL RegisterWindowClass()
	{
		WNDCLASSEX wcex;

		wcex.cbSize = sizeof(WNDCLASSEX);
		wcex.style			= CS_HREDRAW | CS_VREDRAW;
		wcex.lpfnWndProc	= WndProc;
		wcex.cbClsExtra		= 0;
		wcex.cbWndExtra		= 0;
		wcex.hInstance		= NULL;
		wcex.hIcon			= NULL;
		wcex.hCursor		= NULL;
		wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
		wcex.lpszMenuName	= NULL;
		wcex.lpszClassName	= L"MessageThreadWindow";
		wcex.hIconSm		= NULL;

		return RegisterClassEx(&wcex);
	}

	static LRESULT CALLBACK WndProc(HWND hWnd, UINT msg, WPARAM w, LPARAM l)
	{
		if (msg == RegisterWindowMessage(MTW_MSG))
		{
			OutputDebugString(L"1");
		}
		return DefWindowProc(hWnd, msg, w, l);
	}

	static DWORD WINAPI MessageThreadProc(LPVOID pVoid)
	{
		MessageThread* pThis = (MessageThread*)pVoid;

		RegisterWindowClass();
		HWND hWnd = CreateWindow(L"MessageThreadWindow", L"", NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL);
		pThis->m_hWnd = hWnd;
		SetEvent(pThis->m_hEvent);
		if (hWnd)
		{
			MSG msg;
			if (GetMessage(&msg, hWnd, 0, 0))
			{
				DispatchMessage(&msg);
			}
		}

		return 0;
	}
	
	LRESULT SendMessage(WPARAM w, LPARAM l)
	{
		return ::SendMessage(m_hWnd, RegisterWindowMessage(MTW_MSG), w, l);
	}


private:
	HANDLE	m_hEvent;
	HANDLE	m_hThread;
	DWORD	m_dwThreadId;
	HWND	m_hWnd;
};



#endif