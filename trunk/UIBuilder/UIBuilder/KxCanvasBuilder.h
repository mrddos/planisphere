
#ifndef __KXCANVASBUILDER_H__
#define __KXCANVASBUILDER_H__

#pragma once

class KxHolder : public KxDrawable
{
public:
	BOOL IsActived()
	{
		return m_bActived;
	}
	BOOL Actived(BOOL bActived = TRUE)
	{
		m_bActived = bActived;
	}

private:
	BOOL m_bActived;
};

class KxImageBoxHolder : public KxHolder, protected KxImageBox
{
public:
	virtual void Draw(HDC hDC)
	{
		KxImageBox::Draw(hDC);
	}


private:
};


class KxTextBoxHolder : public KxHolder, public KxTextBox
{
public:
	virtual void Draw(HDC hDC)
	{
		KxTextBox::Draw(hDC);
	}

private:
};


class KxLayerWrapper : public KxLayer
{
public:
	KxLayer()
		:m_bVisible(TRUE)
	{

	}

	void Render(HDC hDC)
	{
		if (m_bVisible)
		{
			KxLayer::Render(hDC);
		}
	}

private:
	std::vector<KxDrawable*> m_vecDrawable;
	BOOL m_bVisible;
};

class KxCanvasBuilder
{


public:


private:

};


#endif