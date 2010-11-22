// UIBuilderView.h : interface of the CUIBuilderView class
//
/////////////////////////////////////////////////////////////////////////////

#pragma once

#include "msgdef.h"
#include "WorkView.h"
#include "Ruler.h"

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
		r.bottom = 480;
		r.top = 20;
		r.left = 40;
		r.right = 850;
		pWorkView->Create(this->m_hWnd, r);


		RECT hr;
		VRuler::GetRect(r, hr);
		VRuler* pVRuler = new VRuler(pWorkView->m_hWnd);
		pVRuler->Create(this->m_hWnd, hr);

		RECT vr;
		HRuler::GetRect(r, vr);
		HRuler* pHRuler = new HRuler(pWorkView->m_hWnd);
		pHRuler->Create(this->m_hWnd, vr);

		return S_OK;
	}

// Handler prototypes (uncomment arguments if needed):
//	LRESULT MessageHandler(UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& /*bHandled*/)
//	LRESULT CommandHandler(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
//	LRESULT NotifyHandler(int /*idCtrl*/, LPNMHDR /*pnmh*/, BOOL& /*bHandled*/)



private:
	RECT m_WorkRect;
};
