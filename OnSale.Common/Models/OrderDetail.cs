using OnSale.Common.Responses;
using System.ComponentModel.DataAnnotations;

namespace OnSale.Common.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public ProductResponse Product { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public float Quantity { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal? Value => (decimal)Quantity * Product?.Price;
    }
}