using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BlogCoreSolution.Models
{
    public class Articulo
    {
        public Articulo()
        {
            FechaCreacion = DateTime.Now;
        }


        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del artículo es obligatorio")]
        [Display(Name = "Nombre del Artículo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El nombre del artículo es obligatorio")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Imagen")]
        public string UrlImagen { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }
    }
}
