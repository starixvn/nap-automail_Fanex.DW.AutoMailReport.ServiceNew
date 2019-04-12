using System.Net;

namespace Fanex.ServiceMonitoring.Services
{
    public interface CommonService
    {
        string GetLocalIP();
    }

    public class CommonServiceImp : CommonService
    {
        public string GetLocalIP()
        {
            var ip = string.Empty;
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ip = addr.ToString();
                }
            }

            return ip;
        }
    }
}