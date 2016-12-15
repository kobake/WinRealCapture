using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Win32WrapLib
{
    public class Gd
    {
        static bool _inited = false;
        public static void InitGd()
        {
            if (_inited) return;
            _inited = true;

            // 高解像度の縮小無効処理
            // http://stackoverflow.com/questions/13228185/how-to-configure-an-app-to-run-correctly-on-a-machine-with-a-high-dpi-setting-e/13228495#13228495
            if (Environment.OSVersion.Version.Major >= 6) Win32Wrap.SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
