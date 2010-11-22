
#ifndef __RULER_H__
#define __RULER_H__
#pragma once

#include "msgdef.h"

class Ruler : public CWindowImpl<Ruler>
{
public:
	enum { Ruler_Width = 14 };

	Ruler(HWND m_hViewWnd)
		:m_hViewWnd(m_hViewWnd)
	{

	}

	inline void ClickRuler(int msg, DWORD dwValue)
	{
		ATLASSERT(msg == MSG_HRULER_CLCIK || msg == MSG_VRULER_CLCIK);
		::SendMessage(m_hViewWnd, msg, dwValue, 0);
	}

	inline void RulerHover(int msg, DWORD dwValue)
	{
		ATLASSERT(msg == MSG_HRULER_HOVER || msg == MSG_VRULER_HOVER);
		::SendMessage(m_hViewWnd, msg, dwValue, 0);
	}

private:
	HWND m_hViewWnd;
};


class HRuler : public Ruler
{
	BEGIN_MSG_MAP(HRuler)
		MESSAGE_HANDLER(WM_PAINT, OnPaint)
		MESSAGE_HANDLER(WM_MOUSEMOVE, OnMouseMove)
		MESSAGE_HANDLER(WM_LBUTTONDOWN, OnLButtonDown)
	END_MSG_MAP()

	LRESULT OnPaint(UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/)
	{
		CPaintDC dc(*this);

		return S_OK;
	}

	LRESULT OnLButtonDown(UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/)
	{
		DWORD dwValue = GetMessagePos();
		POINTS p = MAKEPOINTS(dwValue);
		POINT pt;
		pt.x = p.x;
		pt.y = p.y;
		ScreenToClient(&pt);
		ClickRuler(MSG_HRULER_CLCIK, pt.x);
		return S_OK;
	}

	LRESULT OnMouseMove(UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/)
	{
		DWORD dwValue = GetMessagePos();
		POINTS p = MAKEPOINTS(dwValue);
		POINT pt;
		pt.x = p.x;
		pt.y = p.y;
		ScreenToClient(&pt);
		RulerHover(MSG_HRULER_HOVER, pt.x);
		return S_OK;
	}

	static void GetRect(RECT const& r, RECT& hr)
	{
		hr = r;
		hr.top = r.bottom + 3;
		hr.bottom = hr.top + Ruler::Ruler_Width;
	}

public:
	HRuler(HWND m_hViewWnd)
		:Ruler(m_hViewWnd)
	{

	}

};


class VRuler : public Ruler
{
	BEGIN_MSG_MAP(VRuler)
		MESSAGE_HANDLER(WM_PAINT, OnPaint)
		MESSAGE_HANDLER(WM_MOUSEMOVE, OnMouseMove)
		MESSAGE_HANDLER(WM_LBUTTONDOWN, OnLButtonDown)
	END_MSG_MAP()

	LRESULT OnPaint(UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/)
	{
		CPaintDC dc(*this);


		return S_OK;
	}

	LRESULT OnLButtonDown(UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/)
	{
		DWORD dwValue = GetMessagePos();
		POINTS p = MAKEPOINTS(dwValue);
		POINT pt;
		pt.x = p.x;
		pt.y = p.y;
		ScreenToClient(&pt);
		ClickRuler(MSG_VRULER_CLCIK, pt.y);
		return S_OK;
	}

	LRESULT OnMouseMove(UINT /*uMsg*/, WPARAM wParam, LPARAM lParam, BOOL& /*bHandled*/)
	{
		DWORD dwValue = GetMessagePos();
		POINTS p = MAKEPOINTS(dwValue);
		POINT pt;
		pt.x = p.x;
		pt.y = p.y;
		ScreenToClient(&pt);
		RulerHover(MSG_VRULER_HOVER, pt.y);
		return S_OK;
	}
	static void GetRect(RECT const& r, RECT& vr)
	{
		vr = r;
		vr.right = r.left - 3;
		vr.left = vr.right - Ruler::Ruler_Width;
	}


	VRuler(HWND m_hViewWnd)
		:Ruler(m_hViewWnd)
	{

	}
};


#endif