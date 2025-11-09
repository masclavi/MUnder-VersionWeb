namespace MUnder.Models;
using System;
using System.ComponentModel.DataAnnotations;

    public class Review
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio")]
        [StringLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relaciones
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int SongId { get; set; }
        public Song Song { get; set; } = null!;
    }