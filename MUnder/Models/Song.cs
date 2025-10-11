using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MUnder.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string FilePath { get; set; }      // ruta en wwwroot/audio/...
        public string CoverPath { get; set; }     // portada en wwwroot/images/...

        public TimeSpan? Duration { get; set; }

        // Relaciones opcionales
        public int? AlbumId { get; set; }
        public Album Album { get; set; }

        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}
