using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XOX.CRM.Lib
{
    public class Helper
    {
        public static string GetUniqueFilename(string fullPath)
        {
            if (!Path.IsPathRooted(fullPath))
                fullPath = Path.GetFullPath(fullPath);
            if (File.Exists(fullPath))
            {
                String filename = Path.GetFileName(fullPath);
                String path = fullPath.Substring(0, fullPath.Length - filename.Length);
                String filenameWOExt = Path.GetFileNameWithoutExtension(fullPath);
                String ext = Path.GetExtension(fullPath);
                int n = 1;
                do
                {
                    fullPath = Path.Combine(path, String.Format("{0}({1}){2}", filenameWOExt, (n++), ext));
                }
                while (File.Exists(fullPath));
            }
            return fullPath;
        }

    }
}
