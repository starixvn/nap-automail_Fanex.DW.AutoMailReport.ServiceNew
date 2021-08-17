using Fanex.AutoDelFiles.Business.Service;
using Fanex.AutoDelFiles.Service;
using SimpleInjector;

namespace Fanex.AutoDelFiles.Executing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RegisterService.RegisterServices();

            Container container = new Container();
            RegisterService.RegisterContainer(container);
            container.Verify(VerificationOption.VerifyOnly);

            var service = container.GetInstance<DelFileNotificationService>();
            service.Execute();
        }
    }
}