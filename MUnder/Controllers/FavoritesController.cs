using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MUnder.Data;
using MUnder.Models;
namespace MUnder.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoritesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/Favorites/Toggle/5
        [HttpPost("Toggle/{songId}")]
        public async Task<IActionResult> ToggleFavorite(int songId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                // Verificar si la canción existe
                var songExists = await _context.Songs.AnyAsync(s => s.Id == songId);
                if (!songExists)
                {
                    return NotFound(new { message = "Canción no encontrada" });
                }

                var existingFavorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);

                if (existingFavorite != null)
                {
                    // Quitar de favoritos
                    _context.Favorites.Remove(existingFavorite);
                    await _context.SaveChangesAsync();
                    return Ok(new { isFavorite = false, message = "Eliminado de favoritos" });
                }
                else
                {
                    // Agregar a favoritos
                    var favorite = new Favorite
                    {
                        UserId = userId,
                        SongId = songId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Favorites.Add(favorite);
                    await _context.SaveChangesAsync();
                    return Ok(new { isFavorite = true, message = "Agregado a favoritos" });
                }
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                Console.WriteLine($"Error en ToggleFavorite: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Favorites/Check/5
        [HttpGet("Check/{songId}")]
        public async Task<IActionResult> CheckFavorite(int songId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Ok(new { isFavorite = false });
                }

                var isFavorite = await _context.Favorites
                    .AnyAsync(f => f.UserId == userId && f.SongId == songId);

                return Ok(new { isFavorite });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CheckFavorite: {ex.Message}");
                return Ok(new { isFavorite = false });
            }
        }
    }
}