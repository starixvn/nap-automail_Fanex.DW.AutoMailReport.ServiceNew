using Fanex.SabaSoccerLetstalk.Business.Models;
using Spire.Xls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface ImageService
    {
        string ExportImage(string excelPath, int batchIndex, string sheetNameNeedToExport, ImageProperty imageProperty);
    }

    public class ImageServiceImp : ImageService
    {
        private readonly DateRangeService dateRangeService;

        public ImageServiceImp(DateRangeService dateRangeService)
        {
            this.dateRangeService = dateRangeService;
        }

        public string ExportImage(string excelPath, int batchIndex, string sheetNameNeedToExport, ImageProperty imageProperty)
        {
            var date = dateRangeService.RptDate;
            var imageName = $@"{AppConfigService.Config.InternalPath}\{date:yyyy}\{date:MM}";

            if (!Directory.Exists(imageName))
            {
                Directory.CreateDirectory(imageName);
            }

            imageName = $@"{imageName}\{imageProperty.ImageName}_Batch_{batchIndex + 1}_On_{date:yyyyMMdd}.jpg";
            var imageNameFinal = $@"{AppConfigService.Config.InternalPath}\{date:yyyy}\{date:MM}\{imageProperty.ImageName}_Batch_{batchIndex + 1}_Final_On_{date:yyyyMMdd}.jpg";
            var workbook = new Workbook();
            workbook.OpenPassword = AppConfigService.Config.Password;
            workbook.LoadFromFile(excelPath, ExcelVersion.Version2013);
            workbook.DocumentProperties.Clear();
            var sheet = workbook.Worksheets[sheetNameNeedToExport];
            MemoryStream ms = new MemoryStream();
            sheet.ToEMFStream(ms, imageProperty.FirstRow, imageProperty.FirstColumn, imageProperty.LastRow, imageProperty.LastColumn);
            Bitmap images = ResetResolution(Image.FromStream(ms) as Metafile, 300);
            images.Save(imageName, ImageFormat.Jpeg);

            Crop(imageName, imageNameFinal, imageProperty.CropWidth, imageProperty.CropHeight, imageProperty.X, imageProperty.Y);

            return imageNameFinal;
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