using System.Collections.Generic;
using System.IO;
using static CoverageExtractor.Util;

namespace CoverageExtractor
{
    public class DifferenceCounter
    {
        private readonly HashSet<string> myExpectedUsed;
        private readonly HashSet<string> myExpectedUnused;
        private readonly HashSet<string> myFalsePositive;
        private readonly HashSet<string> myUnknown;
        private readonly HashSet<string> myOkey;
        private readonly HashSet<string> myFalseNegative;
        private readonly HashSet<string> myTotal;
        private readonly HashSet<string> myExpectedTotal;

        public DifferenceCounter()
        {
            myExpectedUsed = new HashSet<string>();
            myExpectedUnused = new HashSet<string>();
            myFalsePositive = new HashSet<string>();
            myUnknown = new HashSet<string>();
            myOkey = new HashSet<string>();
            myFalseNegative = new HashSet<string>();
            myTotal = new HashSet<string>();
            myExpectedTotal = new HashSet<string>();
        }

        private static void Dump(TextWriter writer, IEnumerable<string> set)
        {
            foreach (var str in set)
            {
                writer.WriteLine(str);
            }
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

            AssertExtension(expectedUnused, ".gold");
            AssertExtension(expectedUsed, ".gold");
            AssertExtension(used, ".temp");
            AssertExtension(unused, ".temp");

            FillSet(myExpectedUsed, expectedUsed);
            FillSet(myExpectedUnused, expectedUnused);
            /*
             * если используемость в обоих фреймворках сопадает - то тогда ответ на этот метод совпадает в обоих случаях
             * если используемость разнится - то из ковра он попадет в оба множества. тогда вне зависиомсти от варнинга метод попадёт в ок.
             * потому я видимо могу забить на разные фреймворки, единственное - мне нужно сохранять фп, чтобы они не повторялись на фреймворках!
             */
            using (StreamWriter errorsWriter = File.CreateText(errors),
                statisticsWriter = File.CreateText(statistics),
                unknownWriter = File.CreateText(unknown))
            {
                CheckContent(true, used);
                CheckContent(false, unused);
                statisticsWriter.WriteLine("Gold total: " + myExpectedTotal.Count);
                statisticsWriter.WriteLine("Input total: " + myTotal.Count);
                statisticsWriter.WriteLine("Ok: " + myOkey.Count);
                statisticsWriter.WriteLine("False positives: " + myFalsePositive.Count);
                statisticsWriter.WriteLine("False negatives: " + myFalseNegative.Count);
                statisticsWriter.WriteLine("Unknown: " + myUnknown.Count);
                var otstup = new string(' ', 10);
                statisticsWriter.WriteLine("Gold data taken from:");
                statisticsWriter.WriteLine(otstup + expectedUsed);
                statisticsWriter.WriteLine(otstup + expectedUnused);
                statisticsWriter.WriteLine("Test data taken from:");
                statisticsWriter.WriteLine(otstup + used);
                statisticsWriter.WriteLine(otstup + unused);
                Dump(errorsWriter, myFalsePositive);
                Dump(unknownWriter, myUnknown);
                statisticsWriter.WriteLine("False positives listed in: " + errors);
                statisticsWriter.WriteLine("Uknowns listed in: " + unknown);
            }
        }

        private void Check(string input, bool used)
        {
            if (used && myExpectedUsed.Contains(input) || !used && myExpectedUnused.Contains(input))
            {
                myOkey.Add(input);
            }
            else if (used && myExpectedUnused.Contains(input))
            {
                myFalseNegative.Add(input);
            }
            else if (!used && myExpectedUsed.Contains(input))
            {
                myFalsePositive.Add(input);
            }
            else
            {
                myUnknown.Add(input);
            }
        }

        private void CheckContent(bool used, string path)
        {
            using (var reader = File.OpenText(path))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    myTotal.Add(str);
                    Check(str, used);
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
                    myExpectedTotal.Add(str);
                    set.Add(str);
                }
            }
        }
    }
}