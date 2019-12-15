using System;
using CoverageExtractor;
using static CoverageExtractor.CoverageConverter;

namespace Detector
{
    internal static class Program
    {
        private static void PrintHelp()
        {
            Console.WriteLine("help xml diff [exit q q!] clear");
        }

        public static void Main()
        {
            var done = true;
            while (done)
            {
                Console.Write(":");
                var currentCommand = Console.ReadLine().Trim();
                switch (currentCommand)
                {
                    case "exit":
                    case "q":
                    case "q!":
                    {
                        done = false;
                        break;
                    }
                    case "help":
                    {
                        PrintHelp();
                        break;
                    }
                    case "xml":
                    {
                        Console.Write("xml: ");
                        var pathToXml = Console.ReadLine();
                        Console.Write("used: ");
                        var expectedUsed = Console.ReadLine();
                        Console.Write("unused: ");
                        var expectedUnused = Console.ReadLine();
                        try
                        {
                            FromXmlToTxt(pathToXml, expectedUsed, expectedUnused);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        break;
                    }
                    case "diff":
                    {
                        var diff = new DifferenceCounter();
                        Console.Write("expected used: ");
                        var expectedUsed = Console.ReadLine();
                        Console.Write("expected unused: ");
                        var expectedUnused = Console.ReadLine();
                        Console.Write("input used: ");
                        var used = Console.ReadLine();
                        Console.Write("input unused: ");
                        var unused = Console.ReadLine();
                        Console.Write("errors: ");
                        var errors = Console.ReadLine();
                        Console.Write("statistics: ");
                        var statistics = Console.ReadLine();
                        Console.Write("unknown: ");
                        var unknown = Console.ReadLine();
                        try
                        {
                            diff.CalculateError(expectedUsed, expectedUnused, used, unused, errors, statistics,
                                unknown);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        break;
                    }
                    case "clear":
                    {
                        Console.Clear();
                        break;
                    }
                    case "":
                    {
                        break;
                    }
                    default:
                    {
                        Console.WriteLine("Command not recognized, type help to list available commands");
                        break;
                    }
                }
            }
        }
    }
}