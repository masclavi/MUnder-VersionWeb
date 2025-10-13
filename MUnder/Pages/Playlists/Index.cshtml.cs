using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MUnder.Data;
using MUnder.Models;
using System.Security.Claims;

namespace MUnder.Pages.Playlists
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Playlist> Playlists { get; set; } = new List<Playlist>();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                Playlists = await _context.Playlists
                    .Include(p => p.PlaylistSongs) // Carga las canciones para contarlas
                    .Where(p => p.OwnerId == userId)
                    .ToListAsync();
            }
        }
    }
}