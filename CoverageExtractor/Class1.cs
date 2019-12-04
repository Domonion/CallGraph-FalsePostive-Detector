using System;
using System.IO;
using System.Xml;

namespace CoverageExtractor
{
    public class Class1
    {
        public void FromXmlToTxt(string pathToXml, string outputPathGood, string outputPathWrong)
        {
            var document = new XmlDocument();
            if (!File.Exists(pathToXml))
            {
                throw new FileNotFoundException("file not found: " + pathToXml);
            }

            if (!pathToXml.EndsWith(".xml"))
            {
                throw new ArgumentOutOfRangeException(nameof(pathToXml), "first argument should .xml file");
            }

            if (!outputPathGood.EndsWith(".txt"))
            {
                throw new ArgumentOutOfRangeException(nameof(outputPathGood), "first argument should be .txt file");
            }

            if (!outputPathWrong.EndsWith(".txt"))
            {
                throw new ArgumentOutOfRangeException(nameof(outputPathWrong), "second argument should be .txt file");
            }

            document.Load(pathToXml);
            var root = document.DocumentElement;
            using (StreamWriter streamPathGood = File.CreateText(outputPathGood), streamPathWrong = File.CreateText(outputPathWrong))
            {
                using (var visitor = new XmlDfs(streamPathGood, streamPathWrong))
                {
                    visitor.Dfs(root);
                }
            }
        }
    }
}