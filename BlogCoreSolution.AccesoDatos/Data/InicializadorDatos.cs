using BlogCoreSolution.Models;
using BlogCoreSolution.Utilidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogCoreSolution.AccesoDatos.Data
{
    public static class InicializadorDatos
    {
        public static async Task InicializarAsync(IServiceProvider services)
        {
            var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
            var users = services.GetRequiredService<UserManager<ApplicationUser>>();
            var db = services.GetRequiredService<ApplicationDbContext>();

            foreach (var role in new[] { CNT.Administrador, CNT.Registrado, CNT.Cliente })
                if (!await roles.RoleExistsAsync(role))
                    await roles.CreateAsync(new IdentityRole(role));

            await CrearUsuario(users, "admin@mitag.com", "Admin123*", "Administrador", CNT.Administrador);
            await CrearUsuario(users, "editor@mitag.com", "Editor123*", "Editor", CNT.Registrado);
            await CrearUsuario(users, "cliente@mitag.com", "Cliente123*", "Cliente", CNT.Cliente);

            if (!await db.Categorias.AnyAsync())
            {
                var categorias = new[]
                {
                    new Categoria { Nombre = "Celulares y tablets", Orden = 1 },
                    new Categoria { Nombre = "Computadoras", Orden = 2 },
                    new Categoria { Nombre = "Audio y video", Orden = 3 },
                    new Categoria { Nombre = "Accesorios y gaming", Orden = 4 }
                };
                db.Categorias.AddRange(categorias);
                await db.SaveChangesAsync();
            }

            // Registros de demostración para que la página no aparezca vacía al iniciar.
            // El administrador puede reemplazarlos o ampliarlos desde /Admin.
            if (!await db.Articulos.AnyAsync())
            {
                var cats = await db.Categorias.OrderBy(c => c.Orden).ToListAsync();
                var nombres = new[]
                {
                    new[] { "Smartphone Android de última generación", "iPhone: guía de compra", "Tablets para estudiar y trabajar", "Celulares con mejor batería", "Qué revisar antes de comprar un teléfono", "Accesorios esenciales para tu celular" },
                    new[] { "Laptop para estudiantes", "Computadoras para diseño", "PC gamer: componentes básicos", "SSD: velocidad para tu equipo", "Cómo elegir memoria RAM", "Mantenimiento de computadoras" },
                    new[] { "Audífonos inalámbricos", "Bocinas Bluetooth para el hogar", "Monitores para entretenimiento", "Teclados y sonido para streaming", "Cómo elegir una pantalla", "Audio para videollamadas" },
                    new[] { "Mouse ergonómico", "Teclados mecánicos", "Controles para videojuegos", "Webcams para clases y reuniones", "Cargadores y cables seguros", "Accesorios para mejorar tu escritorio" }
                };
                var imagenes = new[]
                {
                    "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1593305841991-05c297ba4575?auto=format&fit=crop&w=1200&q=80"
                };
                for (int c = 0; c < cats.Count && c < nombres.Length; c++)
                    for (int i = 0; i < nombres[c].Length; i++)
                        db.Articulos.Add(new Articulo {
                            Nombre = nombres[c][i],
                            CategoriaId = cats[c].Id,
                            UrlImagen = imagenes[c],
                            Descripcion = $"<p>En <strong>Suportsaaron</strong> te presentamos esta guía sobre {nombres[c][i].ToLowerInvariant()}. Conoce sus características, usos recomendados y los aspectos que conviene comparar antes de realizar una compra.</p><p>Revisa la compatibilidad, la garantía, el rendimiento, la conectividad, los materiales y la relación entre precio y prestaciones. Antes de comprar, compara las especificaciones del fabricante, verifica que el equipo sea compatible con los dispositivos que ya utilizas y considera el soporte técnico disponible. La elección ideal depende de tus necesidades, del uso que le darás y de tu presupuesto. Esta ficha es contenido inicial de demostración y puede editarse desde el panel administrativo para incorporar información comercial verificada.</p>"
                        });
                await db.SaveChangesAsync();
            }

            if (!await db.Sliders.AnyAsync())
            {
                db.Sliders.AddRange(
                    new Slider { Nombre = "Tecnología para tu día a día", Estado = true, UrlImagen = "https://images.unsplash.com/photo-1498049794561-7780e7231661" },
                    new Slider { Nombre = "Potencia para estudiar y trabajar", Estado = true, UrlImagen = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=1200&q=80" },
                    new Slider { Nombre = "Audio que te acompaña", Estado = true, UrlImagen = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=1200&q=80" },
                    new Slider { Nombre = "Accesorios para jugar y crear", Estado = true, UrlImagen = "https://images.unsplash.com/photo-1593305841991-05c297ba4575?auto=format&fit=crop&w=1200&q=80" }
                );
                await db.SaveChangesAsync();
            }
        }

        private static async Task CrearUsuario(UserManager<ApplicationUser> users, string email,
            string password, string nombre, string rol)
        {
            var user = await users.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser {
                    UserName = email, Email = email, EmailConfirmed = true,
                    Nombre = nombre, Direccion = "No especificada",
                    Ciudad = "San José", Pais = "Costa Rica"
                };
                var result = await users.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new InvalidOperationException("No se pudo crear " + email + ": " +
                        string.Join("; ", result.Errors.Select(e => e.Description)));
            }
            if (!await users.IsInRoleAsync(user, rol))
                await users.AddToRoleAsync(user, rol);
        }
    }
}
