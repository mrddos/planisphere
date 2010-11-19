
#ifndef __KXCANVAS_H__
#define __KXCANVAS_H__

#pragma once

#include <vector>
using std::vector;

class KxRect
{
public:
	static LONG Width(RECT const& r)
	{
		return r.right - r.left;
	}

	static LONG Height(RECT const& r)
	{
		return r.bottom - r.top;
	}
};

/* Double Buffering...
class KxMemoryDC : public CDC
{
public:

private:

};
*/

class KxDrawable
{
public:
	virtual void Draw(HDC hDC) = 0;
};


class KxImage
{
public:
	KxImage(){}

	KxImage(HBITMAP hBitmap)
		:m_hBitmap(hBitmap)
	{}

	KxImage(KxImage const& image)
	{
		m_hBitmap = image.m_hBitmap;
	}

	operator HBITMAP()
	{
		return m_hBitmap;
	}

private:
	HBITMAP m_hBitmap;
};

class KxImageBox : public KxDrawable
{
public:
	KxImageBox()
	{

	}

	KxImageBox(KxImage const& image, POINT const& point)
		:m_Image(image)
	{
		m_Rect.left = point.x - 10;
		m_Rect.top = point.y - 10;
	}

	virtual void Draw(HDC hDC)
	{
		HDC hMemDC = CreateCompatibleDC(hDC);
		HANDLE hOldObject = SelectObject(hMemDC, (HBITMAP)m_Image);
		BITMAP bm;
		GetObject((HBITMAP)m_Image, sizeof(BITMAP), &bm);

		m_Rect.right = m_Rect.left + bm.bmWidth;
		m_Rect.bottom = m_Rect.top + bm.bmHeight;
		BLENDFUNCTION bf = {AC_SRC_OVER, 0, 0xFF, AC_SRC_ALPHA};
		AlphaBlend(hDC, m_Rect.left, m_Rect.top, KxRect::Width(m_Rect), KxRect::Height(m_Rect), hMemDC, 0, 0, bm.bmWidth, bm.bmHeight, bf);

		SelectObject(hMemDC, hOldObject);
		DeleteObject(hMemDC);
	}

private:
	KxImage m_Image;
	RECT	m_Rect;
};


class KxTextBox : public KxDrawable
{
public:
	KxTextBox()
	{
		
	}

	KxTextBox(CString const& strTest)
		:m_strText(strTest)
	{
	}

	void SetTextFormat(CString const& strTextFormat)
	{
		m_strTextFormat = strTextFormat;
	}


	virtual void Draw(HDC hDC)
	{

	}



private:
	BOOL	m_bWordWrap;
	CString m_strText;
	CString m_strTextFormat;
	RECT	m_Rect;
	DWORD	m_dwAlignment;

};

enum GridRowFlag
{
	GridRow_Fixed,
	GridRow_ByPercent,
	GridRow_Remaining,

};

enum GridColumnFlag
{
	GridColumn_Fixed,
	GridColumn_ByPercent,
	GridColumn_Remaining,

};

class KxGridRow
{
public:
	

private:
	UINT	m_nFlag;
	DWORD	m_dwHeight;
	DWORD	m_dwPercent;
};

class KxGridColumn
{
public:

private:
	UINT	m_nFlag;
	DWORD	m_dwWidth;
	DWORD	m_dwPercent;
};

class KxGridLayout
{
public:
	KxGridLayout()
	{

	}


private:
	std::vector<KxGridRow>		m_vecRow;
	std::vector<KxGridColumn>	m_vecCol;
};

class KxLayer
{
public:
	KxLayer()
	{

	}

	void Render(HDC hDC)
	{
		std::vector<KxDrawable*>::const_iterator const_iter = m_vecDrawable.begin();
		for (; const_iter != m_vecDrawable.end(); const_iter++)
		{
			KxDrawable* pDrawable = *const_iter;
			if (pDrawable)
			{
				pDrawable->Draw(hDC);
			}
		}
	}

private:
	std::vector<KxDrawable*> m_vecDrawable;
};




class KxCanvas
{
public:
	KxCanvas()
	{

	}
public:
	void Render()
	{
		if (m_pLayer)
			m_pLayer->Render(m_hDC);


		if (m_pTextLayer)
			m_pTextLayer->Render(m_hDC);
	}


private:
	HDC		m_hDC;
	KxLayer* m_pLayer;		//background
	KxLayer* m_pTextLayer;	//Text

	KxGridLayout	m_Layout;
};

#endif