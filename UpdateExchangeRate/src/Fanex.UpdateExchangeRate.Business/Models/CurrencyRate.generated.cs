using System;
using Epplus.Mapper.Annotations;

namespace Fanex.UpdateExchangeRate.Business.Models
{
    public class CurrencyRate
    {
        [Cell("A3")]
        public DateTime ChangeDate { get; set; }

        [Cell("B3")]
        public int CurrencyId { get; set; }

        [Cell("C3")]
        public string CurrencyName { get; set; }

        [Cell("D3")]
        public string IsoCode { get; set; }

        [Cell("E3")]
        public string CurrencyDescription { get; set; }

        [Cell("F3")]
        public decimal OldRate { get; set; }

        [Cell("G3")]
        public decimal NewRate { get; set; }
    }
}