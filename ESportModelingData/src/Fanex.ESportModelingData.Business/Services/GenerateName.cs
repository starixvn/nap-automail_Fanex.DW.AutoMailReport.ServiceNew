using Fanex.ESportModelingData.Business.Models;
using System.Collections.Generic;

namespace Fanex.ESportModelingData.Business.Services
{
    public static class GenerateName
    {
        public static Dictionary<Sport, string> SportNames
        {
            get
            {
                return new Dictionary<Sport, string>
                {
                    { Sport.ESports, "E-Sports" },
                    { Sport.Baskketball, "Baskketball" },
                };
            }
        }
    }
}