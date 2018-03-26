namespace TopWinningAgent.Business.Models
{
    public class AppConfig
    {
        public string ExportDataDirectory { get; set; }

        public string ExcelPasswordProtected { get; set; }

        public string Subject { get; set; }

        public string Title { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public int FunctionIdNodata { get; set; }
    }
}