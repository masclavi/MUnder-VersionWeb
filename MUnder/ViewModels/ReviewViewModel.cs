using System.ComponentModel.DataAnnotations;

namespace MUnder.ViewModels
{
    public class ReviewViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una calificación")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5 estrellas")]
        [Display(Name = "Calificación")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "El comentario debe tener entre 10 y 500 caracteres")]
        [Display(Name = "Tu reseña")]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public int SongId { get; set; }

        public string? SongTitle { get; set; }
        public string? SongArtist { get; set; }
    }
}