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
		static Win32.LowLevelKeyboardProc _proc = HookCallback;
		static IntPtr _hookID = IntPtr.Zero;


		private static IntPtr SetHook(Win32.LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, proc, Win32.GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)Win32.WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);
				Keys k = (Keys)vkCode;
				if(k == Keys.F2)
				{
					short s = Win32.GetKeyState(Keys.ControlKey);
					bool ctrl = ((s & 0x8000) != 0);
					// Console.WriteLine("Ctrl:{0}, {1}, {2}, {3}", ctrl ? "1" : "0", Convert.ToString((int)lParam, 2), Convert.ToString((int)wParam, 2), Convert.ToString(s, 2).PadLeft(16, '0'));
					if (ctrl)
					{
						DoCapture();
					}
				}
			}
			return Win32.CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		


		static void DoCapture()
		{
			IntPtr hwnd = Win32.GetForegroundWindow();

			StringBuilder title = new StringBuilder(1048);
			Win32.GetWindowText(hwnd, title, 1024);
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
			if (Environment.OSVersion.Version.Major >= 6) Win32.SetProcessDPIAware();
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

			Win32.UnhookWindowsHookEx(_hookID);
		}
		static void Main2(string[] args)
		{
			
			while (true)
			{
				Thread.Sleep(1000);

				if (true)
				{
					short s = Win32.GetAsyncKeyState(Keys.F2);
					short s1 = Win32.GetAsyncKeyState(Keys.ControlKey);
					short s2 = Win32.GetKeyState(Keys.ControlKey);
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
