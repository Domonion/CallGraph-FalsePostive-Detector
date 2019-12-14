using System.Collections.Generic;
using System.IO;
using static CoverageExtractor.Util;

namespace CoverageExtractor
{
    public class DifferenceCounter
    {
        private readonly HashSet<string> myExpectedUsed;
        private readonly HashSet<string> myExpectedUnused;
        private int myOkey;
        private int myTotal;
        private int myExpectedTotal;
        private int myFalsePositive;
        private int myFalseNegative;
        private int myUnknown;

        public DifferenceCounter()
        {
            myExpectedUnused = new HashSet<string>();
            myOkey = myTotal = myExpectedTotal = myFalseNegative = myFalsePositive = myUnknown = 0;
            myExpectedUsed = new HashSet<string>();
        }

        public void CalculateError(string expectedUsed, string expectedUnused, string used, string unused,
            string errors, string statistics, string unknown)
        {
            //used, used - okey
            //used, unused - FALSE POSITIVE
            //unsed, used - FALSE NEGATIVE
            //unused, unused. - okey
            AssertExistence(expectedUsed);
            AssertExistence(expectedUnused);
            AssertExistence(unused);
            AssertExistence(used);
            
            AssertExtension(expectedUnused, "gold");
            AssertExtension(expectedUsed, "gold");
            AssertExtension(used, "temp");
            AssertExtension(unused, "temp");
            
            FillSet(myExpectedUsed, expectedUsed);
            FillSet(myExpectedUnused, expectedUnused);
            using (StreamWriter errorsWriter = File.CreateText(errors), statisticsWriter = File.CreateText(statistics),
                    unkownWriter = File.CreateText(unknown))
            {
                CheckContent(true, used, errorsWriter, unkownWriter);
                CheckContent(false, unused, errorsWriter, unkownWriter);
                statisticsWriter.WriteLine("Expected total: " + myExpectedTotal);
                statisticsWriter.WriteLine("Data total: " + myTotal);
                statisticsWriter.WriteLine("Ok: " + myOkey);
                statisticsWriter.WriteLine("False positives: " + myFalsePositive);
                statisticsWriter.WriteLine("False negatives: " + myFalseNegative);
                statisticsWriter.WriteLine("Unknown: " + myUnknown);
                var otstup = new string(' ', 10);
                statisticsWriter.WriteLine("Gold data taken from:");
                statisticsWriter.WriteLine(otstup + expectedUsed);
                statisticsWriter.WriteLine(otstup + expectedUnused);
                statisticsWriter.WriteLine("Test data taken from:");
                statisticsWriter.WriteLine(otstup + used);
                statisticsWriter.WriteLine(otstup + unused);
                statisticsWriter.WriteLine("False positives listed in: " + errors);
                statisticsWriter.WriteLine("Uknowns listed in: " + unknown);
            }
        }

        private void Check(string input, bool used, TextWriter writer, TextWriter unknown)
        {
            if (used && myExpectedUsed.Contains(input) || !used && myExpectedUnused.Contains(input))
            {
                myOkey++;
            }
            else if (used && myExpectedUnused.Contains(input))
            {
                myFalseNegative++;
            }
            else if(!used && myExpectedUsed.Contains(input))
            {
                myFalsePositive++;
                writer.WriteLine(input);
            }
            else
            {
                myUnknown++;
                unknown.WriteLine(input);
            }
        }

        private void CheckContent(bool used, string path, StreamWriter errors, StreamWriter unknown)
        {
            using (var reader = File.OpenText(path))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    myTotal++;
                    Check(str, used, errors, unknown);
                }
            }
        }

        private void FillSet(ISet<string> set, string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                string str;
                while ((str = streamReader.ReadLine()) != null)
                {
                    myExpectedTotal++;
                    set.Add(str);
                }
            }
        }
    }
}