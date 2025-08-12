using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Shared.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string Name { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string Description { get; set; } = null!;

        [Display(Name = "Precio")]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Inventario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public float Stock { get; set; }

        public ICollection<ProductCategory>? ProductCategories { get; set; }

        [Display(Name = "Categorías")]
        public int ProductCategoriesNumber => ProductCategories == null
            ? 0
            : ProductCategories.Count;

        public ICollection<ProductImage>? ProductImages { get; set; }

        [Display(Name = "Imágenes")]
        public int ProductImagesNumber => ProductImages == null
            ? 0
            : ProductImages.Count;

        [Display(Name = "Imagen")]
        public string MainImage => ProductImages == null ? string.Empty : ProductImages.FirstOrDefault()!.Image;

        public ICollection<TemporalOrder>? TemporalOrders { get; set; }
    }
}
