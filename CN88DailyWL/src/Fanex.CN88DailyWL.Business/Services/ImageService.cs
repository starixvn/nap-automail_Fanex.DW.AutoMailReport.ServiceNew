using Fanex.CN88DailyWL.Business.Model;
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
        List<string> ExportImage(string excelPath, ImageProperty imagePropertyEn, ImageProperty imagePropertyCn);
    }

    public class ImageServiceImp : ImageService
    {
        private readonly DateService dateService;
        public ImageServiceImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        public List<string> ExportImage(string excelPath, ImageProperty imagePropertyEn, ImageProperty imagePropertyCn)
        {
            var date = dateService.WinLostDate;
            var imageNameEn = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}";
            var imageNameCn = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}";

            if (!Directory.Exists(imageNameEn))
            {
                Directory.CreateDirectory(imageNameEn);
            }

            imageNameEn = $@"{imageNameEn}\CN88_Daily_WL_EN_{date:ddMMyyyy}.jpg";
            imageNameCn = $@"{imageNameCn}\CN88_Daily_WL_CN_{date:ddMMyyyy}.jpg";

            var imageNameEnFinal = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}\CN88_Daily_WL_EN_Final_{date:ddMMyyyy}.jpg";
            var imageNameCnFinal = $@"{AppConfigService.AppConfig.FilePath}\{date:yyyy}\{date:MM}\CN88_Daily_WL_CN_Final_{date:ddMMyyyy}.jpg";

            //const int FirstRowEn = 2;
            //const int FirstColumnEn = 1;
            //const int LastRowEn = 34;
            //const int LastColumnEn = 5;

            //const int FirstRowCn = 36;
            //const int FirstColumnCn = 1;
            //const int LastRowCn = 68;
            //const int LastColumnCn = 5;

            //const int NewWidth = 520;
            //const int NewHeight = 1000;
            //const int X = 115;
            //const int Y = 100;

            var workbook = new Workbook();
            workbook.LoadFromFile(excelPath, ExcelVersion.Version2013);
            workbook.DocumentProperties.Clear();
            var sheet = workbook.Worksheets[0];

            MemoryStream enms = new MemoryStream();

            sheet.ToEMFStream(enms, imagePropertyEn.FirstRow, imagePropertyEn.FirstColumn, imagePropertyEn.LastRow, imagePropertyEn.LastColumn);

            MemoryStream cnms = new MemoryStream();
            sheet.ToEMFStream(cnms, imagePropertyCn.FirstRow, imagePropertyCn.FirstColumn, imagePropertyCn.LastRow, imagePropertyCn.LastColumn);

            Bitmap enImage = ResetResolution(Image.FromStream(enms) as Metafile, 300);
            Bitmap cnImage = ResetResolution(Image.FromStream(cnms) as Metafile, 300);

            enImage.Save(imageNameEn, ImageFormat.Jpeg);
            cnImage.Save(imageNameCn, ImageFormat.Jpeg);

            Crop(imageNameEn, imageNameEnFinal, imagePropertyEn.CropWidth, imagePropertyEn.CropHeight, imagePropertyEn.X, imagePropertyEn.Y);
            Crop(imageNameCn, imageNameCnFinal, imagePropertyCn.CropWidth, imagePropertyCn.CropHeight, imagePropertyCn.X, imagePropertyCn.Y);

            return new List<string>
            {
                imageNameEnFinal,
                imageNameCnFinal
            };
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

        private static Bitmap ResetResolution(Metafile mf, float resolution)
        {
            int width = (int)(mf.Width * resolution / mf.HorizontalResolution);
            int height = (int)(mf.Height * resolution / mf.VerticalResolution);
            Bitmap bmp = new Bitmap(width, height);
            bmp.SetResolution(resolution, resolution);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(mf, 0, 0);
            g.Dispose();
            return bmp;
        }
    }
}
