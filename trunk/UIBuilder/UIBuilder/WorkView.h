
#pragma once

#include "KxCanvasBuilder.h"

class CWorkView : public CWindowImpl<CWorkView>
{
public:
	CWorkView()
		:m_CanvasBuilder(NULL)
	{

	}

	BEGIN_MSG_MAP(CWorkView)
		MESSAGE_HANDLER(WM_PAINT, OnPaint)
		MSG_WM_CREATE(OnCreate)
		MESSAGE_HANDLER(WM_DROPFILES, OnDropFiles)
	END_MSG_MAP()

	static DWORD GetWndExStyle(DWORD dwStyle)
	{
		return dwStyle | WS_EX_ACCEPTFILES;
	}


	LRESULT OnPaint(UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/)
	{
		CPaintDC dc(*this);
		ATLASSERT(m_CanvasBuilder);
		if (dc)
		{
			
			m_CanvasBuilder->Render(dc);
		}
		
		
		return S_OK;
	}

	LRESULT OnCreate(LPCREATESTRUCT pCreateStruct)
	{

		m_CanvasBuilder = new KxCanvasBuilder();
		return S_OK;
	}

	LRESULT OnDropFiles(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
	{
		HDROP hFileDrop = (HDROP)wParam;
		if (hFileDrop)
		{
			WCHAR szFileName[MAX_PATH] = {};
			UINT nCount = DragQueryFile(hFileDrop, 0, szFileName, MAX_PATH);

			if (PathFileExists(szFileName))
			{
				POINT p;
				DragQueryPoint(hFileDrop, &p);

				HBITMAP hImage = NULL;

				CString fileName(szFileName);
				CString extFileName = fileName.Right(4);
				if (extFileName.CompareNoCase(L".png") == 0)
				{
					hImage = PngLoader::Load(szFileName);
				}
				else if (extFileName.CompareNoCase(L".bmp") == 0)
				{
					OutputDebugString(L".bmp file not be supported now.");
				}

				if (hImage)
				{
					m_CanvasBuilder->AddDnDImage(hImage, p);
				}
				

				InvalidateRect(NULL);
			}
		}
		return S_OK;
	}

private:
	KxCanvasBuilder* m_CanvasBuilder;
};