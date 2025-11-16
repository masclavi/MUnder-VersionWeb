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
        }

        public async Task<IActionResult> OnPostAddFavoriteAsync(int songId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return RedirectToPage("/Login");

            if (!await _context.Favorites.AnyAsync(f => f.UserId == userId && f.SongId == songId))
            {
                _context.Favorites.Add(new Favorite { UserId = userId, SongId = songId });
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { SearchTerm });
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