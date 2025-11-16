using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MUnder.Data;
using MUnder.Models;

namespace MUnder.Pages.Favorites
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Song> FavoriteSongs { get; set; } = new List<Song>();
        public string UserName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                UserName = !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Email ?? "Usuario";
            }

            // Obtener canciones favoritas del usuario
            FavoriteSongs = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Song)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => f.Song)
                .ToListAsync();

            return Page();
        }
    }
}