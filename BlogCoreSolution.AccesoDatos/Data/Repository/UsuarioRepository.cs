using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
namespace BlogCoreSolution.AccesoDatos.Data.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        public UsuarioRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }
        public IEnumerable<ApplicationUser> ObtenerTodos(string idUsuarioActual)
        {
            return _db.Users
            .Where(u => u.Id != idUsuarioActual)
            .ToList();
        }
        public ApplicationUser? ObtenerUsuario(string idUsuario)
        {
            return _db.Users.FirstOrDefault(u => u.Id == idUsuario);
        }
        public void BloquearUsuario(string IdUsuario)
        {
            var usuario = _db.Users.FirstOrDefault(u => u.Id == IdUsuario);
            if (usuario != null)
            {
                usuario.LockoutEnd = DateTime.Now.AddYears(100);
                _db.SaveChanges();
            }
        }
        public void DesbloquearUsuario(string IdUsuario)
        {
            var usuario = _db.Users.FirstOrDefault(u => u.Id == IdUsuario);
            if (usuario != null)
            {
                usuario.LockoutEnd = DateTime.Now;
                _db.SaveChanges();
            }
        }
    }
}
