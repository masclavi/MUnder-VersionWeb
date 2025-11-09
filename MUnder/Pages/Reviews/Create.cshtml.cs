using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MUnder.Data;
using MUnder.Models;
using MUnder.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MUnder.Pages.Reviews
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public ReviewViewModel ReviewViewModel { get; set; } = new ReviewViewModel();

        public async Task<IActionResult> OnGetAsync(int songId)
        {
            var song = await _context.Songs.FindAsync(songId);
            if (song == null)
            {
                return NotFound();
            }

            // Verificar si el usuario ya tiene una reseña para esta canción
            var userId = _userManager.GetUserId(User);
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.SongId == songId && r.UserId == userId);

            if (existingReview != null)
            {
                // Si ya existe, cargar los datos para editar
                ReviewViewModel = new ReviewViewModel
                {
                    Id = existingReview.Id,
                    Rating = existingReview.Rating,
                    Comment = existingReview.Comment,
                    SongId = songId,
                    SongTitle = song.Title,
                    SongArtist = song.Artist
                };
            }
            else
            {
                // Nueva reseña
                ReviewViewModel.SongId = songId;
                ReviewViewModel.SongTitle = song.Title;
                ReviewViewModel.SongArtist = song.Artist;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var song = await _context.Songs.FindAsync(ReviewViewModel.SongId);
                if (song != null)
                {
                    ReviewViewModel.SongTitle = song.Title;
                    ReviewViewModel.SongArtist = song.Artist;
                }
                return Page();
            }

            var userId = _userManager.GetUserId(User);

            // Verificar si ya existe una reseña
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.SongId == ReviewViewModel.SongId && r.UserId == userId);

            if (existingReview != null)
            {
                // Actualizar reseña existente
                existingReview.Rating = ReviewViewModel.Rating;
                existingReview.Comment = ReviewViewModel.Comment;
                existingReview.CreatedAt = System.DateTime.Now;
            }
            else
            {
                // Crear nueva reseña
                var review = new Review
                {
                    Rating = ReviewViewModel.Rating,
                    Comment = ReviewViewModel.Comment,
                    SongId = ReviewViewModel.SongId,
                    UserId = userId,
                    CreatedAt = System.DateTime.Now
                };
                _context.Reviews.Add(review);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Songs/Details", new { id = ReviewViewModel.SongId });
        }
    }
}