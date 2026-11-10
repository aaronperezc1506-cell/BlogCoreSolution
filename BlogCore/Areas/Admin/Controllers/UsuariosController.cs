using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize(Roles = CNT.Administrador)]
    [Area("Admin")]
    public class UsuariosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        public UsuariosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ////Opción 1: Obtener todos los usuario
            //return View(_contenedorTrabajo.Usuario.ObtenerTodos());

            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            //Opción 2: Obtener todos los usuarios menos el que esté logueado,
            //para no bloquearse el mismo
            var usuarioActual = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            return View(_contenedorTrabajo.Usuario.ObtenerTodos(usuarioActual.Value));

        }

        [HttpGet]
        public IActionResult Bloquear(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _contenedorTrabajo.Usuario.BloquearUsuario(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Desbloquear(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _contenedorTrabajo.Usuario.DesbloquearUsuario(id);
            return RedirectToAction(nameof(Index));
        }
    }
}