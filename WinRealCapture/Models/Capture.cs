using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32WrapLib;

namespace WinRealCapture.Models
{
    public class Capture
    {
        public static string CaptureActiveWindow(string savingDirectory)
        {
            IntPtr hwnd = Win32Wrap.GetForegroundWindow();
            return DoCapture(hwnd, savingDirectory);
        }

        public static string DoCapture(IntPtr hwnd, string savingDirectory)
        {
            ScreenCapture sc = new ScreenCapture();

            // ファイル名決定
            string fname = "";
            string fpath = "";
            bool ok = false;
            for (int i = 0; i <= 99; i++)
            {
                fname = "capt_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_") + i.ToString().PadLeft(2, '0') + ".png";
                fpath = Path.Combine(savingDirectory, fname);
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
                Debug.WriteLine("{0} ({1})", fname, title.ToString());

                // ファイル名を返す
                return fpath;
            }
            else
            {
                throw new Exception("Failed to decide filename");
            }
        }
    }
}
