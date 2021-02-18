using Fanex.DHWL.Business.Models;
using Spire.Xls;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Fanex.DHWL.Business.Services
{
    public interface ImageService
    {
        string ExportImage(string excelPath, string sheetNameNeedToExport, ImageProperty imageProperty);
    }

    public class ImageServiceImp : ImageService
    {
        private readonly DateTimeServiceProvider dateTimeService;

        public ImageServiceImp(DateTimeServiceProvider dateTimeService)
        {
            this.dateTimeService = dateTimeService;
        }

        public string ExportImage(string excelPath, string sheetNameNeedToExport, ImageProperty imageProperty)
        {
            var date = dateTimeService.DateTime.ToDate;
            var imageName = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}";

            if (!Directory.Exists(imageName))
            {
                Directory.CreateDirectory(imageName);
            }

            imageName = $@"{imageName}\{imageProperty.ImageName}_On_{date:yyyyMMdd}.jpg";
            var imageNameFinal = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}\{imageProperty.ImageName}_F_{date:yyyyMMdd}.jpg";
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

            if (imageProperty.IsResize)
            {
                var imageNameResizeWidth = $@"{AppConfigService.Config.Path}\{date:yyyy}\{date:MM}\{imageProperty.ImageName}_R_{date:yyyyMMdd}.jpg";
                ResizeImage(imageNameFinal, imageNameResizeWidth, imageProperty.ResizeNewWidth, 0, imageProperty.ImageQuality);
                return imageNameResizeWidth;
            }

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

        private void ResizeImage(string org, string des, int finalWidth, int finalHeight, int imageQuality)
        {
            Bitmap newBMP;
            Graphics graphicTemp;
            Bitmap bmp = new Bitmap(org);

            int iWidth;
            int iHeight;
            if ((finalHeight == 0) && (finalWidth != 0))
            {
                iWidth = finalWidth;
                iHeight = (bmp.Size.Height * iWidth / bmp.Size.Width);
            }
            else if ((finalHeight != 0) && (finalWidth == 0))
            {
                iHeight = finalHeight;
                iWidth = (bmp.Size.Width * iHeight / bmp.Size.Height);
            }
            else
            {
                iWidth = finalWidth;
                iHeight = finalHeight;
            }

            newBMP = new Bitmap(iWidth, iHeight);
            graphicTemp = Graphics.FromImage(newBMP);
            graphicTemp.CompositingMode = CompositingMode.SourceOver;
            graphicTemp.CompositingQuality = CompositingQuality.HighQuality;
            graphicTemp.SmoothingMode = SmoothingMode.HighQuality;
            graphicTemp.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphicTemp.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphicTemp.DrawImage(bmp, 0, 0, iWidth, iHeight);
            graphicTemp.Dispose();
            EncoderParameters encoderParams = new EncoderParameters();
            EncoderParameter encoderParam = new EncoderParameter(Encoder.Quality, imageQuality);
            encoderParams.Param[0] = encoderParam;
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            for (int fwd = 0; fwd <= arrayICI.Length - 1; fwd++)
            {
                if (arrayICI[fwd].FormatDescription.Equals("JPEG"))
                {
                    newBMP.SetResolution(300, 300);
                    newBMP.Save(des, arrayICI[fwd], encoderParams);
                }
            }

            newBMP.Dispose();
            bmp.Dispose();
        }
    }
}