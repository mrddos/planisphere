
#ifndef __KXCANVASBUILDER_H__
#define __KXCANVASBUILDER_H__

#pragma once
#include "KxCanvas.h"

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

	void Render(HDC hDC)
	{
		if (m_bVisible)
		{
			KxLayer::Render(hDC);
		}
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

class KxCanvasBuilder
{


public:
	KxCanvasBuilder()
	{
		_AddDefaultLayer();
	}

	void Render(HDC hDC)
	{
		vector<KxLayerHolder*>::const_iterator const_iter = m_LayerHolderArray.begin();
		for (; const_iter != m_LayerHolderArray.end(); const_iter++)
		{
			KxLayerHolder* pLayerHolder = *const_iter;
			if (pLayerHolder)
			{
				pLayerHolder->Draw(hDC);
			}
		}
	}

public:

	void AddLayer(CString const& strLayerName)
	{
		KxLayerHolder* pLayerHolder = new KxLayerHolder(strLayerName);
		m_LayerHolderArray.push_back(pLayerHolder);
	}

	BOOL RemoveLayer(CString const& strLayerName)
	{
		return FALSE;
	}

	BOOL RemoveLayer(KxLayerHolder* pLayerHolder)
	{
		return FALSE;
	}

	KxLayerHolder* GetActiveLayerHolder()
	{
		vector<KxLayerHolder*>::const_iterator const_iter = m_LayerHolderArray.begin();
		for (; const_iter != m_LayerHolderArray.end(); const_iter++)
		{
			KxLayerHolder* pLayerHolder = *const_iter;
			if (pLayerHolder)
			{
				if (pLayerHolder->IsActived())
				{
					return pLayerHolder;
				}
			}
		}
		return NULL;
	}


	void ShowLayerHolder(KxLayerHolder* pLayerHolder, BOOL bShow)
	{
		pLayerHolder->SetVisible(bShow);
	}

public:
	void AddDnDImage(HBITMAP hBitmap, POINT const& point)
	{
		KxLayerHolder* pLayerHolder = GetActiveLayerHolder();
		if (pLayerHolder)
		{
			pLayerHolder->AddDnDImage(hBitmap, point);
		}
		else
		{
			OutputDebugString(L"NO Active Layer found.");
		}
	}

private:
	void _AddDefaultLayer()
	{
		AddLayer(L"Default Layer");
	}

private:
	vector<KxLayerHolder*> m_LayerHolderArray;

};


#endif