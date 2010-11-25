
#ifndef __WIN32_FINDFILE_H__
#define __WIN32_FINDFILE_H__
#pragma once

#include <vector>
#include <deque>
using std::vector;
using std::deque;

class Win32FindFileData : public WIN32_FIND_DATAW
{
public:
	CString strFullFileName;
public:

};

class Win32FindFile
{
public:
	Win32FindFile(CString const& strPath, BOOL bReturnPath = TRUE, BOOL bRecursion = TRUE)
		: m_strPath(strPath), m_bReturnPath(bReturnPath), m_bRecursion(bRecursion), m_dwFilterFlag(0)
	{

	}


	void Include(CString const& strPattern)
	{
		m_strIncludePattern.push_back(strPattern);
	}

	void Exclude(CString const& strPattern)
	{
		m_strExcludePattern.push_back(strPattern);
	}

	BOOL Find(Win32FindFileData* pFindData)
	{
		if (m_findFileStack.empty())
		{
			if (_FindFirstFile(m_strPath, pFindData))
			{
				if (!_ValidPathFileName(pFindData->cFileName))
				{
					BOOL bFind = _FindNextMatchedFile(pFindData);
					if (bFind)
					{
						pFindData->strFullFileName = m_strCurrentPath + L"\\" + pFindData->cFileName;
						return TRUE;
					}
				}
			}
			return FALSE;
		}
		else
		{
			BOOL bFind = _FindNextMatchedFile(pFindData);
			if (bFind)
			{
				pFindData->strFullFileName = m_strCurrentPath + L"\\" + pFindData->cFileName;
				return TRUE;
			}
		}
		return FALSE;
	}



private:
	// Valid
	BOOL _FindFirstFile(CString const& strPath, Win32FindFileData* pFindData)
	{
		CString strPathToFind = strPath + L"\\*.*";
		HANDLE hFindFile = ::FindFirstFile(strPathToFind, pFindData);
		if (hFindFile != INVALID_HANDLE_VALUE)
		{
			m_strCurrentPath = strPath;
			m_findFileStack.push_back(hFindFile);
			m_pathStack.push_back(m_strCurrentPath);
			return TRUE;
		}
		return FALSE;
	}

	// Valid
	BOOL _FindNextMatchedFile(Win32FindFileData* pFindData)
	{
		while (!m_findFileStack.empty())
		{
			HANDLE hFindFile = m_findFileStack.back();
			BOOL bFind = ::FindNextFile(hFindFile, pFindData);
			if (bFind)
			{
				if (pFindData->dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
				{
					if (_ValidPathFileName(pFindData->cFileName))
					{
						if (m_bReturnPath && m_bRecursion)
						{
							Win32FindFileData finddata;
							CString strPathToFind = m_strCurrentPath + L"\\" + pFindData->cFileName;
							_FindFirstFile(strPathToFind, &finddata);

							return TRUE;
						}
						else if (m_bReturnPath)
						{
							return TRUE;
						}
						else if (m_bRecursion)
						{
							CString strPathToFind = m_strCurrentPath + L"\\" + pFindData->cFileName;
							_FindFirstFile(strPathToFind, pFindData);
						}
					}
				}
				else // File
				{
					BOOL bMatch = (m_dwFilterFlag == 0) ? TRUE : _Match(pFindData->cFileName);
					if (bMatch)
					{
						return TRUE;
					}
					else
					{
						continue;
					}
				}
			}
			else
			{
				m_findFileStack.pop_back();
				m_pathStack.pop_back();
				if (!m_pathStack.empty())
					m_strCurrentPath = m_pathStack.back();
				else
					return FALSE;
			}
		}
		return FALSE;
	}



private:
	BOOL _Match(CString const& strFileName)
	{
		return TRUE;
	}

	inline BOOL _ValidPathFileName(CString const& strPathFileName)
	{
		return 
			(strPathFileName != L".") &&
			(strPathFileName != L"..");
	}

private:
	CString			m_strPath;
	CString			m_strCurrentPath;
	BOOL			m_bReturnPath;
	BOOL			m_bRecursion;
	deque<HANDLE>	m_findFileStack;
	deque<CString>	m_pathStack;
private:
	// Too many pattern settings would make the application inefficient
	DWORD			m_dwFilterFlag;
	vector<CString>	m_strIncludePattern;
	vector<CString>	m_strExcludePattern;

};


#endif