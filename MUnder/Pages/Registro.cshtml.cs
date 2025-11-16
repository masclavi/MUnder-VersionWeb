using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MUnder.Models;

namespace MUnder.Pages
{
    public class RegistroModel : PageModel
    {
        // Servicios de Identity
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // Constructor
        public RegistroModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Propiedad para mostrar errores
        public string ErrorMessage { get; set; }

        // Método GET: cuando cargas la página
        public void OnGet()
        {
            // Nada especial por ahora
        }

        // Método POST: cuando hacés clic en "Registrarse"
        public async Task<IActionResult> OnPostAsync(
            string nombre,
            string email,
            string password,
            string telefono)
        {
            // Validaciones básicas
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Por favor, completá todos los campos obligatorios";
                return Page();
            }

            // Crear el nuevo usuario
            var user = new ApplicationUser
            {
                UserName = email, // Identity usa UserName como identificador único
                Email = email,
                DisplayName = nombre,
                PhoneNumber = telefono
            };

            // Intentar crear el usuario en la base de datos
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Usuario creado exitosamente
                // Guardar mensaje de éxito para mostrar en la página de login
                TempData["SuccessMessage"] = "¡Usuario registrado exitosamente! Ahora podés iniciar sesión.";

                // Redirigir al login
                return RedirectToPage("/Login");
            }
            else
            {
                // Hubo errores al crear el usuario
                // Mostrar el primer error
                ErrorMessage = result.Errors.FirstOrDefault()?.Description
                    ?? "Error al registrar el usuario";

                // Si el error es sobre email duplicado, hacerlo más claro
                if (ErrorMessage.Contains("already taken") || ErrorMessage.Contains("ya existe"))
                {
                    ErrorMessage = "Ya existe un usuario con ese correo";
                }

                return Page();
            }
        }
    }
}