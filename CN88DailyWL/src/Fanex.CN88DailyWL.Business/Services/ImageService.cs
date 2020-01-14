using Spire.Xls;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Fanex.CN88DailyWL.Business.Services
{
    public interface ImageService
    {
        List<string> ExportImage(string excelPath);
    }

    public class ImageServiceImp : ImageService
    {
        private readonly DateService dateService;
        public ImageServiceImp(DateService dateService)
        {
            this.dateService = dateService;
        }
        
        public List<string> ExportImage(string excelPath)
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

            const int FirstRowEn = 2;
            const int FirstColumnEn = 1;
            const int LastRowEn = 34;
            const int LastColumnEn = 5;

            const int FirstRowCn = 36;
            const int FirstColumnCn = 1;
            const int LastRowCn = 68;
            const int LastColumnCn = 5;

            const int NewWidth = 520;
            const int NewHeight = 1000;
            const int X = 115;
            const int Y = 100;
            
            var workbook = new Workbook();
            workbook.LoadFromFile(excelPath);
            var sheet = workbook.Worksheets[0];

            var imageEn = sheet.ToImage(FirstRowEn, FirstColumnEn, LastRowEn, LastColumnEn);
            var newImgEn = ResizeImage(imageEn, 700, 1200);
            newImgEn.Save(imageNameEn, ImageFormat.Png);

            var imageCn = sheet.ToImage(FirstRowCn, FirstColumnCn, LastRowCn, LastColumnCn);
            var newImgCn = ResizeImage(imageCn, 700, 1200);
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
