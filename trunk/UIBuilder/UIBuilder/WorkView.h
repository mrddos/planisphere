
#pragma once

#include "KxCanvas.h"

class CWorkView : public CWindowImpl<CWorkView>
{

	BEGIN_MSG_MAP(CWorkView)
		MSG_WM_PAINT(OnPaint)
	END_MSG_MAP()

	static DWORD GetWndExStyle(DWORD dwStyle)
	{
		return dwStyle | WS_EX_ACCEPTFILES;
	}


	LRESULT OnPaint(HDC hDC)
	{

		return S_OK;
	}
};