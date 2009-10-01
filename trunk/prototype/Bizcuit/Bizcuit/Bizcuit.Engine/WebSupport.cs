using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bizcuit.Engine.WebSupport
{
	internal class WebRender
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		internal string LoadFromSource(string path)
		{
			// TODO: the root path should be get from config.file
			string filePath = "web/root/actions/" + path;
			FileStream fs = new FileStream(filePath, FileMode.Open);

			byte[] bs = new byte[fs.Length];
			fs.Read(bs, 0, (int)fs.Length);
			return Encoding.Default.GetString(bs);

		}
	}
}
