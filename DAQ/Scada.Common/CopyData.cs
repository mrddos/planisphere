using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Scada.Common
{
	/// <summary>
	/// 
	/// </summary>
    [StructLayout(LayoutKind.Sequential)] 
	public struct CopyDataStruct
	{
		public IntPtr dwData;
		public int cbData;

		[MarshalAs(UnmanagedType.LPStr)]
		public string lpData;
	}
}
