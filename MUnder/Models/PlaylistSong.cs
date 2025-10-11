namespace MUnder.Models
{
    public class PlaylistSong
    {
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }

        public int Order { get; set; } // posición en la playlist
    }
}
