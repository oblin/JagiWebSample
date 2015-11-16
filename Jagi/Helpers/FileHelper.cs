using System;
using System.IO;

namespace Jagi.Helpers
{
    public class FileHelper
    {
        public static void CopyFile(string file, string template)
        {
            if (File.Exists(file))
            {
                // 刪除判斷建立時間之功能
                //DateTime createTime = File.GetCreationTime(file);
                //TimeSpan substract = DateTime.Now.Subtract(createTime);
                //if (substract.Minutes < 30)
                //    return false;

                File.Delete(file);
            }

            File.Copy(template, file);
            File.SetLastWriteTime(file, DateTime.Now);
        }
    }
}
