using System;
using System.Collections.Generic;
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
        public static void DoCapture()
        {
            IntPtr hwnd = Win32Wrap.GetForegroundWindow();
            DoCapture(hwnd);
        }

        public static void DoCapture(IntPtr hwnd)
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
    }
}
