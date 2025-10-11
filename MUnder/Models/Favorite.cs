namespace MUnder.Models
{
    public class Favorite
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
