using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using JetBrains.Annotations;

namespace CoverageExtractor
{
    public class XmlDfs : IDisposable
    {
        [NotNull] private readonly StreamWriter myOutputGood;
        [NotNull] private readonly StreamWriter myOutputBad;
        [NotNull] private readonly List<string> myCurrentName;

        public XmlDfs([NotNull] StreamWriter writerGood, [NotNull] StreamWriter writerBad)
        {
            myOutputGood = writerGood;
            myOutputBad = writerBad;
            myCurrentName = new List<string>();
        }

        private static bool CheckName(string name)
        {
            return name == "Method" || name == "AnonymousMethod" || name == "Constructor" || name == "PropertyGetter" ||
                   name == "PropertySetter";
        }

        private StreamWriter GetCoverStream([NotNull] XmlNode node)
        {
            return int.Parse(node.Attributes?["CoveragePercent"].Value ?? "0") > 0 ? myOutputGood : myOutputBad;
        }

        public void Dfs(XmlNode node)
        {
            if (node.Name == "PropertyGetter" || node.Name == "PropertySetter")
            {
                
                myCurrentName.Add(node.Attributes?[0].Value);
            }

            if (CheckName(node.Name))
            {
                var covered = GetCoverStream(node);
                switch (node.Name)
                {
                    case "Method":
                    case "AnonymousMethod":
                    case "Constructor":
                    {
                        
                    }
                    case "PropertyGetter":
                    case "PropertySetter":
                }
            }
            
            foreach (XmlElement child in node)
            {
                Dfs(child);
            }

            myCurrentName.RemoveAt(myCurrentName.Count - 1);
        }

        public void Dispose()
        {
            myOutputBad.Dispose();
            myOutputGood.Dispose();
        }
    }
}