
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
	{
		GetObject(m_hBitmap, sizeof(BITMAP), &m_bm);
	}

	KxImage(KxImage const& image)
	{
		m_hBitmap = image.m_hBitmap;
		CopyMemory(&m_bm, &image.m_bm, sizeof(BITMAP));
	}

	operator HBITMAP() const
	{
		return m_hBitmap;
	}

	LONG Width() const
	{
		return m_bm.bmWidth;
	}

	LONG Height() const
	{
		return m_bm.bmHeight;
	}

	BOOL Transparent() const
	{
		return m_bm.bmBitsPixel == 32;
	}

	

private:
	HBITMAP m_hBitmap;
	BITMAP	m_bm;
};


class KxImageDraw
{
public:
	void Draw(HDC hDC, RECT const& rect, KxImage const& image, BOOL bTransparent)
	{
		HDC hMemDC = CreateCompatibleDC(hDC);
		HANDLE hOldObject = SelectObject(hMemDC, (HBITMAP)image);

		if (bTransparent)
		{
			BLENDFUNCTION bf = {AC_SRC_OVER, 0, 0xFF, AC_SRC_ALPHA};
			AlphaBlend(hDC, rect.left, rect.top, KxRect::Width(rect), KxRect::Height(rect), hMemDC, 0, 0, image.Width(), image.Height(), bf);
		}
		else
		{
			::StretchBlt(hDC, rect.left, rect.top, KxRect::Width(rect), KxRect::Height(rect), hMemDC, 0, 0, image.Width(), image.Height(), SRCCOPY);
		}
		SelectObject(hMemDC, hOldObject);
		DeleteObject(hMemDC);
	}

private:
};


class KxImageBox : public KxDrawable, protected KxImageDraw
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
		m_Rect.right = m_Rect.left + m_Image.Width();
		m_Rect.bottom = m_Rect.top + m_Image.Height();

		KxImageDraw::Draw(hDC, m_Rect, m_Image, m_Image.Transparent());

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