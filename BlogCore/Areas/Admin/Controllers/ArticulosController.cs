using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models.ViewModels;
using BlogCoreSolution.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = CNT.Administrador)]
    [Area("Admin")]
    public class ArticulosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ArticulosController(IContenedorTrabajo contenedorTrabajo,
            IWebHostEnvironment hostingEnvironment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ArticuloVM artiVM = new ArticuloVM()
            {
                Articulo = new BlogCoreSolution.Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategoria()
            };            
            
            return View(artiVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticuloVM artiVM)
        {
            // UrlImagen se genera en el servidor después de recibir el archivo.
            // ASP.NET puede marcarla como requerida antes de que exista; se valida aquí manualmente.
            ModelState.Remove("Articulo.UrlImagen");
            var archivos = HttpContext.Request.Form.Files;

            if (ModelState.IsValid && artiVM.Articulo.Id == 0 && archivos.Count > 0)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var subidas = Path.Combine(rutaPrincipal, "imagenes", "articulos");
                Directory.CreateDirectory(subidas);
                string nombreArchivo = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(archivos[0].FileName);
                using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStreams);
                }
                artiVM.Articulo.UrlImagen = "/imagenes/articulos/" + nombreArchivo + extension;
                _contenedorTrabajo.Articulo.Add(artiVM.Articulo);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (archivos.Count == 0)
                    ModelState.AddModelError("Articulo.UrlImagen", "Debes seleccionar una imagen.");
            }

            artiVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategoria();
            return View(artiVM);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ArticuloVM artiVM = new ArticuloVM()
            {
                Articulo = new BlogCoreSolution.Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategoria()
            };

            if (id != null)
            {
                artiVM.Articulo = _contenedorTrabajo.Articulo.Get(id.GetValueOrDefault());
            }

            return View(artiVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ArticuloVM artiVM)
        {
            ModelState.Remove("Articulo.UrlImagen");
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                Directory.CreateDirectory(Path.Combine(rutaPrincipal, "imagenes", "articulos"));
                var archivos = HttpContext.Request.Form.Files;

                var articuloDesdeBd = _contenedorTrabajo.Articulo.Get(artiVM.Articulo.Id);

                if (archivos.Count > 0)
                {
                    //Nuevo imagen para el artículo
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, "imagenes", "articulos");
                    Directory.CreateDirectory(subidas);
                    var extension = Path.GetExtension(archivos[0].FileName);
                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);

                    var rutaImagen = Path.Combine(rutaPrincipal,
                        (articuloDesdeBd.UrlImagen ?? "").TrimStart('/', '\\'));

                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }

                    //Nuevamente subimos el archivo
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    artiVM.Articulo.UrlImagen = "/imagenes/articulos/" + nombreArchivo + extension;
                }
                else
                {
                    //Aquí sería cuando la imagen ya existe y se conserva
                    artiVM.Articulo.UrlImagen = articuloDesdeBd.UrlImagen;
                }

                _contenedorTrabajo.Articulo.Update(artiVM.Articulo);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            artiVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategoria();
            return View(artiVM);
        }



        #region Llamadas a la API
        public IActionResult GetAll()
        {
            var articulos = _contenedorTrabajo.Articulo.GetAll(includeProperties: "Categoria")
                .Select(a => new {
                    id = a.Id,
                    nombre = a.Nombre,
                    categoria = new { nombre = a.Categoria != null ? a.Categoria.Nombre : "" },
                    urlImagen = a.UrlImagen,
                    fechaCreacion = a.FechaCreacion
                }).ToList();
            return Json(new { data = articulos });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Articulo.Get(id);
            if (objFromDb == null)
                return Json(new { success = false, message = "No se encontró el artículo." });

            string rutaDirectorioPrincipal = _hostingEnvironment.WebRootPath
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var rutaImagen = Path.Combine(rutaDirectorioPrincipal,
                (objFromDb.UrlImagen ?? "").TrimStart('/', '\\'));
            if (System.IO.File.Exists(rutaImagen))
                System.IO.File.Delete(rutaImagen);

            _contenedorTrabajo.Articulo.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Artículo borrado correctamente." });
        }

        #endregion
    }
}
