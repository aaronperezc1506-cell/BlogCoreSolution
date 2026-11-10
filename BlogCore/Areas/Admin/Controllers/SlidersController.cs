using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models;
using BlogCoreSolution.Models.ViewModels;
using BlogCoreSolution.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = CNT.Administrador)]
    [Area("Admin")]
    public class SlidersController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public SlidersController(IContenedorTrabajo contenedorTrabajo,
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
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            // La ruta de imagen se asigna en el servidor, no viene como campo de texto del formulario.
            ModelState.Remove("UrlImagen");
            var archivos = HttpContext.Request.Form.Files;
            if (ModelState.IsValid && archivos.Count > 0)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string subidas = Path.Combine(rutaPrincipal, "imagenes", "sliders");
                Directory.CreateDirectory(subidas);
                string nombreArchivo = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(archivos[0].FileName);
                using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStreams);
                }
                slider.UrlImagen = "/imagenes/sliders/" + nombreArchivo + extension;
                _contenedorTrabajo.Slider.Add(slider);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            if (archivos.Count == 0)
                ModelState.AddModelError("UrlImagen", "Debes seleccionar una imagen.");
            return View(slider);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Buscamos el slider en la base de datos por su ID
            var slider = _contenedorTrabajo.Slider.Get(id.GetValueOrDefault());
            if (slider == null)
            {
                return NotFound();
            }
            // Retornamos el modelo directamente a la vista (el slider no requiere ViewModel)
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            ModelState.Remove("UrlImagen");
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostingEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;
                // Obtenemos el registro original desde la base de datos para recuperar su image actual
                 var sliderDesdeBD = _contenedorTrabajo.Slider.Get(slider.Id);
                // ESCENARIO A: El usuario seleccionó y subió una nueva imagen
                if (archivos.Count > 0)
                {
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, "imagenes", "sliders");
                    Directory.CreateDirectory(subidas);
                    var extension = Path.GetExtension(archivos[0].FileName);
                    // Buscamos la ruta física de la imagen antigua en el servidor
                    var rutaImagenAntigua = Path.Combine(rutaPrincipal,
                    sliderDesdeBD.UrlImagen.TrimStart('\\'));
                    // Si el archivo físico anterior existe en la carpeta del servidor, lo eliminamos
                    if (System.IO.File.Exists(rutaImagenAntigua))
                    {
                        System.IO.File.Delete(rutaImagenAntigua);
                    }
                    // Guardamos la nueva imagen en el servidor físico
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo +
                    extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }
                    // Guardamos la nueva ruta relativa en el modelo
                    slider.UrlImagen = "/imagenes/sliders/" + nombreArchivo + extension;
                }
                // ESCENARIO B: No se subió una imagen nueva. Conservamos la que ya tenía
                else
                {
                    slider.UrlImagen = sliderDesdeBD.UrlImagen;
                }
                // Actualizamos en la base de datos a través de la Unidad de Trabajo
                _contenedorTrabajo.Slider.Update(slider);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            // Si falla el modelo, devolvemos la misma vista con el slider para que no se pierdan los campos
            return View(slider);
        }


        #region Llamadas a la API
        public IActionResult GetAll()
        {
            var sliders = _contenedorTrabajo.Slider.GetAll()
                .Select(s => new {
                    id = s.Id,
                    nombre = s.Nombre,
                    estado = s.Estado,
                    urlImagen = s.UrlImagen
                }).ToList();
            return Json(new { data = sliders });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {

            // 1. Buscamos el slider en la base de datos usando el ID
            var sliderDesdeBD = _contenedorTrabajo.Slider.Get(id);
            if (sliderDesdeBD == null)
            {
                return Json(new { success = false, message = "Error borrando slider" });
            }

            // 2. Ruta física del archivo de imagen
            string rutaPrincipal = _hostingEnvironment.WebRootPath
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var rutaImagen = Path.Combine(rutaPrincipal,
                (sliderDesdeBD.UrlImagen ?? "").TrimStart('/', '\\'));

            // 3. Borramos el archivo físico en el servidor si existe
            if (System.IO.File.Exists(rutaImagen))
            {
                System.IO.File.Delete(rutaImagen);
            }

            // 4. Removemos el registro de la base de datos
            _contenedorTrabajo.Slider.Remove(sliderDesdeBD);
            _contenedorTrabajo.Save();

            // Retornamos éxito en formato JSON para que sea procesado por SweetAlert en el JS
            return Json(new { success = true, message = "Slider borrado correctamente" });
        }

        #endregion
    }
}