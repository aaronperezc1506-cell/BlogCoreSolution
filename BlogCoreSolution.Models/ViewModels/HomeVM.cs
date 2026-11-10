using System;
using System.Collections.Generic;
using System.Text;
namespace BlogCoreSolution.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Slider> Sliders { get; set; }
        public IEnumerable<Articulo> ListArticulos { get; set; }

        // Propiedades requeridas para el control de paginación
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

    }
}
