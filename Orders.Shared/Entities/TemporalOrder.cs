using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Shared.Entities
{
    public class TemporalOrder
    {
        public int Id { get; set; }
        public User? User { get; set; }
        public string? UserId { get; set; }
        public Product? Product { get; set; }
        public int? ProductId { get; set; }
        [Display(Name = "Cantidad")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public float Quantity { get; set; }

        [Display(Name = "Comentarios")]
        [DataType(DataType.MultilineText)]
        public string? Remarks { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value => Product is null ? 0 : Product.Price * (decimal)Quantity;
    }
}
