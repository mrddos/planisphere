// Scada.FormProxy.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "tlhelp32.h"

#pragma warning(disable: 4996)

typedef struct tagWNDINFO
{
    DWORD	dwProcessId;
    HWND	hWnd;
} WNDINFO, *LPWNDINFO;



BOOL CALLBACK EnumProc(HWND hWnd, LPARAM lParam)
{
    DWORD dwProcID = 0;
    LPWNDINFO pInfo = (LPWNDINFO)lParam;
    GetWindowThreadProcessId(hWnd, &dwProcID);
    if(dwProcID == pInfo->dwProcessId)
    {
        // !!! first is not main
        HWND hParentWnd = NULL;
        while ((hParentWnd = GetParent(hWnd)) != NULL)
			hParentWnd = hWnd;
        pInfo->hWnd = hWnd;
        return FALSE;
    }
    return TRUE;
}


HWND __fastcall GetProcessMainWnd(DWORD dwProcessID)
{
    WNDINFO wi;
    wi.dwProcessId = dwProcessID;
    wi.hWnd = NULL;
    EnumWindows((WNDENUMPROC)EnumProc, (LPARAM)&wi);
    return wi.hWnd;
}

CString ExtractFileName(CString const& strPath)
{
	return L"";
}

DWORD __fastcall GetProcessID(CString const& strProcessName)
{
    PROCESSENTRY32 pe;
    pe.dwSize = sizeof(pe);
    HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    bool bContinue = Process32First(hSnapshot, &pe);
    while (bContinue)
    {
		if (strProcessName.CompareNoCase(pe.szExeFile) == 0)
		{
            return pe.th32ProcessID;
        }
        bContinue = Process32Next(hSnapshot, &pe);
    }
    return 0;
}

void WriteWindowInfo(CString const& strPathName, HWND hWnd)
{
	FILE* file = NULL;
	file = _wfopen(strPathName, L"w");
	char buffer[32] = {};
	int l = sprintf(buffer, "%d", (long)hWnd);
	fwrite(buffer, l, 1, file); 
	// TODO: Write.
	if (file)
	{
		fclose(file);
	}
}

///////////////////////////////////////////////////////////////
// TODO:
// Scada.FormProxy "Process.Name" "<PATH>\dest.cfg"
// 
int _tmain(int argc, _TCHAR* argv[])
{
	CString strProcName;
	CString strPathName;
	if (argc > 1)
	{
		strProcName = argv[1];
		strPathName = argv[2];
	}
	// MessageBox(NULL, strProcName, strPathName, 0);
	if (strProcName.GetLength() == 0)
	{
		strProcName = L"Scada.DAQ.DataForm.exe";
	}

	if (strPathName.GetLength() == 0)
	{
		strPathName = L"C:\\Users\\HealerKx\\Projects\\DAQ-Proj\\DAQ\\Bin\\Debug\\devices\\Scada.FormProxy\\0.9\\HWND.r";
	}

	DWORD dwProcId = GetProcessID(strProcName);

	
	// ----
	HWND hWnd = GetProcessMainWnd(dwProcId);
	WriteWindowInfo(strPathName, hWnd);
	return 0;
}

