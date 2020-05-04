using System.Collections.Generic;

namespace Fanex.MailLetstalkTemplate.Business.Models
{
    public class TotalSetting
    {
        public List<int> SumColumnsIndex { get; set; }

        public int LabelIndex { get; set; }

        public string Label { get; set; }

        public int StartMergeColumnIndex { get; set; }

        public int EndMergeColumnIndex { get; set; }
    }
}