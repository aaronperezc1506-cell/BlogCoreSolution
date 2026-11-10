using System;
using System.Collections.Generic;
using System.Text;


namespace BlogCoreSolution.Models
{
    public class Pelicula
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Imagen { get; set; } = string.Empty;
    }
}
