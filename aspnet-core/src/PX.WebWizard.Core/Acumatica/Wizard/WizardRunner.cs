using Abp.Dependency;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PX.WebWizard.Acumatica.Wizard
{
    public class WizardRunner : IWizardRunner, ISingletonDependency
    {
        public async Task RunAcExeAsync(
            string acExePath,
            WizardArgs args,
            DataReceivedEventHandler outputDataHandler = null,
            DataReceivedEventHandler errorDataHandler = null,
            CancellationToken? cancellationToken = null)
        {
            await RunProcessAsync(acExePath, args.Serialize(), outputDataHandler, errorDataHandler, cancellationToken);
        }

        public async Task RunProcessAsync(
            string file,
            string args,
            DataReceivedEventHandler outputDataHandler = null,
            DataReceivedEventHandler errorDataHandler = null,
            CancellationToken? cancellationToken = null)
        {
            using (var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = file,
                    Arguments = args,

                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                    //Verb = "runas",
                },
            })
            {
                await RunProcessAsync(process, outputDataHandler, errorDataHandler, cancellationToken);
            }
        }

        //todo: move to separate class
        private async static Task RunProcessAsync(Process process
            , DataReceivedEventHandler outputDataHandler = null
            , DataReceivedEventHandler errorDataHandler = null
            , CancellationToken? cancellationToken = null)
        {
            //todo: remove int
            var tcs = new TaskCompletionSource<int>();

            process.Exited += (sender, args) =>
            {
                if (process.ExitCode != 0)
                {
                    string errorMessage = null;
                    if (errorDataHandler == null)
                        errorMessage = process.StandardError.ReadToEnd();
                    //todo: error message in other case
                    tcs.SetException(new ProcessExecutionException(errorMessage, process.StartInfo.FileName, process.ExitCode));
                }
                else
                {
                    tcs.SetResult(process.ExitCode);
                }
            };

            if (outputDataHandler != null)
                process.OutputDataReceived += outputDataHandler;
            if (errorDataHandler != null)
                process.ErrorDataReceived += errorDataHandler;

            var started = process.Start();
            if (!started)
            {
                //you may allow for the process to be re-used (started = false)
                //but I'm not sure about the guarantees of the Exited event in such a case
                tcs.SetException(new InvalidOperationException("Could not start process: " + process));
            }

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            if (cancellationToken != null)
                cancellationToken.Value.Register(process.Kill);
            await tcs.Task;
        }
    }
}