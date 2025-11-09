using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MUnder.Data;
using MUnder.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MUnder.Pages.Songs
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Song Song { get; set; } = default!;
        public List<Review> Reviews { get; set; } = new List<Review>();
        public double AverageRating { get; set; }
        public bool UserHasReviewed { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Cargar la canción con sus reseñas
            Song = await _context.Songs
                .Include(s => s.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Song == null)
            {
                return NotFound();
            }

            // Obtener las reseñas ordenadas por fecha
            Reviews = Song.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            // Calcular promedio de calificaciones
            if (Reviews.Any())
            {
                AverageRating = Reviews.Average(r => r.Rating);
            }

            // Verificar si el usuario actual ya tiene una reseña
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                UserHasReviewed = Reviews.Any(r => r.UserId == userId);
            }

            return Page();
        }
    }
}