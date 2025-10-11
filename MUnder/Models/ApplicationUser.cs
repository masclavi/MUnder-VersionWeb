using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MUnder.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Campos extra que quieras
        public string DisplayName { get; set; }

        // Relaciones
        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}
