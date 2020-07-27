using System.Threading;

namespace Fanex.ServiceMonitoring.Service
{
    public class StartInstance
    {
        private readonly ServiceMonitoring serviceMonitoring;
        private readonly MailMonitoring mailMonitoring;

        public StartInstance(ServiceMonitoring serviceMonitoring, MailMonitoring mailMonitoring)
        {
            this.serviceMonitoring = serviceMonitoring;
            this.mailMonitoring = mailMonitoring;
        }

        public bool Start()
        {
            // var instanceService = new Thread(new ThreadStart(serviceMonitoring.Start));
            var instanceMail = new Thread(new ThreadStart(mailMonitoring.Start));

            // instanceService.Start();
            instanceMail.Start();

            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}