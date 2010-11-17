// UIBuilderView.h : interface of the CUIBuilderView class
//
/////////////////////////////////////////////////////////////////////////////

#pragma once

#include "WorkView.h"

class CUIBuilderView : public CDialogImpl<CUIBuilderView>
{
public:
	enum { IDD = IDD_UIBUILDER_FORM };

	BOOL PreTranslateMessage(MSG* pMsg)
	{
		return CWindow::IsDialogMessage(pMsg);
	}



	BEGIN_MSG_MAP(CUIBuilderView)
		MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)
	END_MSG_MAP()



	LRESULT OnInitDialog(UINT, WPARAM, LPARAM, BOOL& bHandled)
	{
		CWorkView* pWorkView = new CWorkView();
		RECT r;
		r.bottom = 500;
		r.top = 100;
		r.left = 100;
		r.right = 800;
		pWorkView->Create(this->m_hWnd, r);


		return S_OK;
	}

// Handler prototypes (uncomment arguments if needed):
//	LRESULT MessageHandler(UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& /*bHandled*/)
//	LRESULT CommandHandler(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
//	LRESULT NotifyHandler(int /*idCtrl*/, LPNMHDR /*pnmh*/, BOOL& /*bHandled*/)



private:
	RECT m_WorkRect;
};
