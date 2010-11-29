
#ifndef __KXCANVASBUILDER_I_H__
#define __KXCANVASBUILDER_I_H__

#include "KxCanvas.h"

enum
{
	LayerType_Default,

	LayerType_Builder,
};

class KxHolder : public KxDrawable
{
public:
	KxHolder()
	{

	}

	BOOL IsActived()
	{
		return m_bActived;
	}

	void SetActived(BOOL bActived = TRUE)
	{
		m_bActived = bActived;
	}

	BOOL IsVisible()
	{
		return m_bVisible;
	}

	BOOL SetVisible(BOOL bVisible)
	{
		m_bVisible = bVisible;
	}

private:
	BOOL m_bActived;
	BOOL m_bVisible;
};

class KxImageBoxHolder : public KxHolder, protected KxImageBox
{
public:
	KxImageBoxHolder(KxImage const& image, POINT const& point)
		:KxImageBox(image, point)
	{

	}

	virtual void Draw(HDC hDC)
	{
		KxImageBox::Draw(hDC);
	}


private:
};


class KxTextBoxHolder : public KxHolder, protected KxTextBox
{
public:
	virtual void Draw(HDC hDC)
	{
		KxTextBox::Draw(hDC);
	}

private:
};


class KxLayerHolder : public KxHolder, protected KxLayer
{
public:
	KxLayerHolder(CString const& strLayerName)
		:m_bVisible(TRUE), m_strLayerName(strLayerName)
	{

	}


	virtual void Draw(HDC hDC)
	{
		if (!IsVisible())
			return;

		vector<KxHolder*>::const_iterator const_iter = m_vecDrawable.begin();
		for (; const_iter != m_vecDrawable.end(); const_iter++)
		{
			KxHolder* pHolder = *const_iter;
			if (pHolder)
			{
				if (pHolder)
				{
					pHolder->Draw(hDC);
				}
			}
		}
	}

	void AddDnDImage(HBITMAP hBitmap, POINT const& point)
	{
		KxImage image(hBitmap);
		KxImageBoxHolder* pImageBoxHolder = new KxImageBoxHolder(image, point);
		if (pImageBoxHolder)
		{
			m_vecDrawable.push_back(pImageBoxHolder);
		}

	}

private:
	std::vector<KxHolder*> m_vecDrawable;
	BOOL		m_bVisible;
	CString		m_strLayerName;
};


class KxBuilderLayerHolder : public KxLayerHolder
{
public:
	KxBuilderLayerHolder()
		:KxLayerHolder(L"Builder-Layer")
	{

	}

	virtual void Draw(HDC hDC)
	{
		if (!IsVisible())
			return;

		// Draw Grid-lines
		if (m_bShowGridLines)
		{

		}

		// Draw Grid builder line
		if (m_bShowGridBuilderLine)
		{
			if (1 == m_nGridBuilderLineDirect)
			{

			}
			else if (2 == m_nGridBuilderLineDirect)
			{

			}
		}

		
	}

	void ShowGridLines(BOOL bShowGridLines)
	{
		m_bShowGridLines = bShowGridLines;
	}

	void ShowGridBuilderLine(BOOL bShowGridBuilderLine, UINT nGridBuilderLineDirect /* 1: Vertical, 2: Horizontal */, LONG nGridBuilderLinePos)
	{
		m_bShowGridBuilderLine = bShowGridBuilderLine;
		m_nGridBuilderLineDirect = nGridBuilderLineDirect;
		m_nGridBuilderLinePos = nGridBuilderLinePos;
	}
private:
	BOOL					m_bShowGridLines;
	BOOL					m_bShowGridBuilderLine;
	UINT					m_nGridBuilderLineDirect;
	LONG					m_nGridBuilderLinePos;
};


#endif