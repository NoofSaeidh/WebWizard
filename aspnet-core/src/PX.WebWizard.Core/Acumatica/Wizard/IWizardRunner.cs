﻿using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PX.WebWizard.Acumatica.Wizard
{
    public interface IWizardRunner
    {
        Task RunAcExeAsync(
            string acExePath,
            WizardArgs args, 
            DataReceivedEventHandler outputDataHandler = null, 
            DataReceivedEventHandler errorDataHandler = null, 
            CancellationToken? cancellationToken = null);

        Task RunAcExeAsync(
            string acExePath,
            string args,
            DataReceivedEventHandler outputDataHandler = null,
            DataReceivedEventHandler errorDataHandler = null,
            CancellationToken? cancellationToken = null);
    }
}