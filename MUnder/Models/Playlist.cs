using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MUnder.Models
{
    public class Playlist
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string OwnerId { get; set; } = string.Empty;

        public ApplicationUser Owner { get; set; } = null!;

        public bool IsPublic { get; set; } = true;

        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
    }
}