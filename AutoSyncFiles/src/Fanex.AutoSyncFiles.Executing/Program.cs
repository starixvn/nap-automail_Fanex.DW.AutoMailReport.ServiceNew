using System;
using Fanex.AutoSyncFiles.Business.Services;
using SimpleInjector;

namespace Fanex.AutoSyncFiles.Executing
{
    internal class Program
    {
        private static void Main()
        {
            RegisterService.RegisterServices();
            var container = new Container();
            RegisterService.RegisterContainer(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<SyncFilesService>();

            service.Execute();

            Console.WriteLine("Done");
            Console.ReadLine();

            Environment.Exit(0);
        }
    }
}