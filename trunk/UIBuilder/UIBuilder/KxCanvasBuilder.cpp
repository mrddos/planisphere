
#include "stdafx.h"
#include "KxCanvasBuilder.h"





void KxCanvasBuilder::Render(HDC hDC)
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

	// Draw Builder Layer...
	if (m_pBuilderLayerHolder)
	{
		m_pBuilderLayerHolder->Draw(hDC);
	}
}



