using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;


namespace BlogCoreSolution.AccesoDatos.Data.Repository
{
    public class ArticuloRepository : Repository<Articulo>, IArticuloRepository
    {
        private readonly ApplicationDbContext _db;

        public ArticuloRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        // Retorna la entidad de Artículos como consultable para aplicar filtros y paginación
        public IQueryable<Articulo> AsQueryable()
        {
            return _db.Set<Articulo>().AsQueryable();
        }


        public void Update(Articulo articulo)
        {
            var objDesdeDb = _db.Articulos.FirstOrDefault(s => s.Id == articulo.Id);
            if (objDesdeDb == null)
            {
                return;
            }
            objDesdeDb.Nombre = articulo.Nombre;
            objDesdeDb.Descripcion = articulo.Descripcion;
            objDesdeDb.UrlImagen = articulo.UrlImagen;
            objDesdeDb.CategoriaId = articulo.CategoriaId;
        }
    }
}
