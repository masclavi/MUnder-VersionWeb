using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MUnder.Models;

namespace MUnder.Pages
{
    public class LoginModel : PageModel
    {
        // Servicios de Identity que necesitamos
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor: recibe los servicios por inyección de dependencias
        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Propiedades para mostrar mensajes
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        // Método GET: se ejecuta cuando cargas la página
        public void OnGet()
        {
            // Si hay un mensaje de éxito en TempData (viene del registro), lo mostramos
            if (TempData["SuccessMessage"] != null)
            {
                SuccessMessage = TempData["SuccessMessage"].ToString();
            }
        }

        // Método POST: se ejecuta cuando hacés clic en "Iniciar Sesión"
        public async Task<IActionResult> OnPostAsync(string email, string password)
        {
            // Validación básica
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Por favor, completá todos los campos";
                return Page();
            }

            // Buscar el usuario por email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ErrorMessage = "Correo o contraseña incorrectos";
                return Page();
            }

            // Intentar hacer login
            // isPersistent: false = no recordar la sesión
            // lockoutOnFailure: false = no bloquear cuenta por intentos fallidos
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                // Login exitoso - redirigir a la página principal
                return RedirectToPage("/PaginaPrincipal");
            }
            else
            {
                ErrorMessage = "Correo o contraseña incorrectos";
                return Page();
            }
        }
    }
}