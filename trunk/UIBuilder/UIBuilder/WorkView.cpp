
#include "stdafx.h"

#include "WorkView.h"

LRESULT CWorkView::OnHRulerHover( UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/ )
{
	if (m_CanvasBuilder)
	{
		m_CanvasBuilder->ShowGridBuilderLine(TRUE, 1, (LONG)wParam);
	}
	return S_OK;
}

LRESULT CWorkView::OnVRulerHover( UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/ )
{
	if (m_CanvasBuilder)
	{
		m_CanvasBuilder->ShowGridBuilderLine(TRUE, 2, (LONG)wParam);
	}

	return S_OK;
}

LRESULT CWorkView::OnHRulerClick( UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/ )
{

	return S_OK;
}

LRESULT CWorkView::OnVRulerClick( UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/ )
{

	return S_OK;
}