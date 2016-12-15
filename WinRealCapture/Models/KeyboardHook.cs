using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32WrapLib;

namespace WinRealCapture.Models
{
    class KeyboardHook
    {
        static Win32Wrap.LowLevelKeyboardProc _proc = HookCallback;
        static IntPtr _hookID = IntPtr.Zero;
        static Action m_f2action;

        public static void StartHook(Action f2action)
        {
            m_f2action = f2action;
            SetHook(_proc);
        }
        public static void EndHook()
        {
            Win32Wrap.UnhookWindowsHookEx(_hookID);
        }

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
                if (k == Keys.F2)
                {
                    short s = Win32Wrap.GetKeyState(Keys.ControlKey);
                    bool ctrl = ((s & 0x8000) != 0);
                    // Console.WriteLine("Ctrl:{0}, {1}, {2}, {3}", ctrl ? "1" : "0", Convert.ToString((int)lParam, 2), Convert.ToString((int)wParam, 2), Convert.ToString(s, 2).PadLeft(16, '0'));
                    if (ctrl)
                    {
                        m_f2action();
                    }
                }
            }
            return Win32Wrap.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
