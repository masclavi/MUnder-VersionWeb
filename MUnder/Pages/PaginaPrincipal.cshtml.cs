using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MUnder.Models;

namespace MUnder.Pages
{
    // [Authorize]
    public class PaginaPrincipalModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public string UserName { get; set; }

        public PaginaPrincipalModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            // DEBUGGING
            Console.WriteLine($"=== INDEX PAGE ===");
            Console.WriteLine($"Usuario autenticado: {User.Identity.IsAuthenticated}");
            Console.WriteLine($"Usuario nombre: {User.Identity.Name}");

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                Console.WriteLine($"Usuario encontrado: {user.Email}");
                UserName = !string.IsNullOrEmpty(user.DisplayName)
                    ? user.DisplayName
                    : user.Email;
            }
            else
            {
                Console.WriteLine("Usuario NO encontrado");
                UserName = "Invitado";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Login");
        }
    }
}