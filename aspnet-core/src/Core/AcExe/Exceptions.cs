using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.AcExe
{
    public class AcExeExecutionException : Exception
    {
        public AcExeExecutionException()
        {
        }

        public AcExeExecutionException(string message) : base(message)
        {
        }

        public AcExeExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ProcessExecutionException : Exception
    {
        public ProcessExecutionException()
        {
        }

        public ProcessExecutionException(string message, string process, int exitCode) : base(message)
        {
            HResult = exitCode;
            Process = process;
        }

        public ProcessExecutionException(string message, string process, int exitCode, Exception innerException) : base(message, innerException)
        {
            HResult = exitCode;
            Process = process;
        }

        public string Process { get; protected set; }

        public override string Message
        {
            get
            {
                var message = base.Message;
                var result = $@"Process execution failed.";
                if (!string.IsNullOrEmpty(Process))
                {
                    result += Environment.NewLine + "    Process: " + Process;
                }
                if (HResult != 0)
                {
                    result += Environment.NewLine + "    Code: " + HResult;
                }
                if (!string.IsNullOrEmpty(message))
                {
                    result += Environment.NewLine + "    Message: " + Environment.NewLine + "    " + message;
                }
                return result;
            }
        }
    }
}
