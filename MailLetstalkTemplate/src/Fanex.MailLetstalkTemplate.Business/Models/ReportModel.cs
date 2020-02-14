namespace Fanex.MailLetstalkTemplate.Business.Models
{
    public class ReportModel
    {
        public string ReportFile { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailEnabled { get; set; }

        public bool IsLetstalkEnabled { get; set; }
    }
}