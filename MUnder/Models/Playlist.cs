using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MUnder.Models
{
    public class Playlist
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        // Owner
        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public bool IsPublic { get; set; } = true;

        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
    }
}
