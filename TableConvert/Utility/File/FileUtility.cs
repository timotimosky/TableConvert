using System.IO;

namespace TableConvert.Utility.File
{
    public static class FileUtility
    {


        public static string[] GetFiles( string path, string suffix = "*.*" )
        {
            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path, suffix);
            }

            return default(string[]);
        }


    }


}