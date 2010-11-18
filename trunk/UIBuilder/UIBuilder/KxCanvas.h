
#ifndef __KXCANVAS_H__
#define __KXCANVAS_H__

#pragma once

#include <vector>

class KxDrawable
{
public:
	virtual void Draw(HDC hDC) = 0;
};


class KxImage
{

};

class KxImageBox : public KxDrawable
{
public:


	virtual void Draw(HDC hDC)
	{

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

enum
{
	GridRow_LeftAlignment,
	GridRow_RightAlignment,
};

class KxGridRow
{
	
private:
	UINT	m_nFlag;
	DWORD	m_dwHeight;
	DWORD	m_dwPercent;
};

class KxGridColumn
{

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