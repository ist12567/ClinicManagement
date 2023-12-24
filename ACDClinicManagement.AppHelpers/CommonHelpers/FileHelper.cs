using ACDClinicManagement.Helpers;
using System;
using System.IO;

namespace ACDClinicManagement.AppHelpers.CommonHelpers
{
    public static class FileHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Const

        private const int Kbyte = 1024;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static long GetFileSizeInKb(this FileInfo value)

        public static long GetFileSizeInKb(this FileInfo value)
        {
            var fileSize = value.Length/Kbyte;
            return fileSize;
        }

        #endregion

        #region public static long GetFileSizeInKb(this byte[] value)

        public static long GetFileSizeInKb(this byte[] value)
        {
            var fileSize = value.Length/Kbyte;
            return fileSize;
        }

        #endregion

        #region public void ToLogFile(this string logMessage)
        public static void ToLogFile(this string logMessage)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (var streamWriter = File.AppendText(Path.Combine(path, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "Log.txt")))
            {
                streamWriter.WriteLine($"Date and Time: {DateTime.Now.ToPersianDateTime()}");
                streamWriter.WriteLine($"Message: {logMessage}");
                streamWriter.WriteLine();
            }
        }

        #endregion
    }
}
