using System;
using System.Collections.Generic;

namespace Fanex.MailLetstalkTemplate.Business.Models
{
    public class ConfigSettingModel
    {
        public string SPName { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public bool IsIgnoredCheckMailSent { get; set; } = true;

        public string Path { get; set; }

        public string TargetName { get; set; }

        public string Group { get; set; }

        public string Password { get; set; }

        public int FunctionIdNodata { get; set; }

        public DateRange DateRange { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public Dictionary<string, ImageProperty> ImageSettings { get; set; } = new Dictionary<string, ImageProperty>();

        public string TemplateExcelFile { get; set; }

        public List<string> SheetExcelTemplateName { get; set; }

        public List<string> CellAddressRender { get; set; }

        public int StartExelRowIndex { get; set; }

        public string EmailTemplate { get; set; }

        public string EmailSubject { get; set; }

        public int MailId { get; set; }

        public string MailName { get; set; }

        public string FileName { get; set; }

        public int StartExcelColumnIndex { get; set; } = 1;

        public bool IsEnabledSectionTotal { get; set; }

        public List<TotalSetting> TotalSettings { get; set; } = new List<TotalSetting>();

        public bool IsEnabledSectionHeader { get; set; }

        public List<TitleSetting> TitleSettings { get; set; } = new List<TitleSetting>();
    }
}