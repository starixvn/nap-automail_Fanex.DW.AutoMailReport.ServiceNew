using System;
using Fanex.AutoSyncFiles.Business.Services;
using Newtonsoft.Json;
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
            var jobMessage = service.Execute();
            var json = JsonConvert.SerializeObject(jobMessage);

            Console.Write(json);
            Environment.Exit(0);
        }
    }
}