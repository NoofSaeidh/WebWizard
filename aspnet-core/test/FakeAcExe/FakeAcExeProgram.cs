using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Tests.FakeAcExe
{
    public static class FakeAcExeProgram
    {
        public const string WriteLine = "writeline";
        public const string Write = "write";
        public const string Throw = "throw";


        static void Main(string[] args)
        {
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
    }
}
