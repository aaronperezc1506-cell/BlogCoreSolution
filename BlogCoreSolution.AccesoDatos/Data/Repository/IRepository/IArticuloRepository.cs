using BlogCoreSolution.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BlogCoreSolution.AccesoDatos.Data.Repository.IRepository
{
    public interface IArticuloRepository : IRepository<Articulo>
    {
        void Update(Articulo articulo);      
    }

    namespace BlogCoreSolution.AccesoDatos.Repository
    {
        public interface IArticuloRepository : IRepository<Articulo>
        {
            // Método que permite realizar consultas LINQ avanzadas sobre la colección de artículos
            IQueryable<Articulo> AsQueryable();
        }
    }
}
