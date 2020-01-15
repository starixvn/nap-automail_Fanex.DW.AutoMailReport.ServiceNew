namespace Fanex.TopLicensee.Service.Models
{
    public class ImageProperty
    {
        public int FirstRow { get; set; }

        public int FirstColumn { get; set; }

        public int LastRow { get; set; }

        public int LastColumn { get; set; }

        public int CropWidth { get; set; }

        public int CropHeight { get; set; }

        public string ImageName { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}