using Microsoft.AspNetCore.Builder;
<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Identity;
>>>>>>> testAri
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUnder.Data;
using MUnder.Models;
<<<<<<< HEAD
using Microsoft.AspNetCore.Identity;

=======
>>>>>>> testAri

var builder = WebApplication.CreateBuilder(args);

// Configuración de DbContext con SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

<<<<<<< HEAD
// Configuración de Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // aquí podés ajustar políticas de contraseña, bloqueo, etc.
})
.AddEntityFrameworkStores<ApplicationDbContext>();
=======
// Configuración de Identity con ApplicationUser y IdentityRole
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Políticas de contraseña (ajustadas para desarrollo)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
>>>>>>> testAri

    // Políticas de usuario
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false; // Cambia a 'true' si usas confirmación por email
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuración de cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/Login";
    options.AccessDeniedPath = "/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Soporte para Razor Pages
builder.Services.AddRazorPages();

builder.Services.AddControllers();

var app = builder.Build();

// Configuración de pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

<<<<<<< HEAD
=======
app.UseHttpsRedirection();
>>>>>>> testAri
app.UseStaticFiles();
app.UseRouting();

// ¡Importante! El orden: Authentication antes que Authorization
app.UseAuthentication();
app.UseAuthorization();

// Rutas
app.MapRazorPages();

app.MapControllers();

// Redirigir la raíz al login
app.MapGet("/", () => Results.Redirect("/Login"));

app.Run();