using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUnder.Data;
using MUnder.Models;
<<<<<<< HEAD
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
=======
using Microsoft.AspNetCore.Identity;

>>>>>>> 25a9ac242342f34a08c853dcdb8c0185c19725e6

var builder = WebApplication.CreateBuilder(args);

<<<<<<< HEAD
// DbContext
=======
// Configuración de DbContext con SQLite
>>>>>>> 25a9ac242342f34a08c853dcdb8c0185c19725e6
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

<<<<<<< HEAD
// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuración de contraseña
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// IMPORTANTE: Configuración de cookies de autenticación
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
=======
// Configuración de Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // aquí podés ajustar políticas de contraseña, bloqueo, etc.
})
.AddEntityFrameworkStores<ApplicationDbContext>();
>>>>>>> 25a9ac242342f34a08c853dcdb8c0185c19725e6

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

<<<<<<< HEAD
app.UseHttpsRedirection();
app.UseStaticFiles();

=======
app.UseStaticFiles();
>>>>>>> 25a9ac242342f34a08c853dcdb8c0185c19725e6
app.UseRouting();

// IMPORTANTE: El orden es crítico
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Redirigir raíz a Login
app.MapGet("/", () => Results.Redirect("/Login"));

app.Run();