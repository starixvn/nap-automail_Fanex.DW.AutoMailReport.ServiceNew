using System;
using System.Collections.Generic;

namespace NegativeBalance.Business.Models
{
    public class MatchInfo
    {
        public string LeagueName { get; set; } = string.Empty;

        public string LeagueNameCS { get; set; } = string.Empty;

        public string HomeName { get; set; } = string.Empty;

        public string HomeNameCS { get; set; } = string.Empty;

        public string AwayName { get; set; } = string.Empty;

        public string AwayNameCS { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string MatchCode { get; set; } = string.Empty;

        public int MatchId { get; set; }

        public int GroupId { get; set; }

        public string SportName { get; set; } = string.Empty;

        public string SportNameCS { get; set; } = string.Empty;

        public bool IsValid
        {
            get
            {
                const int GroupId1 = 1;
                const int GroupId2 = 2;
                const int GroupId3 = 3;

                var listName = new List<string> { };
                switch (GroupId)
                {
                    case GroupId1:
                    case GroupId2:
                        listName.Add(HomeName);
                        listName.Add(HomeNameCS);
                        listName.Add(AwayName);
                        listName.Add(AwayNameCS);
                        break;

                    case GroupId3:
                        listName.Add(HomeName);
                        listName.Add(HomeNameCS);
                        listName.Add(LeagueName);
                        listName.Add(LeagueNameCS);
                        break;

                    default: // group 7: [Betradar] Virtual Sports 2
                        listName.Add(HomeName);
                        listName.Add(HomeNameCS);
                        listName.Add(AwayName);
                        listName.Add(AwayNameCS);
                        listName.Add(SportName);
                        listName.Add(SportNameCS);
                        break;
                }

                var isValidName = true;
                listName.ForEach(name =>
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        isValidName = false;
                    }
                });

                return isValidName;
            }
        }
    }
}