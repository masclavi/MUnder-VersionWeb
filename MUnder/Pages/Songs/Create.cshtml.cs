using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MUnder.Data;
using MUnder.Models;

namespace MUnder.Pages.Songs
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string? Title { get; set; }

        [BindProperty]
        public string? Artist { get; set; }

        [BindProperty]
        public string? YouTubeUrl { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Title) ||
                string.IsNullOrWhiteSpace(Artist) ||
                string.IsNullOrWhiteSpace(YouTubeUrl))
            {
                ModelState.AddModelError("", "Todos los campos son obligatorios.");
                return Page();
            }

            var song = new Song
            {
                Title = Title.Trim(),
                Artist = Artist.Trim(),
                YouTubeUrl = YouTubeUrl.Trim()
                // CoverPath usa valor por defecto
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
