using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MUnder.Models
{
    public class Album
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }

        public string CoverPath { get; set; }

        public ICollection<Song>? Songs { get; set; }
    }
}
