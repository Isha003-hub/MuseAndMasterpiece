using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MuseAndMasterpiece.Models
{
    public class Artwork
    {
        [Key]
        public int ArtWorkId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateOnly DatePosted { get; set; }

        // one artwork is related to one artist

        [ForeignKey("Artists")]
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; } //navigation property


        // one artwork belongs to one category
        [ForeignKey("Categories")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

    }


    public class ArtworkDto
    {
        [Key]
        public int ArtWorkId { get; set; }

        [Required]
        public string Title { get; set; }

        public DateOnly DatePosted { get; set; }

        public string ArtistName { get; set; }

        public string CategoryName { get; set; }


    }


    public class AddUpdArtworkDto
    {
        [Key]
        public int ArtWorkId { get; set; }

        [Required]
        public string Title { get; set; }

        public DateOnly DatePosted { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public int ArtistId { get; set; }
    }
}
