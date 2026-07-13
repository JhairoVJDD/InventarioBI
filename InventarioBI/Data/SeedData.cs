using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InventarioBI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace InventarioBI.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Roles
            string[] roles = { "Admin", "Supervisor", "Usuario" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Admin User
            var adminUser = await userManager.FindByEmailAsync("admin@inventario.com");
            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin@inventario.com",
                    Email = "admin@inventario.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Admin123*");
                await userManager.AddToRoleAsync(user, "Admin");
            }

            // 3. Tiendas
            if (!await db.Tiendas.AnyAsync())
            {
                var tiendas = new[]
                {
                    new Tienda { Nombre = "Tienda Principal - Lima", Distrito = "Miraflores", Region = "Lima", AreaM2 = 120.50m },
                    new Tienda { Nombre = "Tienda Express - Norte", Distrito = "Los Olivos", Region = "Lima", AreaM2 = 45.00m },
                    new Tienda { Nombre = "Almacén Central", Distrito = "Ate", Region = "Lima", AreaM2 = 350.00m }
                };

                await db.Tiendas.AddRangeAsync(tiendas);
                await db.SaveChangesAsync();
            }

            // 4. Movimientos e Inventario / Conteos Físicos (dependen de que existan productos en la base de datos)
            var productos = await db.Productos.Take(5).ToListAsync();
            var tiendasList = await db.Tiendas.ToListAsync();

            if (productos.Any() && tiendasList.Any())
            {
                var tiendaPrincipal = tiendasList.FirstOrDefault(t => t.Nombre.Contains("Principal")) ?? tiendasList[0];
                var tiendaExpress = tiendasList.FirstOrDefault(t => t.Nombre.Contains("Express")) ?? tiendasList[0];
                var almacenCentral = tiendasList.FirstOrDefault(t => t.Nombre.Contains("Almacén")) ?? tiendasList[0];                // Sembrar MovimientosInventario
                var movCount = await db.MovimientosInventario.CountAsync();
                if (movCount < 10)
                {
                    db.MovimientosInventario.RemoveRange(db.MovimientosInventario);
                    await db.SaveChangesAsync();

                    var producto1 = productos[0];
                    var producto2 = productos.Count > 1 ? productos[1] : productos[0];
                    var producto3 = productos.Count > 2 ? productos[2] : productos[0];
                    var producto4 = productos.Count > 3 ? productos[3] : productos[0];
                    var producto5 = productos.Count > 4 ? productos[4] : productos[0];

                    var movimientos = new[]
                    {
                        new MovimientoInventario
                        {
                            IdProducto = producto1.IdProducto,
                            Producto = producto1,
                            IdTienda = tiendaPrincipal.IdTienda,
                            TipoMovimiento = "ENTRADA",
                            Cantidad = 100.000m,
                            StockAnterior = 0.000m,
                            StockNuevo = 100.000m,
                            Motivo = "Carga inicial de stock",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-10)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto2.IdProducto,
                            Producto = producto2,
                            IdTienda = tiendaExpress.IdTienda,
                            TipoMovimiento = "ENTRADA",
                            Cantidad = 50.000m,
                            StockAnterior = 0.000m,
                            StockNuevo = 50.000m,
                            Motivo = "Reabastecimiento Tienda Express",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-9)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto3.IdProducto,
                            Producto = producto3,
                            IdTienda = almacenCentral.IdTienda,
                            TipoMovimiento = "ENTRADA",
                            Cantidad = 200.000m,
                            StockAnterior = 0.000m,
                            StockNuevo = 200.000m,
                            Motivo = "Ingreso de importación",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-8)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto1.IdProducto,
                            Producto = producto1,
                            IdTienda = tiendaPrincipal.IdTienda,
                            TipoMovimiento = "SALIDA",
                            Cantidad = 15.000m,
                            StockAnterior = 100.000m,
                            StockNuevo = 85.000m,
                            Motivo = "Ventas registradas POS",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-7)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto2.IdProducto,
                            Producto = producto2,
                            IdTienda = tiendaExpress.IdTienda,
                            TipoMovimiento = "SALIDA",
                            Cantidad = 10.000m,
                            StockAnterior = 50.000m,
                            StockNuevo = 40.000m,
                            Motivo = "Ventas registradas POS",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-6)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto4.IdProducto,
                            Producto = producto4,
                            IdTienda = tiendaPrincipal.IdTienda,
                            TipoMovimiento = "ENTRADA",
                            Cantidad = 80.000m,
                            StockAnterior = 0.000m,
                            StockNuevo = 80.000m,
                            Motivo = "Carga inicial de stock",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-5)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto5.IdProducto,
                            Producto = producto5,
                            IdTienda = almacenCentral.IdTienda,
                            TipoMovimiento = "ENTRADA",
                            Cantidad = 120.000m,
                            StockAnterior = 0.000m,
                            StockNuevo = 120.000m,
                            Motivo = "Ingreso por lote nuevo",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-4)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto3.IdProducto,
                            Producto = producto3,
                            IdTienda = almacenCentral.IdTienda,
                            TipoMovimiento = "SALIDA",
                            Cantidad = 40.000m,
                            StockAnterior = 200.000m,
                            StockNuevo = 160.000m,
                            Motivo = "Despacho a sucursales",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-3)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto4.IdProducto,
                            Producto = producto4,
                            IdTienda = tiendaPrincipal.IdTienda,
                            TipoMovimiento = "MERMA",
                            Cantidad = 2.000m,
                            StockAnterior = 80.000m,
                            StockNuevo = 78.000m,
                            Motivo = "Producto defectuoso detectado en almacén",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-3)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto5.IdProducto,
                            Producto = producto5,
                            IdTienda = almacenCentral.IdTienda,
                            TipoMovimiento = "AJUSTE",
                            Cantidad = 5.000m,
                            StockAnterior = 120.000m,
                            StockNuevo = 125.000m,
                            Motivo = "Ajuste por inventario sobrante",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-2)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto1.IdProducto,
                            Producto = producto1,
                            IdTienda = tiendaExpress.IdTienda,
                            TipoMovimiento = "ENTRADA",
                            Cantidad = 30.000m,
                            StockAnterior = 0.000m,
                            StockNuevo = 30.000m,
                            Motivo = "Transferencia entre tiendas",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-2)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto2.IdProducto,
                            Producto = producto2,
                            IdTienda = tiendaPrincipal.IdTienda,
                            TipoMovimiento = "SALIDA",
                            Cantidad = 8.000m,
                            StockAnterior = 100.000m,
                            StockNuevo = 92.000m,
                            Motivo = "Ventas del fin de semana",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-1)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto3.IdProducto,
                            Producto = producto3,
                            IdTienda = tiendaExpress.IdTienda,
                            TipoMovimiento = "MERMA",
                            Cantidad = 1.000m,
                            StockAnterior = 40.000m,
                            StockNuevo = 39.000m,
                            Motivo = "Merma por ruptura de empaque",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddDays(-1)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto1.IdProducto,
                            Producto = producto1,
                            IdTienda = tiendaPrincipal.IdTienda,
                            TipoMovimiento = "SALIDA",
                            Cantidad = 10.000m,
                            StockAnterior = 85.000m,
                            StockNuevo = 75.000m,
                            Motivo = "Ventas del fin de semana",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddHours(-18)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto4.IdProducto,
                            Producto = producto4,
                            IdTienda = tiendaExpress.IdTienda,
                            TipoMovimiento = "AJUSTE",
                            Cantidad = 3.000m,
                            StockAnterior = 15.000m,
                            StockNuevo = 12.000m,
                            Motivo = "Ajuste por conteo físico faltante",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddHours(-12)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto5.IdProducto,
                            Producto = producto5,
                            IdTienda = tiendaPrincipal.IdTienda,
                            TipoMovimiento = "SALIDA",
                            Cantidad = 4.000m,
                            StockAnterior = 30.000m,
                            StockNuevo = 26.000m,
                            Motivo = "Venta POS",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddHours(-6)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto3.IdProducto,
                            Producto = producto3,
                            IdTienda = almacenCentral.IdTienda,
                            TipoMovimiento = "ENTRADA",
                            Cantidad = 50.000m,
                            StockAnterior = 160.000m,
                            StockNuevo = 210.000m,
                            Motivo = "Ingreso de proveedor local",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddHours(-2)
                        },
                        new MovimientoInventario
                        {
                            IdProducto = producto2.IdProducto,
                            Producto = producto2,
                            IdTienda = tiendaExpress.IdTienda,
                            TipoMovimiento = "SALIDA",
                            Cantidad = 3.000m,
                            StockAnterior = 40.000m,
                            StockNuevo = 37.000m,
                            Motivo = "Venta POS rápida",
                            UsuarioResponsable = "admin@inventario.com",
                            Fecha = DateTime.Now.AddMinutes(-30)
                        }
                    };

                    await db.MovimientosInventario.AddRangeAsync(movimientos);
                }

                // Sembrar ConteosFisicos
                var conteoCount = await db.ConteosFisicos.CountAsync();
                if (conteoCount < 10)
                {
                    // Limpiar existentes para evitar duplicados y forzar carga completa de 12 registros
                    db.ConteosFisicos.RemoveRange(db.ConteosFisicos);
                    await db.SaveChangesAsync();

                    var producto1 = productos[0];
                    var producto2 = productos.Count > 1 ? productos[1] : productos[0];
                    var producto3 = productos.Count > 2 ? productos[2] : productos[0];

                    var listToAdd = new List<ConteoFisico>
                    {
                        new ConteoFisico
                        {
                            IdProducto = producto1.IdProducto,
                            Producto = producto1,
                            IdTienda = tiendaPrincipal.IdTienda,
                            StockSistema = 50.000m,
                            StockFisico = 50.000m,
                            Estado = "Resuelto",
                            Comentario = "Conteo coincide perfectamente con el sistema.",
                            FechaConteo = DateTime.Now.AddDays(-6)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto2.IdProducto,
                            Producto = producto2,
                            IdTienda = tiendaExpress.IdTienda,
                            StockSistema = 25.000m,
                            StockFisico = 23.000m,
                            Estado = "Pendiente",
                            Comentario = "Faltan 2 unidades en góndola. Se procede a revisión de cámaras.",
                            FechaConteo = DateTime.Now.AddDays(-5)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto3.IdProducto,
                            Producto = producto3,
                            IdTienda = almacenCentral.IdTienda,
                            StockSistema = 12.000m,
                            StockFisico = 12.000m,
                            Estado = "Resuelto",
                            Comentario = "Auditoría en Almacén Central sin novedades.",
                            FechaConteo = DateTime.Now.AddDays(-4)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto2.IdProducto,
                            Producto = producto2,
                            IdTienda = tiendaPrincipal.IdTienda,
                            StockSistema = 15.000m,
                            StockFisico = 14.000m,
                            Estado = "Pendiente",
                            Comentario = "Diferencia de 1 unidad. En investigación.",
                            FechaConteo = DateTime.Now.AddDays(-3)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto1.IdProducto,
                            Producto = producto1,
                            IdTienda = tiendaExpress.IdTienda,
                            StockSistema = 40.000m,
                            StockFisico = 42.000m,
                            Estado = "Pendiente",
                            Comentario = "Sobrante de 2 unidades en Tienda Express.",
                            FechaConteo = DateTime.Now.AddDays(-2)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto3.IdProducto,
                            Producto = producto3,
                            IdTienda = tiendaPrincipal.IdTienda,
                            StockSistema = 10.000m,
                            StockFisico = 10.000m,
                            Estado = "Resuelto",
                            Comentario = "Conteo físico coincide.",
                            FechaConteo = DateTime.Now.AddDays(-1)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto2.IdProducto,
                            Producto = producto2,
                            IdTienda = almacenCentral.IdTienda,
                            StockSistema = 20.000m,
                            StockFisico = 18.000m,
                            Estado = "Pendiente",
                            Comentario = "Diferencia de 2 unidades en Almacén Central.",
                            FechaConteo = DateTime.Now.AddHours(-18)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto1.IdProducto,
                            Producto = producto1,
                            IdTienda = almacenCentral.IdTienda,
                            StockSistema = 80.000m,
                            StockFisico = 80.000m,
                            Estado = "Resuelto",
                            Comentario = "Conteo de stock valorizado coincide.",
                            FechaConteo = DateTime.Now.AddHours(-12)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto3.IdProducto,
                            Producto = producto3,
                            IdTienda = tiendaExpress.IdTienda,
                            StockSistema = 5.000m,
                            StockFisico = 4.000m,
                            Estado = "Investigado",
                            Comentario = "Investigación en curso por merma sospechosa.",
                            FechaConteo = DateTime.Now.AddHours(-8)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto2.IdProducto,
                            Producto = producto2,
                            IdTienda = tiendaExpress.IdTienda,
                            StockSistema = 100.000m,
                            StockFisico = 100.000m,
                            Estado = "Resuelto",
                            Comentario = "Coincidencia perfecta en inventario mensual.",
                            FechaConteo = DateTime.Now.AddHours(-4)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto1.IdProducto,
                            Producto = producto1,
                            IdTienda = tiendaPrincipal.IdTienda,
                            StockSistema = 30.000m,
                            StockFisico = 25.000m,
                            Estado = "Pendiente",
                            Comentario = "Faltante de 5 unidades. En revisión por Supervisor.",
                            FechaConteo = DateTime.Now.AddMinutes(-90)
                        },
                        new ConteoFisico
                        {
                            IdProducto = producto3.IdProducto,
                            Producto = producto3,
                            IdTienda = almacenCentral.IdTienda,
                            StockSistema = 15.000m,
                            StockFisico = 15.000m,
                            Estado = "Resuelto",
                            Comentario = "Coincide con sistema.",
                            FechaConteo = DateTime.Now.AddMinutes(-30)
                        }
                    };

                    await db.ConteosFisicos.AddRangeAsync(listToAdd);
                }

                await db.SaveChangesAsync();
            }
        }
    }
}