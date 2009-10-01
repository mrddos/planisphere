using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizcuit.Common
{
	public interface IBizActionDigest
	{

		string Name { get; }

		string ClassName { get; set; }

		void SetNextActionOnCondition(string condition, string nextActionName);

		void SetReturnContentType(string contentType);
		void SetReturnContent(string contentValue);
		void SetReturnContentSource(string contentSrc);


		string GetReturnContentType();
		string GetReturnContent();
		string GetReturnContentSource();
	}
}
