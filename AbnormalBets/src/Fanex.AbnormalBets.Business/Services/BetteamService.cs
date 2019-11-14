using System.Collections.Generic;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface BetteamService
    {
        string GetBetteam(int bettype, string betteam);
    }

    public class BetteamServiceImp : BetteamService
    {
        private readonly Dictionary<string, string> dictOthers = new Dictionary<string, string>
            {
                { "h", "Home 主" },
                { "a", "AWAY 客" },
                { "o", "OVER 大" },
                { "u", "UNDER 小" },
            };

        private readonly Dictionary<string, string> dict17 = new Dictionary<string, string>
            {
                { "h", "Home 主" },
                { "a", "AWAY 客" }
            };

        private readonly Dictionary<string, string> dict38 = new Dictionary<string, string>
            {
                { "h", "OVER 大" },
                { "a", "UNDER 小" }
            };

        public string GetBetteam(int bettype, string betteam)
        {
            var dict = new Dictionary<int, Dictionary<string, string>>
                {
                    { 1, dict17 },
                    { 7, dict17 },
                    { 3, dict38 },
                    { 8, dict38 },
                };

            var dictResult = dict.ContainsKey(bettype) ? dict[bettype] : dictOthers;
            var result = dictResult.ContainsKey(betteam) ? dictResult[betteam] : betteam;

            return result;
        }
    }
}