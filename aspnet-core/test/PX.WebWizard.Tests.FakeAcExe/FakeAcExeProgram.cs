using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.WebWizard.Tests.FakeAcExe
{
    public static class FakeAcExeProgram
    {
        public const string WriteLine = "writeline";
        public const string Write = "write";
        public const string Throw = "throw";
        public const string LongRun = "longrun";

        static void Main(string[] args)
        {
            // check first word, if it not started from "-"
            var action = WriteLine;
            IEnumerable<string> newArgs = args;
            if (!args[0].StartsWith("-"))
            {
                action = args[0];
                newArgs = args.Skip(1);
            }

            switch (action)
            {
                case WriteLine:
                    WriteLineArgs(newArgs);
                    break;
                case Write:
                    WriteArgs(newArgs);
                    break;
                case Throw:
                    ThrowException(string.Join(" ", newArgs));
                    break;
                case LongRun:
                    WriteLongRun(newArgs);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid action specified: {action}");
            }
        }

        static void ThrowException(string message)
        {
            throw new Exception(message);
        }

        static void WriteLineArgs(IEnumerable<string> args)
        {
            Console.WriteLine(string.Join(" ", args));
        }
        static void WriteArgs(IEnumerable<string> args)
        {
            Console.Write(string.Join(" ", args));
        }

        static void WriteLongRun(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
                Thread.Sleep(5000);
            }
        }
    }
}
