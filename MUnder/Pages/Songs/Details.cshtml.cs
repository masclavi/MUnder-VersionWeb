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
        public string CurrentUserId { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Obtener la canción con su álbum
            Song = await _context.Songs
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (Song == null)
            {
                return NotFound();
            }

            // Obtener todas las reviews de esta canción con los usuarios
            Reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.SongId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            // Calcular promedio de rating
            if (Reviews.Any())
            {
                AverageRating = Reviews.Average(r => r.Rating);
            }

            // Verificar si el usuario actual ya ha hecho review
            if (User.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = _userManager.GetUserId(User) ?? string.Empty;
                UserHasReviewed = Reviews.Any(r => r.UserId == CurrentUserId);
            }

            return Page();
        }
    }
}