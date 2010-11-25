
#ifndef __MESSAGETHREAD_H__
#define __MESSAGETHREAD_H__


#define MTW_MSG L"MTW_MSG"

class MessageThread
{
public:
	MessageThread()
		:m_hEvent(NULL), m_hThread(NULL), m_dwThreadId(0)
	{

	}

	BOOL Create()
	{
		m_hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
		if (m_hEvent)
		{
			m_hThread = ::CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)MessageThreadProc, this, NULL, &m_dwThreadId);
			if (m_hThread)
			{
				WaitForSingleObject(m_hEvent, INFINITE);
				return TRUE;
			}
		}
		return FALSE;
	}

	BOOL MessageLoop()
	{
		const LPCWSTR MessageThreadWindowClass = L"MessageThreadWindow";
		WNDCLASSEX wcex;
		wcex.cbSize = sizeof(WNDCLASSEX);
		wcex.style			= CS_HREDRAW | CS_VREDRAW;
		wcex.lpfnWndProc	= WndProc;
		wcex.cbClsExtra		= 0;
		wcex.cbWndExtra		= 0;
		wcex.hInstance		= NULL;
		wcex.hIcon			= NULL;
		wcex.hCursor		= NULL;
		wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW + 1);
		wcex.lpszMenuName	= NULL;
		wcex.lpszClassName	= MessageThreadWindowClass;
		wcex.hIconSm		= NULL;

		if (!RegisterClassEx(&wcex))
			return FALSE;
		HWND hWnd = CreateWindow(MessageThreadWindowClass, L"", NULL, 0, 0, 0, 0, NULL, NULL, NULL, NULL);
		if (hWnd)
		{
			SetEvent(m_hEvent);
			CloseHandle(m_hEvent);
			m_hEvent = NULL;
			m_hWnd = hWnd;
			MSG msg;
			while (GetMessage(&msg, hWnd, 0, 0)){}

			return TRUE;
		}
		return FALSE;
	}

	static LRESULT CALLBACK WndProc(HWND hWnd, UINT msg, WPARAM w, LPARAM l)
	{
		if (msg == RegisterWindowMessage(MTW_MSG))
		{
			ReplyMessage(0);
			Sleep(10000);
		}
		return DefWindowProc(hWnd, msg, w, l);
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
	HANDLE	m_hEvent;
	HANDLE	m_hThread;
	DWORD	m_dwThreadId;
	HWND	m_hWnd;
};



#endif