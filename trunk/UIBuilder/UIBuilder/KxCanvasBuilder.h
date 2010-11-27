
#ifndef __KXCANVASBUILDER_H__
#define __KXCANVASBUILDER_H__

#pragma once
#include "KxCanvasBuilder_i.h"

class KxCanvasBuilder : public KxCanvas
{
public:

public:
	KxCanvasBuilder()
		:m_pBuilderLayerHolder(NULL)
	{
		KxImageDraw::Init();
		_AddDefaultLayer();
	}

	void Render(HDC hDC);


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

	void ShowGridLines(BOOL bShowGridLines)
	{
		if (m_pBuilderLayerHolder)
		{
			m_pBuilderLayerHolder->ShowGridLines(bShowGridLines);
		}
	}

	void ShowGridBuilderLine(BOOL bShowGridBuilderLine, UINT nGridBuilderLineDirect /* 1: Vertical, 2: Horizontal */, LONG nGridBuilderLinePos)
	{
		if (m_pBuilderLayerHolder)
		{
			m_pBuilderLayerHolder->ShowGridBuilderLine(bShowGridBuilderLine, nGridBuilderLineDirect, nGridBuilderLinePos);
		}
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
		// As the background
		AddLayer(L"Default-Layer");

		// As the foreground, show grid lines...
		m_pBuilderLayerHolder = new KxBuilderLayerHolder();

	}

private:
	vector<KxLayerHolder*>	m_LayerHolderArray;


private:
	KxBuilderLayerHolder*	m_pBuilderLayerHolder;

};


#endif