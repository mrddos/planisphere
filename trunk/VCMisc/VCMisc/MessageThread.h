
#ifndef __MESSAGETHREAD_H__
#define __MESSAGETHREAD_H__


class MessageThread
{
public:
	MessageThread()
		:m_hThread(NULL), m_dwThreadId(0)
	{

	}

	BOOL Create()
	{
		m_hThread = ::CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)MessageThreadProc, this, NULL, &m_dwThreadId);
		return FALSE;
	}

	BOOL MessageLoop()
	{
		const LPCWSTR MESSAGETHREADWINDOWCLASS = L"MessageThreadWindow";
		WNDCLASSEX wcex;
		wcex.cbSize = sizeof(WNDCLASSEX);
		wcex.style			= 0;
		wcex.lpfnWndProc	= WndProc;
		wcex.cbClsExtra		= 0;
		wcex.cbWndExtra		= 0;
		wcex.hInstance		= NULL;
		wcex.hIcon			= NULL;
		wcex.hCursor		= NULL;
		wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW + 1);
		wcex.lpszMenuName	= NULL;
		wcex.lpszClassName	= MESSAGETHREADWINDOWCLASS;
		wcex.hIconSm		= NULL;

		if (!RegisterClassEx(&wcex))
			return FALSE;
		HWND hWnd = CreateWindowEx(NULL, MESSAGETHREADWINDOWCLASS, NULL, NULL, 0, 0, 0, 0, HWND_MESSAGE, NULL, NULL, NULL);
		if (hWnd)
		{
			MSG msg;
			while (GetMessage(&msg, hWnd, 0, 0))
			{
				DispatchMessage(&msg);
			}
			return TRUE;
		}
		return FALSE;
	}



	static DWORD WINAPI MessageThreadProc(LPVOID pVoid)
	{
		MessageThread* pThis = (MessageThread*)pVoid;
		pThis->MessageLoop();
		return 0;
	}
	
	LRESULT SendMessage(WPARAM w, LPARAM l)
	{
		return ::SendMessage(m_hWnd, RegisterWindowMessage(MTW_MSG), w, l);
	}

	LRESULT StartIdleWork()
	{

		return 0;
	}
private:
	

	static LRESULT CALLBACK WndProc(HWND hWnd, UINT msg, WPARAM w, LPARAM l)
	{
		if (msg == RegisterWindowMessage(MTW_MSG))
		{
			ReplyMessage(0);
			Sleep(10000);
		}
		return DefWindowProc(hWnd, msg, w, l);
	}


private:
	HANDLE	m_hEvent;
	HANDLE	m_hThread;
	DWORD	m_dwThreadId;
	HWND	m_hWnd;
};



#endif