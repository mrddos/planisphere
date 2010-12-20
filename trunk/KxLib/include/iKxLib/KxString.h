
#ifndef __KxString_H__
#define __KxString_H__

#include <strsafe.h>

class KxString;
// inner
class __KxString
{
public:
	friend class KxString;
	__KxString(LPCWSTR lpStr, UINT nLen)
		:m_lpStr(lpStr), m_nLen(nLen)
	{

	}
private:
	LPCWSTR	m_lpStr;
	UINT	m_nLen;
};

// public
class KxString
{
public:
	KxString()
		:m_pStr(NULL), m_nLen(0)
	{
	}

	KxString(LPCWSTR lpStr)
		:m_pStr(NULL), m_nLen(0)
	{
		m_nLen = wcslen(lpStr);
		m_pStr = new WCHAR[m_nLen + 1];
		StringCchCopy(m_pStr, m_nLen + 1, lpStr);
	}

	KxString(LPCWSTR lpStr, UINT nFrom, UINT nCount)
		:m_pStr(NULL), m_nLen(0)
	{
		m_pStr = new WCHAR[nCount + 1];
		m_nLen = nCount;
		StringCchCopy(m_pStr, nCount + 1, lpStr + nFrom);
	}

	KxString(KxString const& other)
	{
		m_nLen = other.Length();
		m_pStr = new WCHAR[m_nLen + 1];
		StringCchCopy(m_pStr, m_nLen + 1, other);
	}

	KxString(__KxString const& temp)
	{
		m_pStr = (WCHAR*)temp.m_lpStr;
		m_nLen = temp.m_nLen;
	}

	KxString operator=(KxString const& other)
	{
		return KxString();
	}

	KxString operator=(__KxString const& temp)
	{
		
	}

	~KxString()
	{
		if (NULL != m_pStr)
			delete[] m_pStr;
	}

	UINT Length() const
	{
		return m_nLen;
	}

	KxString IndexOf(LPCWSTR) const
	{
		
	}

	KxString IndexOf(LPCWSTR, UINT nFrom) const
	{

	}

	KxString SubString(UINT nFrom, UINT nCount)
	{
		KxString ret(m_pStr, nFrom, nCount);
		
		__KxString temp(ret.Detach(), ret.Length());
		return temp;
	}

	operator LPCWSTR() const
	{
		return m_pStr;
	}

	bool operator==(LPCWSTR right) const
	{

		return true;
	}
private:
	LPCWSTR Detach()
	{
		WCHAR* pStr = m_pStr;
		m_pStr = NULL;
		m_nLen = 0;
		return pStr;
	}

private:
	WCHAR*	m_pStr;
	UINT	m_nLen;
};

#endif