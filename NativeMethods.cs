using System;
using System.Runtime.InteropServices;

namespace QuickGenerator
{
	class NativeMethods
	{
		[DllImport("user32.dll")]
		public extern static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

		[DllImport("User32.dll")]
		public static extern int SetClipboardViewer(int hWndNewViewer);


		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);


		private NativeMethods()
		{
		}
	}
}
