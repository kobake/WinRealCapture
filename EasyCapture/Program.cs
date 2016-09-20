using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyCapture
{
	class Program
	{
		const int WH_KEYBOARD_LL = 13;
		const int WM_KEYDOWN = 0x0100;

		static LowLevelKeyboardProc _proc = HookCallback;
		static IntPtr _hookID = IntPtr.Zero;

		[DllImport("user32.dll")]
		static extern short GetAsyncKeyState(Keys vKey);

		[DllImport("user32.dll")]
		static extern short GetKeyState(Keys vKey);

		const int VK_F2 = 0x71;
		const int VK_F10 = 0x79;        // F10 キー
		const int VK_F11 = 0x7A;        // F11 キー
		const int VK_MULTIPLY = 0x6A;   // *　キー
		const int VK_CONTROL = 0x11;

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr SetWindowsHookEx(
			int idHook,
			LowLevelKeyboardProc lpfn,
			IntPtr hMod,
			uint dwThreadId
		);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr CallNextHookEx(
			IntPtr hhk,
			int nCode,
			IntPtr wParam,
			IntPtr lParam
		);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr GetModuleHandle(string lpModuleName);

		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}
		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);
				Keys k = (Keys)vkCode;
				if(k == Keys.F2)
				{
					short s = GetKeyState(Keys.ControlKey);
					bool ctrl = ((s & 0x8000) != 0);
					// Console.WriteLine("Ctrl:{0}, {1}, {2}, {3}", ctrl ? "1" : "0", Convert.ToString((int)lParam, 2), Convert.ToString((int)wParam, 2), Convert.ToString(s, 2).PadLeft(16, '0'));
					if (ctrl)
					{
						DoCapture();
					}
				}
			}
			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		


		static void DoCapture()
		{
			IntPtr hwnd = User32.GetForegroundWindow();

			StringBuilder title = new StringBuilder(1048);
			User32.GetWindowText(hwnd, title, 1024);
			Console.WriteLine(title.ToString());

			DoCapture(hwnd);
		}
		static void DoCapture(IntPtr hwnd)
		{
			ScreenCapture sc = new ScreenCapture();
			// capture entire screen, and save it to a file
			Image img = sc.CaptureScreen();

			// display image in a Picture control named imageDisplay
			//this.imageDisplay.Image = img;

			// ファイル名決定
			string fname = "";
			string fpath = "";
			bool ok = false;
			for (int i = 0; i <= 99; i++)
			{
				fname = "capt_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_") + i.ToString().PadLeft(2, '0') + ".png";
				fpath = "C:\\_tmp\\" + fname;
				if (!File.Exists(fpath))
				{
					ok = true;
					break;
				}
			}
			if (ok)
			{
				// capture this window, and save it
				sc.CaptureWindowToFile(hwnd, fpath, ImageFormat.Png);
				Console.WriteLine(fname);
			}
			else
			{
				Console.WriteLine("failed to decide filename");
			}
		}

		static void InitGd()
		{
			// 高解像度の縮小無効処理
			// http://stackoverflow.com/questions/13228185/how-to-configure-an-app-to-run-correctly-on-a-machine-with-a-high-dpi-setting-e/13228495#13228495
			if (Environment.OSVersion.Version.Major >= 6) User32.SetProcessDPIAware();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}
		static void Main4()
		{
			InitGd();

			DoCapture((IntPtr)0x000B09D0);
		}
		static void Main()
		{
			InitGd();

			var doWork = Task.Run(() =>
			{
				try
				{
					Thread.Sleep(Timeout.Infinite);
				}
				catch (ThreadInterruptedException)
				{
				}
				Application.Exit(); // Quick exit for demonstration only.
			});

			_hookID = SetHook(_proc);

			Application.Run();

			UnhookWindowsHookEx(_hookID);
		}
		static void Main2(string[] args)
		{
			
			while (true)
			{
				Thread.Sleep(1000);

				if (true)
				{
					short s = GetAsyncKeyState(Keys.F2);
					short s1 = GetAsyncKeyState(Keys.ControlKey);
					short s2 = GetKeyState(Keys.ControlKey);
					if (s != 0)
					{
						Console.WriteLine("F2 pushed: {0}", Convert.ToString(s, 2));
						Console.WriteLine("Ctrl_1: {0}", Convert.ToString(s1, 2));
						Console.WriteLine("Ctrl_2: {0}", Convert.ToString(s2, 2));
						Console.WriteLine("");
					}
				}
				
			}
		}
	}
}
