using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlogCoreSolution.Models
{
    public class Categoria
    {
        public Categoria()
        {
            FechaCreacion = DateTime.Now;
        }


        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de categoría es obligatorio")]
        [MaxLength(60)]
        [Display(Name = "Nombre de Categoría")]
        public string Nombre { get; set; }

        [Display(Name = "Orden de Visualización")]
        [Range(1, 10, ErrorMessage = "El valor debe estar entre 1 y 100")]
        public int Orden { get; set; }

        public DateTime FechaCreacion { get; set; }

    }
}
