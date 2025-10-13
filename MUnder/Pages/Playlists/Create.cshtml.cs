using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MUnder.Data;
using MUnder.Models;
using System.Security.Claims;

namespace MUnder.Pages.Playlists
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string? Name { get; set; }

        [BindProperty]
        public string? Description { get; set; }

        public void OnGet()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"[Create.OnGet] Usuario autenticado. UserID: {userId ?? "null"}");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("=== INICIO OnPostAsync ===");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("[Create.OnPost] ModelState inválido");
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Error de validación: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"[Create.OnPost] UserID obtenido: {(userId ?? "null")}");

            if (userId == null)
            {
                Console.WriteLine("[Create.OnPost] ERROR: Usuario no autenticado.");
                return RedirectToPage("/Login");
            }

            // Crear la playlist manualmente
            var playlist = new Playlist
            {
                Name = Name ?? string.Empty,
                Description = Description,
                OwnerId = userId
            };

            try
            {
                _context.Playlists.Add(playlist);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[Create.OnPost] ✅ Playlist guardada con ID: {playlist.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Create.OnPost] ❌ Error al guardar: {ex.Message}");
                ModelState.AddModelError("", "No se pudo guardar la playlist. Inténtalo de nuevo.");
                return Page();
            }

            Console.WriteLine("=== FIN OnPostAsync ===");
            return RedirectToPage("Index");
        }
    }
}