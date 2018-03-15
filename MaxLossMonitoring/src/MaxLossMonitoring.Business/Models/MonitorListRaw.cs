namespace MaxLossMonitoring.Business.Models
{
    public class MonitorListRaw
    {
        public MonitorListRaw()
        {
            Username = string.Empty;
            RoleNameForEmail = string.Empty;
            RoleNameForLetstalk = string.Empty;
            Currency = string.Empty;
        }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public string RoleNameForEmail { get; set; }

        public string RoleNameForLetstalk { get; set; }

        public string Currency { get; set; }

        public int GroupHeadlineId { get; set; }

        public decimal YesterdayBalance { get; set; }

        public decimal TotalBalance { get; set; }

        public decimal MaxLoss { get; set; }

        public int LevelId { get; set; }

        public int Movement { get; set; }

        public decimal PercentageLoss { get; set; }
    }
}