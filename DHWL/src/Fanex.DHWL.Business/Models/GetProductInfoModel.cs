namespace Fanex.DHWL.Business.Models
{
    public class GetProductInfoModel
    {
        public int OrderByProduct { get; set; }

        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductShortName { get; set; }
    }
}