using System;
using System.IO;

namespace CoverageExtractor
{
    public static class Util
    {
        public static void AssertExistence(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
        }

        public static void AssertExtension(string path, string extesnion)
        {
            if (Path.GetExtension(path) != extesnion)
            {
                throw new FileFormatException(path, extesnion);
            }
        }
    }

    public class FileFormatException : Exception
    {
        public FileFormatException(string path, string extension) : base(path + " does not have extension: " + extension)
        {
            
        }
    }
}