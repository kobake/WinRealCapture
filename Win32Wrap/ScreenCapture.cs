using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Win32WrapLib;

namespace Win32WrapLib
{
    public class ScreenCapture
    {
        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        public Image CaptureScreen()
        {
            return CaptureWindow(Win32Wrap.GetDesktopWindow());
        }

        // http://dobon.net/vb/dotnet/graphics/screencapture.html
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        
        public static Bitmap CaptureScreen2()
        {
            
            //プライマリモニタのデバイスコンテキストを取得
            IntPtr disDC = GetDC(IntPtr.Zero);
            //Bitmapの作成
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);
            //Graphicsの作成
            Graphics g = Graphics.FromImage(bmp);
            //Graphicsのデバイスコンテキストを取得
            IntPtr hDC = g.GetHdc();
            //Bitmapに画像をコピーする
            Win32Wrap.BitBlt(hDC, 0, 0, bmp.Width, bmp.Height,
                disDC, 0, 0, Win32Wrap.SRCCOPY);
            //解放
            g.ReleaseHdc(hDC);
            g.Dispose();
            Win32Wrap.ReleaseDC(IntPtr.Zero, disDC);

            return bmp;
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public Image CaptureWindow(IntPtr handle)
        {
            // return CaptureScreen2();
            Gd.InitGd();


            // get the size
            Win32Wrap.RECT windowRect = new Win32Wrap.RECT();
            Win32Wrap.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;

            // real rect
            Win32Wrap.RECT windowRect2 = new Win32Wrap.RECT();
            Win32Wrap.DwmGetWindowAttribute(handle, Win32Wrap.DWMWA_EXTENDED_FRAME_BOUNDS, ref windowRect2, 4 * 4);
            int width2 = windowRect2.right - windowRect2.left;
            int height2 = windowRect2.bottom - windowRect2.top;
            windowRect = windowRect2;
            width = width2;
            height = height2;

            // get te hDC of the target window
            IntPtr hdcSrc = Win32Wrap.GetWindowDC(IntPtr.Zero);

            // create a device context we can copy to
            IntPtr hdcDest = Win32Wrap.CreateCompatibleDC(hdcSrc);

            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = Win32Wrap.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = Win32Wrap.SelectObject(hdcDest, hBitmap);

            // bitblt over
            Win32Wrap.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, windowRect.left, windowRect.top, Win32Wrap.SRCCOPY);

            // restore selection
            Win32Wrap.SelectObject(hdcDest, hOld);

            // clean up 
            Win32Wrap.DeleteDC(hdcDest);
            Win32Wrap.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            Win32Wrap.DeleteObject(hBitmap);
            return img;
        }
        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowToFile(IntPtr windowHandle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(windowHandle);
            img.Save(filename, format);
        }
        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        

        
    }
}
