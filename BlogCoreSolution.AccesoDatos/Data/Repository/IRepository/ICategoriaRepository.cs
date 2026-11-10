using BlogCoreSolution.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCoreSolution.AccesoDatos.Data.Repository.IRepository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        void Update(Categoria categoria);

        IEnumerable<SelectListItem> GetListaCategoria();


        //¿Por qué el método Update NO va en el repositorio genérico?
        /*
         * Cada entidad se actualiza distinto

            Categoría: Nombre, Orden

            Post: Título, Contenido, Imagen, Fecha

            Usuario: Datos distintos

            No todas las propiedades se deben actualizar

            Algunas se ignoran

            Otras se calculan

            Otras no se tocan nunca

            Evita errores

            Update(entity) genérico puede marcar todo como modificado

            Riesgo de sobrescribir datos sin querer
        */
    }
}
