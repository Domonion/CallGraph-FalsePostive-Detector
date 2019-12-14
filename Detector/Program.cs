using CoverageExtractor;
using static CoverageExtractor.CoverageConverter;

namespace Detector
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var pathToXml = args[0];
            var expectedUsed = args[1];
            var expectedUnused = args[2];
            var used = args[3];
            var unused = args[4];
            var errors = args[5];
            var statistics = args[6];
            var unknown = args[7];
            FromXmlToTxt(pathToXml, expectedUsed, expectedUnused);
            var diff = new DifferenceCounter();
            diff.CalculateError(expectedUsed, expectedUnused, used, unused, errors, statistics, unknown);
        }
    }
}