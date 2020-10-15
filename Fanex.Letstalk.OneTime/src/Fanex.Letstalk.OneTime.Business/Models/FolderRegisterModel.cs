namespace Fanex.Letstalk.OneTime.Business.Models
{
    public class FolderRegisterModel
    {
        public string FolderName { get; set; }

        public bool IsActive { get; set; }

        public FolderMode FolderMode { get; set; }

        public string ReportName { get; set; }
    }
}