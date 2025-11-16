using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MUnder.Data;
using MUnder.Models;
using System.Security.Claims;

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

        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                FavoriteSongs = new List<Song>();
                return;
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                UserName = !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Email ?? "Usuario";
            }

            FavoriteSongs = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Song)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => f.Song)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostRemoveFavoriteAsync(int songId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { success = false, message = "Usuario no autenticado" })
                {
                    StatusCode = 401
                };
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, isFavorite = false, message = "Eliminado de favoritos" });
            }

            return new JsonResult(new { success = false, message = "Favorito no encontrado" })
            {
                StatusCode = 404
            };
        }
    }
}