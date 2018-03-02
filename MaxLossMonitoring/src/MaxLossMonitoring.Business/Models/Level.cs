namespace MaxLossMonitoring.Business.Models
{
    public class Level
    {
        public Level()
        {
            TextForEmail = string.Empty;
            TextForLetstalk = string.Empty;
        }

        public int LevelId { get; set; }

        public int Value { get; set; }

        public string TextForEmail { get; set; }

        public string TextForLetstalk { get; set; }
    }
}