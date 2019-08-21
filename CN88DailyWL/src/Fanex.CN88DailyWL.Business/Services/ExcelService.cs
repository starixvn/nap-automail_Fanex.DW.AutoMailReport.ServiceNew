using Fanex.CN88DailyWL.Business.Models;
using OfficeOpenXml;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Fanex.CN88DailyWL.Business.Services
{
    public interface ExcelService
    {
        List<string> Execute(Tuple<CN88Model, CN88Model> data);
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly ExcelWorksheet template;
        private readonly DateService dateService;

        public ExcelServiceImp(ExcelPackage excelPackage, DateService dateService)
        {
            this.excelPackage = excelPackage;
            this.dateService = dateService;

            template = excelPackage.Workbook.Worksheets["Template"];
        }

        public List<string> Execute(Tuple<CN88Model, CN88Model> data)
        {
            RenderDailyEN(data.Item1);
            RenderDailyCN(data.Item1);

            RenderPeriodEN(data.Item2);
            RenderPeriodCN(data.Item2);

            return SaveImage();
        }

        private void RenderDailyEN(CN88Model data)
        {
            template.Cells["C2"].Value = $"   [{data.Date}]";
            template.Cells["E3"].Value = data.ActiveMember;
            template.Cells["E4"].Value = data.BetCount;
            template.Cells["E5"].Value = data.MemberTO;
            template.Cells["E6"].Value = data.NetTO;
            template.Cells["E7"].Value = data.MemberWL;
            template.Cells["E8"].Value = data.CompanyTO;
            template.Cells["E9"].Value = data.CompanyIncome;
            template.Cells["E10"].Value = data.CompanyMargin;
        }

        private void RenderDailyCN(CN88Model data)
        {
            template.Cells["C26"].Value = $"   [{data.Date}]";
            template.Cells["E27"].Value = data.ActiveMember;
            template.Cells["E28"].Value = data.BetCount;
            template.Cells["E29"].Value = data.MemberTO;
            template.Cells["E30"].Value = data.NetTO;
            template.Cells["E31"].Value = data.MemberWL;
            template.Cells["E32"].Value = data.CompanyTO;
            template.Cells["E33"].Value = data.CompanyIncome;
            template.Cells["E34"].Value = data.CompanyMargin;
        }

        private void RenderPeriodEN(CN88Model data)
        {
            template.Cells["C13"].Value = $"   [Current Period: {data.Date}]";
            template.Cells["E14"].Value = data.ActiveMember;
            template.Cells["E15"].Value = data.BetCount;
            template.Cells["E16"].Value = data.MemberTO;
            template.Cells["E17"].Value = data.NetTO;
            template.Cells["E18"].Value = data.MemberWL;
            template.Cells["E19"].Value = data.CompanyTO;
            template.Cells["E20"].Value = data.CompanyIncome;
            template.Cells["E21"].Value = data.CompanyMargin;
        }

        private void RenderPeriodCN(CN88Model data)
        {
            template.Cells["C37"].Value = $"   [本期: {data.Date}]";
            template.Cells["E38"].Value = data.ActiveMember;
            template.Cells["E39"].Value = data.BetCount;
            template.Cells["E40"].Value = data.MemberTO;
            template.Cells["E41"].Value = data.NetTO;
            template.Cells["E42"].Value = data.MemberWL;
            template.Cells["E43"].Value = data.CompanyTO;
            template.Cells["E44"].Value = data.CompanyIncome;
            template.Cells["E45"].Value = data.CompanyMargin;
        }

        private string SaveExceFile()
        {
            var date = dateService.WinLostDate;
            var fileName = $"CN88_Daily_WL{date:ddMMyyyy}.xlsx";
            var path = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), string.Empty);

            return path;
        }

        private List<string> SaveImage()
        {
            var date = dateService.WinLostDate;
            var imageNameEn = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}";            
            var imageNameCn = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}";

            if (!Directory.Exists(imageNameEn))
            {
                Directory.CreateDirectory(imageNameEn);
            }

            imageNameEn = $@"{imageNameEn}\CN88_Daily_WL_EN_{date:ddMMyyyy}.png";
            imageNameCn = $@"{imageNameCn}\CN88_Daily_WL_CN_{date:ddMMyyyy}.png";

            var imageNameEnFinal = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}\CN88_Daily_WL_EN_Final_{date:ddMMyyyy}.png";
            var imageNameCnFinal = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}\CN88_Daily_WL_CN_Final_{date:ddMMyyyy}.png";

            const int FirstRowEn = 1;
            const int FirstColumnEn = 2;
            const int LastRowEn = 23;
            const int LastColumnEn = 7;

            const int FirstRowCn = 25;
            const int FirstColumnCn = 2;
            const int LastRowCn = 47;
            const int LastColumnCn = 7;

            const int NewWidth = 400;
            const int NewHeight = 600;
            const int X = 100;
            const int Y = 100;

            var excelPath = SaveExceFile();
            var workbook = new Workbook();
            workbook.LoadFromFile(excelPath);
            var sheet = workbook.Worksheets[0];

            var imageEn = sheet.ToImage(FirstRowEn, FirstColumnEn, LastRowEn, LastColumnEn);
            var newImgEn = ResizeImage(imageEn, 600, 800);
            newImgEn.Save(imageNameEn, ImageFormat.Png);

            var imageCn = sheet.ToImage(FirstRowCn, FirstColumnCn, LastRowCn, LastColumnCn);
            var newImgCn = ResizeImage(imageCn, 600, 800);
            newImgCn.Save(imageNameCn, ImageFormat.Png);

            Crop(imageNameEn, imageNameEnFinal, NewWidth, NewHeight, X, Y);
            Crop(imageNameCn, imageNameCnFinal, NewWidth, NewHeight, X, Y);

            return new List<string>
            {
                imageNameEnFinal,
                imageNameCnFinal
            };
        }

        private static Image ResizeImage(Image image, int newWidth, int newHeight)
        {
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(newImage);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        private static void Crop(string sourceImg, string newImage, int width, int height, int x, int y)
        {
            using (Image originalImage = Image.FromFile(sourceImg))
            {
                using (Bitmap bmp = new Bitmap(width, height))
                {
                    bmp.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);
                    using (Graphics graphic = Graphics.FromImage(bmp))
                    {
                        graphic.SmoothingMode = SmoothingMode.AntiAlias;
                        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphic.DrawImage(originalImage, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
                        MemoryStream ms = new MemoryStream();
                        bmp.Save(ms, originalImage.RawFormat);

                        if (File.Exists(newImage))
                        {
                            File.Delete(newImage);
                        }                        

                        File.WriteAllBytes(newImage, ms.GetBuffer());                        
                    }
                }
            }

            if (File.Exists(sourceImg))
            {
                File.Delete(sourceImg);
            }
        }
    }
}