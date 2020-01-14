using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.CN88DailyWL.Business.Models
{
    public class ReportModel
    {
        public string Date { get; set; }

        public CN88Model TPG { get; set; }

        public CN88Model XPG { get; set; }

        public CN88Model Total => new CN88Model
        {
            Date = string.Empty,
            Headline = string.Empty,
            ActiveMember = TPG.ActiveMember + XPG.ActiveMember,
            BetCount = TPG.BetCount + XPG.BetCount,
            MemberTO = TPG.MemberTO + XPG.MemberTO,
            NetTO = TPG.NetTO + XPG.NetTO,
            MemberWL = TPG.MemberWL + XPG.MemberWL,
            CompanyTO = TPG.CompanyTO + XPG.CompanyTO,
            CompanyTO_Live = TPG.CompanyTO_Live + XPG.CompanyTO_Live,
            CompanyTO_NonLive = TPG.CompanyTO_NonLive + XPG.CompanyTO_NonLive,
            CompanyTO_Mix = TPG.CompanyTO_Mix + XPG.CompanyTO_Mix,
            CompanyIncome = TPG.CompanyIncome + XPG.CompanyIncome,
            CompanyIncome_Live = TPG.CompanyIncome_Live + XPG.CompanyIncome_Live,
            CompanyIncome_NonLive = TPG.CompanyIncome_NonLive + XPG.CompanyIncome_NonLive,
            CompanyIncome_Mix = TPG.ActiveMember + XPG.CompanyIncome_Mix,
            CompanyMargin = TPG.CompanyMargin + XPG.CompanyMargin
        };
    }
}
