namespace Fanex.AutoDelFiles.Business.Models
{
    public class PathModel
    {
        public string Source { get; set; }

        public string SourceServerUsername { get; set; }

        public string SourceServerPassword { get; set; }

        public int MaxAgeOfFileInDays { get; set; }
    }
}