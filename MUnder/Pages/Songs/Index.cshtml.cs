using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MUnder.Data;
using MUnder.Models;
using System.Security.Claims;

namespace MUnder.Pages.Songs
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Song> Songs { get; set; } = new List<Song>();
        public IList<Playlist> UserPlaylists { get; set; } = new List<Playlist>();

        // Para búsqueda
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        public Dictionary<int, double> SongRatings { get; set; } = new Dictionary<int, double>();
        public Dictionary<int, int> ReviewCounts { get; set; } = new Dictionary<int, int>();

        public async Task OnGetAsync()
        {
            var query = _context.Songs.AsQueryable();

            // Búsqueda
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                query = query.Where(s =>
                    s.Title.ToLower().Contains(term) ||
                    s.Artist.ToLower().Contains(term));
            }

            Songs = await query
                .Include(s => s.Album)
                .ToListAsync();

            // Cargar playlists del usuario (para el selector)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                UserPlaylists = await _context.Playlists
                    .Where(p => p.OwnerId == userId)
                    .ToListAsync();
            }
            // Calcular promedios de ratings para cada canción
            var songIds = Songs.Select(s => s.Id).ToList();

            var reviewData = await _context.Reviews
                .Where(r => songIds.Contains(r.SongId))
                .GroupBy(r => r.SongId)
                .Select(g => new
                {
                    SongId = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    ReviewCount = g.Count()
                })
                .ToListAsync();

            SongRatings = reviewData.ToDictionary(x => x.SongId, x => x.AverageRating);
            ReviewCounts = reviewData.ToDictionary(x => x.SongId, x => x.ReviewCount);
        }

        public async Task<IActionResult> OnPostAddFavoriteAsync(int songId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "Usuario no autenticado" })
                {
                    StatusCode = 401
                };
            }

            // Verificar si la canción existe
            var songExists = await _context.Songs.AnyAsync(s => s.Id == songId);
            if (!songExists)
            {
                return new JsonResult(new { success = false, message = "Canción no encontrada" })
                {
                    StatusCode = 404
                };
            }

            // Verificar si ya existe en favoritos
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);

            if (existingFavorite == null)
            {
                _context.Favorites.Add(new Favorite { UserId = userId, SongId = songId });
                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Añadido a favoritos" });
            }

            return new JsonResult(new { success = true, message = "Ya estaba en favoritos" });
        }

        public async Task<IActionResult> OnPostAddToPlaylistAsync(int songId, int playlistId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return RedirectToPage("/Login");

            // Verificar que la playlist pertenece al usuario
            var playlistExists = await _context.Playlists
                .AnyAsync(p => p.Id == playlistId && p.OwnerId == userId);

            if (!playlistExists) return Forbid();

            if (!await _context.PlaylistSongs.AnyAsync(ps => ps.SongId == songId && ps.PlaylistId == playlistId))
            {
                _context.PlaylistSongs.Add(new PlaylistSong { SongId = songId, PlaylistId = playlistId });
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { SearchTerm });
        }
    }
}