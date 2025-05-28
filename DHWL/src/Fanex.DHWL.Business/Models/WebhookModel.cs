namespace Fanex.DHWL.Business.Models
{
    public class WebhookModel
    {
        public string WebhookUrl { get; set; }

        public string WebhookHeaderKey { get; set; }

        public string WebhookToken { get; set; }

        public bool IsEnabled { get; set; }

        public bool UseProxy { get; set; }

        public string ProxyUrl { get; set; }
    }
}