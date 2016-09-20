using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace EasyCapture
{
	public class ScreenCapture
	{
		/// <summary>
		/// Creates an Image object containing a screen shot of the entire desktop
		/// </summary>
		/// <returns></returns>
		public Image CaptureScreen()
		{
			return CaptureWindow(User32.GetDesktopWindow());
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
			GDI32.BitBlt(hDC, 0, 0, bmp.Width, bmp.Height,
				disDC, 0, 0, GDI32.SRCCOPY);
			//解放
			g.ReleaseHdc(hDC);
			g.Dispose();
			User32.ReleaseDC(IntPtr.Zero, disDC);

			return bmp;
		}

		/// <summary>
		/// Creates an Image object containing a screen shot of a specific window
		/// </summary>
		/// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
		/// <returns></returns>
		public Image CaptureWindow(IntPtr handle)
		{
			return CaptureScreen2();

			// get te hDC of the target window
			IntPtr hdcSrc = User32.GetWindowDC(handle);
			// get the size
			User32.RECT windowRect = new User32.RECT();
			User32.GetWindowRect(handle, ref windowRect);
			int width = windowRect.right - windowRect.left;
			int height = windowRect.bottom - windowRect.top;
			// create a device context we can copy to
			IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
			// create a bitmap we can copy it to,
			// using GetDeviceCaps to get the width/height
			IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
			// select the bitmap object
			IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);

			// test:line
			GDI32.MoveToEx(hdcDest, 0, 0, IntPtr.Zero);
			GDI32.SetDCPenColor(hdcDest, 0x0000FF);
			GDI32.LineTo(hdcDest, 300, 300);
			
			// bitblt over
			//GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);

			// restore selection
			//GDI32.SelectObject(hdcDest, hOld);
			// clean up 
			GDI32.DeleteDC(hdcDest);
			User32.ReleaseDC(handle, hdcSrc);
			// get a .NET image object for it
			Image img = Image.FromHbitmap(hBitmap);
			// free up the Bitmap object
			GDI32.DeleteObject(hBitmap);
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

		/// <summary>
		/// Helper class containing Gdi32 API functions
		/// </summary>
		private class GDI32
		{

			public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
			[DllImport("gdi32.dll")]
			public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
				int nWidth, int nHeight, IntPtr hObjectSource,
				int nXSrc, int nYSrc, int dwRop);
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
				int nHeight);
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
			[DllImport("gdi32.dll")]
			public static extern bool DeleteDC(IntPtr hDC);
			[DllImport("gdi32.dll")]
			public static extern bool DeleteObject(IntPtr hObject);
			[DllImport("gdi32.dll")]
			public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);



			// add
			// http://www.pinvoke.net/default.aspx/gdi32.LineTo
			[DllImport("gdi32.dll")]
			public static extern bool MoveToEx(IntPtr hdc, int X, int Y, IntPtr lpPoint);
			[DllImport("gdi32.dll")]
			public static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);
			[DllImport("gdi32.dll")]
			public static extern uint SetDCPenColor(IntPtr hdc, uint crColor);
			[DllImport("gdi32.dll")]
			public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
		}

		/// <summary>
		/// Helper class containing User32 API functions
		/// </summary>
		private class User32
		{
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

			

		}
	}
}
