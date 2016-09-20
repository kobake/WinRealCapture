using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyCapture
{


	/// <summary>
	/// Helper class containing User32 API functions
	/// </summary>
	public class Win32
	{
		public const int WH_KEYBOARD_LL = 13;
		public const int WM_KEYDOWN = 0x0100;

		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(Keys vKey);

		[DllImport("user32.dll")]
		public static extern short GetKeyState(Keys vKey);

		public const int VK_F2 = 0x71;
		public const int VK_F10 = 0x79;        // F10 キー
		public const int VK_F11 = 0x7A;        // F11 キー
		public const int VK_MULTIPLY = 0x6A;   // *　キー
		public const int VK_CONTROL = 0x11;

		public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(
			int idHook,
			LowLevelKeyboardProc lpfn,
			IntPtr hMod,
			uint dwThreadId
		);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CallNextHookEx(
			IntPtr hhk,
			int nCode,
			IntPtr wParam,
			IntPtr lParam
		);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);



		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool SetProcessDPIAware();

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int length);
	}
}
