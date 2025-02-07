using System.ComponentModel.DataAnnotations;

namespace MuseAndMasterpiece.Models
{
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Bio { get; set; }

        [Required]
        public string Email { get; set; }


        // one artist can have many artworks
        public ICollection<Artwork>? Artworks { get; set; }
    }

    public class ArtistDto
    {
        [Key]
        public int ArtistId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Bio { get; set; }

        public int TotalArtworks { get; set; }

        public List<string> ArtworksTitle { get; set; }

    }


    public class AddUpdArtistDto
    {
        [Key]
        public int ArtistId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Bio { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
