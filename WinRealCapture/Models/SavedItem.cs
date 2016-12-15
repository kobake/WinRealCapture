using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRealCapture.Models
{
    public class SavedItem
    {
        public string FilePath { get; set; }

        public override string ToString()
        {
            return Path.GetFileName(FilePath);
        }
    }
}
