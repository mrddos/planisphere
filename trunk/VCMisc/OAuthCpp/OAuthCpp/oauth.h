
#ifndef __OAUTH_H__
#define __OAUTH_H__
#pragma once

enum OAuth_Signature_Method
{
	HMAC_SHA1 = 0,
	RSA_SHA1 = 1,
	PLAINTEXT = 2
};

class OAuth_Consumer_Key
{
public:

	operator ATL::CString()
	{
		return m_strKey;
	}
private:
	ATL::CString m_strKey;
};

class OAuth_Consumer_Secret
{
public:
	operator ATL::CString()
	{
		return m_strSecret;
	}
private:
	ATL::CString m_strSecret;
};


#endif