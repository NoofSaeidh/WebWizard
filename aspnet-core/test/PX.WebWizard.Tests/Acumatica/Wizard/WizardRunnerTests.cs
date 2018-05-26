using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PX.WebWizard.Acumatica.Wizard;
using PX.WebWizard.Tests.FakeAcExe;

namespace PX.WebWizard.Tests.Acumatica.Wizard
{
    public class WizardRunnerTests
    {
        private readonly string _fakeAcExePath;
        public WizardRunnerTests()
        {
            var fakeAsm = Assembly.GetAssembly(typeof(FakeAcExeProgram));
            _fakeAcExePath = fakeAsm.Location;
        }

        [Fact]
        public async Task RunAcExe_ShouldCatchConsoleOut()
        {
            //todo: chaged - fix

            //// Arrange
            //var runner = new AcExeRunner();
            //var args = JoinArgs(FakeAcExeProgram.Write, "console message");
            //// Act
            //var result = await runner.RunAcExe(_fakeAcExePath, args);
            //// Assert
            //Assert.Equal(0, result);
            ////Assert.Equal("console message", result.Data);
            ////Assert.True(result.Success);
        }

        [Fact]
        public async Task RunAcExe_ShouldCatchException()
        {
            // Arrange
            var runner = new WizardRunner();
            var args = JoinArgs(FakeAcExeProgram.Throw, "throw message");
            // Act
            Func<Task> act = async () => await runner.RunProcessAsync(_fakeAcExePath, args);
            // Assert
            await Assert.ThrowsAsync<ProcessExecutionException>(act);
        }

        private string JoinArgs(string command, string args) => command + " " + args;
    }
}
