using Spire.Xls;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Fanex.SoccerPerformance.Business.Services
{
    public interface ImageService
    {
        List<string> SaveImage(string excelPath);
    }

    public class ImageServiceImp : ImageService
    {
        private readonly DateServiceProvider dateServiceProvider;

        public ImageServiceImp(DateServiceProvider dateServiceProvider)
        {
            this.dateServiceProvider = dateServiceProvider;
        }

        public List<string> SaveImage(string excelPath)
        {
            var date = dateServiceProvider.Date.Yesterday;
            var imageName = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}";

            if (!Directory.Exists(imageName))
            {
                Directory.CreateDirectory(imageName);
            }

            var imageNameYesterday = $@"{imageName}\SoccerPerformance_{date:yyyyMMdd}.png";
            var imageNameYesterdayFinal = $@"{imageName}\SoccerPerformance_Final_{date:yyyyMMdd}.png";
            var imageNameMTD = $@"{imageName}\MTD_SoccerPerformance_{dateServiceProvider.Date.StartDate:yyyyMMdd}_{dateServiceProvider.Date.EndDate:yyyyMMdd}.png";
            var imageNameMTDFinal = $@"{imageName}\MTD_SoccerPerformance_Final_{dateServiceProvider.Date.StartDate:yyyyMMdd}_{dateServiceProvider.Date.EndDate:yyyyMMdd}.png";

            const int FirstRow = 1;
            const int FirstColumn = 1;
            const int LastRow = 48;
            const int LastColumn = 7;

            const int NewWidth = 890;
            const int NewHeight = 920;
            const int X = 55;
            const int Y = 65;

            var workbook = new Workbook();
            workbook.LoadFromFile(excelPath);
            var yesterdaySheet = workbook.Worksheets[0];
            var mtdSheet = workbook.Worksheets[1];

            var imageYesterday = yesterdaySheet.ToImage(FirstRow, FirstColumn, LastRow, LastColumn);
            var newImgYesterday = ResizeImage(imageYesterday, 1000, 1050);
            newImgYesterday.Save(imageNameYesterday, ImageFormat.Png);

            var imageMTD = mtdSheet.ToImage(FirstRow, FirstColumn, LastRow, LastColumn);
            var newImgMTD = ResizeImage(imageMTD, 1000, 1050);
            newImgMTD.Save(imageNameMTD, ImageFormat.Png);

            Crop(imageNameYesterday, imageNameYesterdayFinal, NewWidth, NewHeight, X, Y);
            Crop(imageNameMTD, imageNameMTDFinal, NewWidth, NewHeight, X, Y);

            return new List<string>
            {
                imageNameYesterdayFinal,
                imageNameMTDFinal
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
