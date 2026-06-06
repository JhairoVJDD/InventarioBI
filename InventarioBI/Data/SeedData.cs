using Microsoft.AspNetCore.Identity;

namespace InventarioBI.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Supervisor", "Usuario" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Crear usuario Admin por defecto
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
        }
    }
}