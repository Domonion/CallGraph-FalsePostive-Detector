using System;
using System.Collections.Generic;
using System.IO;
using CoverageExtractor;
using static CoverageExtractor.CoverageConverter;

namespace Detector
{
    internal static class Program
    {
        private class Command
        {
            private readonly List<string> myArgs;
            private readonly Action<List<string>> myCommand;

            public Command(List<string> args, Action<List<string>> command)
            {
                myArgs = args;
                myCommand = command;
            }

            public void Execute()
            {
                myCommand(myArgs);
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("help xml diff [exit q q!] clear dir last");
        }

        public static void Main()
        {
            var done = true;
            Command last = null;
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
                        last = new Command(null, list => PrintHelp());
                        last.Execute();
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
                        last = new Command(new List<string> {pathToXml, expectedUsed, expectedUnused}, list =>
                        {
                            try
                            {
                                FromXmlToTxt(list[0], list[1], list[2]);
//                                FromXmlToTxt(pathToXml, expectedUsed, expectedUnused);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                        break;
                    }
                    case "last":
                    {
                        last?.Execute();
                        break;
                    }
                    case "dir":
                    {
                        var diff = new DifferenceCounter();
                        Console.Write("directory: ");
                        var dir = Console.ReadLine();
                        last = new Command(new List<string> {dir}, list =>
                        {
                            try
                            {
                                FromXmlToTxt(Path.Combine(list[0], "snapshot.xml"), Path.Combine(list[0], "used.gold"), Path.Combine(list[0], "unused.gold"));
                                diff.CalculateError(Path.Combine(list[0], "used.gold"), Path.Combine(list[0], "unused.gold"), Path.Combine(list[0], "used.temp"),
                                    Path.Combine(list[0], "unused.temp"), Path.Combine(list[0], "errors.txt"), Path.Combine(list[0], "stats.txt"),
                                    Path.Combine(list[0], "unknown.txt"));
//                                FromXmlToTxt(Path.Combine(dir, "snapshot.xml"), Path.Combine(dir, "used.gold"), Path.Combine(dir, "unused.gold"));
//                                diff.CalculateError(Path.Combine(dir, "used.gold"), Path.Combine(dir, "unused.gold"), Path.Combine(dir, "used.temp"),
//                                    Path.Combine(dir, "unused.temp"), Path.Combine(dir, "errors.txt"), Path.Combine(dir, "stats.txt"),
//                                    Path.Combine(dir, "unknown.txt"));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                        last.Execute();


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
                        last = new Command(new List<string> {expectedUsed, expectedUnused, used, unused, errors, statistics, unknown}, list =>
                        {
                            try
                            {
//                                diff.CalculateError(expectedUsed, expectedUnused, used, unused, errors, statistics, unknown);
                                diff.CalculateError(list[0], list[1], list[2], list[3], list[4], list[5], list[6]);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                        last.Execute();
                        break;
                    }
                    case "clear":
                    {
                        last = new Command(null, list => Console.Clear());
                        last.Execute();
                        break;
                    }
                    case "":
                    {
                        break;
                    }
                    default:
                    {
                        last = new Command(null, list => Console.WriteLine("Command not recognized, type help to list available commands"));
                        last.Execute();
                        break;
                    }
                }
            }
        }
    }
}