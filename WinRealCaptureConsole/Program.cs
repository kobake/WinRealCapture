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
using Win32WrapLib;

namespace EasyCapture
{
    public class Program
    {
        static Win32Wrap.LowLevelKeyboardProc _proc = HookCallback;
        static IntPtr _hookID = IntPtr.Zero;


        private static IntPtr SetHook(Win32Wrap.LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return Win32Wrap.SetWindowsHookEx(Win32Wrap.WH_KEYBOARD_LL, proc, Win32Wrap.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)Win32Wrap.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys k = (Keys)vkCode;
                if(k == Keys.F2)
                {
                    short s = Win32Wrap.GetKeyState(Keys.ControlKey);
                    bool ctrl = ((s & 0x8000) != 0);
                    // Console.WriteLine("Ctrl:{0}, {1}, {2}, {3}", ctrl ? "1" : "0", Convert.ToString((int)lParam, 2), Convert.ToString((int)wParam, 2), Convert.ToString(s, 2).PadLeft(16, '0'));
                    if (ctrl)
                    {
                        DoCapture();
                    }
                }
            }
            return Win32Wrap.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        


        static void DoCapture()
        {
            IntPtr hwnd = Win32Wrap.GetForegroundWindow();
            DoCapture(hwnd);
        }
        static void DoCapture(IntPtr hwnd)
        {
            ScreenCapture sc = new ScreenCapture();
            
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

                // title取得
                StringBuilder title = new StringBuilder(1048);
                Win32Wrap.GetWindowText(hwnd, title, 1024);

                // ファイル名表示
                Console.WriteLine("{0} ({1})", fname, title.ToString());
            }
            else
            {
                Console.WriteLine("failed to decide filename");
            }
        }

        
        static void Main()
        {
            Gd.InitGd();

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

            Win32Wrap.UnhookWindowsHookEx(_hookID);
        }
        
    }
}
