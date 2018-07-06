using System.IO;

namespace TableConvert.Utility.Process
{
    public static class ProcessUtility
    {
        public static void OpenFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                folderPath = folderPath.Replace("/", "\\");
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
        }



    }
}