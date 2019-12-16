using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using JetBrains.Annotations;

namespace CoverageExtractor
{
    public class XmlDfs : IDisposable
    {
        [NotNull] private readonly StreamWriter myUsed;
        [NotNull] private readonly StreamWriter myUnused;
        [NotNull] private readonly List<string> myCurrentName;

        public XmlDfs([NotNull] StreamWriter used, [NotNull] StreamWriter unused)
        {
            myUsed = used;
            myUnused = unused;
            myCurrentName = new List<string>();
        }

        private static bool ShouldRemoveReturn(string name)
        {
            return name == "Method" || name == "AnonymousMethod" || name == "AutoProperty" ||
                   name == "Event" || name == "InternalCompiledMethod" || name == "Property";
        }

        private static bool IsNamed(string name)
        {
            return ShouldRemoveReturn(name) || name == "Constructor";
        }

        private static bool IsUnnamed(string name)
        {
            return name == "PropertyGetter" ||
                   name == "PropertySetter" ||
                   name == "EventAdder" ||
                   name == "EventRemover";
        }

        private static bool IsUseless(string name)
        {
            return name == "Root" || name == "xml" || name == "?xml" || name == "OwnCoverage" || name == "Assembly";
        }

        private static bool ShouldBeDisplayed(string name)
        {
            return IsNamed(name) || IsUnnamed(name);
        }

        private StreamWriter GetCoverStream([NotNull] XmlNode node)
        {
            return int.Parse(GetOrThrow(node, "CoveragePercent")) > 0 ? myUsed : myUnused;
        }

        private static string GetOrThrow([NotNull] XmlNode node, string attr)
        {
            var res = node.Attributes?[attr].Value ?? throw new XmlException("incorrect coverage");
            if (ShouldRemoveReturn(node.Name) && attr == "Name")
            {
                var ind = res.LastIndexOf(":", StringComparison.Ordinal);
                res = res.Remove(ind);
            }

            return res;
        }

        public void Dfs(XmlNode node)
        {
            if (ShouldBeDisplayed(node.Name))
            {
                var covered = GetCoverStream(node);
                myCurrentName.Add(IsNamed(node.Name) ? GetOrThrow(node, "Name") : node.Name);

                for (var index = 0; index < myCurrentName.Count; index++)
                {
                    var res = Regex.Replace(Regex.Replace(myCurrentName[index], @"\bSystem.String\b", "string"),
                        @"\bSystem.Int32\b", "int");
                    var l = res.IndexOf("(", StringComparison.Ordinal);
                    var r = res.LastIndexOf(")", StringComparison.Ordinal);
                    var DotReductor = new DotReductor();
                    DotReductor.Register(res);
                    DotReductor.Reduct(l, r);
                    res = DotReductor.Get();
                    covered.Write(res);
                    if (index != myCurrentName.Count - 1)
                    {
                        covered.Write(".");
                    }
                }

                covered.Write("\n");
            }
            else
            {
                if (!IsUseless(node.Name))
                {
                    myCurrentName.Add(GetOrThrow(node, "Name"));
                }
            }

            foreach (XmlElement child in node)
            {
                Dfs(child);
            }

            if (!IsUseless(node.Name))
            {
                myCurrentName.RemoveAt(myCurrentName.Count - 1);
            }
        }

        public void Dispose()
        {
            myUnused.Dispose();
            myUsed.Dispose();
        }
    }
}