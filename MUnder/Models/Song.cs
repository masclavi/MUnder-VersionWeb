using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MUnder.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Artist { get; set; } = string.Empty; 

        [Required]
        public string YouTubeUrl { get; set; } = string.Empty;  

        public string CoverPath { get; set; } = "/Imagenes/default-cover.png"; 

        public TimeSpan? Duration { get; set; }

        // Relaciones
        public int? AlbumId { get; set; }
        public Album? Album { get; set; } = null!;

        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}