using System.IO;
using System.Xml;
using static CoverageExtractor.Util;

namespace CoverageExtractor
{
    public static class CoverageConverter
    {
        public static void FromXmlToTxt(string pathToXml, string pathUsed, string pathUnused)
        {
            var document = new XmlDocument();
            AssertExistence(pathToXml);
            AssertExtension(pathToXml, "xml");
            AssertExtension(pathUsed, "gold");
            AssertExtension(pathUnused, "gold");
            document.Load(pathToXml);
            var root = document.DocumentElement;
            using (StreamWriter streamUsed = File.CreateText(pathUsed), streamUnused = File.CreateText(pathUnused))
            {
                using (var visitor = new XmlDfs(streamUsed, streamUnused))
                {
                    visitor.Dfs(root);
                }
            }
        }
    }
}