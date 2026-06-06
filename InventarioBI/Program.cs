using InventarioBI.Data;
using InventarioBI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Registrar servicios propios antes de Build
builder.Services.AddScoped<ExcelService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   
app.UseAuthorization();   

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapRazorPages();       // ← Necesario para Identity
// Apply any pending migrations, then seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var db = serviceProvider.GetRequiredService<ApplicationDbContext>();
    // Apply migrations (creates database if it does not exist)
    db.Database.Migrate();

    await SeedData.Initialize(serviceProvider);
}

app.Run();