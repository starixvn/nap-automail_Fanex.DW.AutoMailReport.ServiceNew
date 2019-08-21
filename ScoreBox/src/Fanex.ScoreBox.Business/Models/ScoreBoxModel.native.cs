using System;

namespace Fanex.ScoreBox.Business.Models
{
    public class ScoreBoxModel
    {
        public DateTime DateTime { get; set; }

        public string Site { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public string League { get; set; }

        public int MatchId { get; set; }

        public string TeamA { get; set; }

        public string TeamB { get; set; }

        public string BetType { get; set; }

        public string BetChoice { get; set; }

        public int NoOfPlayers { get; set; }

        public int TotalTicketWon { get; set; }

        public int TotalTicketLost { get; set; }

        public int TotalTicketDraw { get; set; }

        public decimal TotalStakeWon { get; set; }

        public decimal TotalWLWon { get; set; }

        public decimal TotalStakeLost { get; set; }

        public decimal TotalWLLost { get; set; }

        public decimal TotalStakeDraw { get; set; }

        public decimal TotalStakeWonRM { get; set; }

        public decimal TotalWLWonRM { get; set; }

        public decimal TotalStakeLostRM { get; set; }

        public decimal TotalWLLostRM { get; set; }

        public decimal TotalStakeDrawRM { get; set; }
    }
}