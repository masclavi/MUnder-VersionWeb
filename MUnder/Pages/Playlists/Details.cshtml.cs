using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MUnder.Data;
using MUnder.Models;
using System.Security.Claims;

namespace MUnder.Pages.Playlists
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Playlist Playlist { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Playlist == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveSongAsync(int songId, int playlistId)
        {
            var playlistSong = await _context.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.SongId == songId && ps.PlaylistId == playlistId);

            if (playlistSong == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.Id == playlistId);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar que el usuario sea el propietario de la playlist
            if (playlist?.OwnerId != userId)
            {
                return Forbid();
            }

            try
            {
                _context.PlaylistSongs.Remove(playlistSong);
                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Canción eliminada de la playlist. SongId: {songId}, PlaylistId: {playlistId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar canción: {ex.Message}");
                TempData["ErrorMessage"] = "No se pudo eliminar la canción.";
            }

            return RedirectToPage(new { id = playlistId });
        }

        public async Task<IActionResult> OnPostDeletePlaylistAsync(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar que el usuario sea el propietario
            if (playlist.OwnerId != userId)
            {
                return Forbid();
            }

            try
            {
                // Eliminar todas las canciones asociadas primero
                _context.PlaylistSongs.RemoveRange(playlist.PlaylistSongs);

                // Eliminar la playlist
                _context.Playlists.Remove(playlist);

                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Playlist eliminada. PlaylistId: {id}");

                TempData["SuccessMessage"] = "Playlist eliminada correctamente.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar playlist: {ex.Message}");
                TempData["ErrorMessage"] = "No se pudo eliminar la playlist.";
            }

            return RedirectToPage("Index");
        }
    }
}