using Fanex.MailDelivery.Client;
using SimpleInjector;
using System;
using System.IO;

namespace Fanex.DailyDHWL.Executing.Services
{
    public class DHWLService
    {
        public void Start()
        {
            try
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

                RegisterService.RegisterServices();
                var container = new Container();
                RegisterService.Container = container;
                RegisterService.RegisterContainer(container);
                container.Verify(VerificationOption.VerifyOnly);

                MailClientHandler.Start();
            }
            catch (Exception e)
            {
                File.WriteAllText("/log.txt", e.Message);
            }
        }

        public void Stop()
        {
            MailClientHandler.Stop();
        }
    }
}