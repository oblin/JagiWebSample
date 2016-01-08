using System;
using System.Collections.Generic;
using System.IO;

namespace Jagi.Helpers
{
    public class FileHelper
    {
        public const string EXCEL_CONTENT_TYPE = "application//vnd.ms-excel";
        public const string WORD_CONTENT_TYPE = "application//ms-word";
        public const string RTF_CONTENT_TYPE = "application//rtf";
        public const string PDF_CONTENT_TYPE = "application/pdf";
        public const string TXT_CONTENT_TYPE = "text/plain";
        public const string RTF_EXTENSION = ".rtf";
        public const string TXT_EXTENSION = ".txt";
        public const string LOG_EXTENSION = ".log";
        public const string XML_EXTENSION = ".xml";
        public const string XLS_EXTENSION = ".xls";
        public const string BAK_EXTENSION = ".bak";
        public const string SEVEN_EXTENSION = ".7z";
        public const char FILE_SEP = '_';

        public static IEnumerable<string> GetFilesWithoutExtension(string direcotry, string fileLeading)
        {
            if (System.IO.Directory.Exists(direcotry))
            {
                foreach (var fullPath in Directory.GetFiles(direcotry))
                {
                    var file = Path.GetFileNameWithoutExtension(fullPath);
                    if (file.Contains(fileLeading))
                    {
                        yield return file;
                    }
                }
            }
        }

        public static IEnumerable<string> GetFilesFixedExtension(string direcotry, string fileLeading, string fileEnding)
        {
            if (System.IO.Directory.Exists(direcotry))
            {
                foreach (var fullPath in Directory.GetFiles(direcotry))
                {
                    if (!string.Equals(Path.GetExtension(fullPath), fileEnding,
                            StringComparison.OrdinalIgnoreCase))
                        continue;

                    var file = Path.GetFileNameWithoutExtension(fullPath);
                    if (file.Contains(fileLeading))
                    {
                        yield return file + fileEnding;
                    }
                }
            }
        }

        public static string GetFile(string direcotry, string id)
        {
            return direcotry + id;
        }

        public static string GetFileNameWithoutExtionsion(string file)
        {
            return Path.GetFileNameWithoutExtension(file);
        }

        public static string GetFileName(string file)
        {
            return Path.GetFileName(file);
        }

        public static bool DeleteFile(string fullFilePath)
        {
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
                return true;
            }

            return false;
        }

        public static bool CopyFile(string file, string template)
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
            return true;
        }

        public static void SaveToFile(MemoryStream memoryStream, string output)
        {
            DeleteFile(output);

            using (FileStream file = new FileStream(output, FileMode.Create, System.IO.FileAccess.Write))
            {
                memoryStream.WriteTo(file);
                memoryStream.Close();
            }
        }

        public static DateTime GetFileCreateDate(string outputFile)
        {
            return File.GetCreationTime(outputFile);
        }

        public static DateTime GetFileModifyDate(string outputFile)
        {
            return File.GetLastWriteTime(outputFile);
        }
    }
}
