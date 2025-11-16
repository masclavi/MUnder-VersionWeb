using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MUnder.Models;

namespace MUnder.Pages
{
    [Authorize]
    public class PaginaPrincipalModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        // Evitar nulls inicializando con un valor por defecto
        public string UserName { get; set; } = "Invitado";

        public PaginaPrincipalModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // OnGetAsync: carga el nombre a mostrar
        public async Task OnGetAsync()
        {
            // Debug (opcional)
            Console.WriteLine("=== INDEX PAGE ===");
            Console.WriteLine($"Usuario autenticado: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"Usuario nombre: {User.Identity?.Name}");

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                Console.WriteLine($"Usuario encontrado: {user.Email}");
                UserName = !string.IsNullOrWhiteSpace(user.DisplayName)
                    ? user.DisplayName
                    : (string.IsNullOrWhiteSpace(user.UserName) ? user.Email ?? "Usuario" : user.UserName);
            }
            else
            {
                Console.WriteLine("Usuario NO encontrado");
                UserName = "Invitado";
            }
        }

        // OnPostAsync: logout — si tu form usa asp-page-handler="Logout" cámbialo a OnPostLogoutAsync
        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Login");
        }
    }
}