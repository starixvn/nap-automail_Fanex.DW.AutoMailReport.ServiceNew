using Fanex.MailDelivery.Core.Models;

namespace Fanex.Service.Core
{
    public class ExecuteResult
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public Mail Mail { get; set; } = new Mail();
    }
}