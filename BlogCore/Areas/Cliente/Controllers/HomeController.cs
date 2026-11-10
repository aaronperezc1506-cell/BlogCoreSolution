using System;
using System.Diagnostics;
using System.Linq;
using BlogCore.Models;
using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models;
using BlogCoreSolution.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class HomeController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public HomeController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        // ==========================================
        // ACCIÓN GET: Página principal con paginación
        // ==========================================
        [HttpGet]
        public IActionResult Index(int page = 1, int pageSize = 6)
        {
            // Validar los parámetros de paginación
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 6;

            // Obtener todos los artículos
            var articulos = _contenedorTrabajo.Articulo
                .GetAll()
                .AsQueryable();

            // Calcular el total de registros y páginas
            int totalRegistros = articulos.Count();

            int totalPaginas = (int)Math.Ceiling(
                totalRegistros / (double)pageSize
            );

            // Obtener los artículos de la página actual
            var listaArticulos = articulos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Construir el ViewModel
            HomeVM homeVM = new HomeVM()
            {
                Sliders = _contenedorTrabajo.Slider
                .GetAll(s => s.Estado == true)
                .ToList(),

                ListArticulos = listaArticulos,

                PageIndex = page,
                TotalPages = totalPaginas
            };

            ViewBag.IsHome = true;
            ViewBag.Sliders = homeVM.Sliders;

            return View(homeVM);
        }

        // ==========================================
        // ACCIÓN GET: Detalle del artículo
        // ==========================================
        [HttpGet]
        public IActionResult Detalle(int id)
        {
            var articuloDesdeBd = _contenedorTrabajo.Articulo.Get(id);

            if (articuloDesdeBd == null)
            {
                return NotFound();
            }

            return View(articuloDesdeBd);
        }

        // ==========================================
        // ACCIÓN GET: Búsqueda de artículos
        // ==========================================
        [HttpGet]
        public IActionResult ResultadoBusqueda(
            string searchString,
            int page = 1)
        {
            int pageSize = 3;

            if (page < 1)
                page = 1;

            // Obtener los artículos
            IQueryable<Articulo> articulos = _contenedorTrabajo.Articulo
                .GetAll()
                .AsQueryable();

            // Filtrar por nombre o descripción
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                articulos = articulos.Where(a =>
                    (a.Nombre != null &&
                     a.Nombre.Contains(searchString)) ||
                    (a.Descripcion != null &&
                     a.Descripcion.Contains(searchString))
                );
            }

            // Conservar el texto de búsqueda
            ViewData["SearchString"] = searchString;

            // Crear la lista paginada
            var listaPaginada = ListaPaginada<Articulo>.CrearPaginacion(
                articulos,
                page,
                pageSize,
                searchString
            );

            return View(listaPaginada);
        }

        // ==========================================
        // ACCIÓN GET: Página de error
        // ==========================================
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id
                        ?? HttpContext.TraceIdentifier
                }
            );
        }
    }
}