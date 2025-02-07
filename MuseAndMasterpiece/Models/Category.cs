using System.ComponentModel.DataAnnotations;

namespace MuseAndMasterpiece.Models
{
    public class Category
    {

        [Key]
        public int CategoryId { get; set; }

        public string CName { get; set; }

        public DateOnly DateCreated { get; set; }

        // one category can have many artworks

        public ICollection<Artwork> Artworks { get; set; }

    }

    public class CategoryDto
    {
        [Key]
        public int CategoryId { get; set; }

        public string CName { get; set; }

        public DateOnly DateCreated { get; set; }

        public int TotalArtworks { get; set; }

        public List<string> ArtworksTitle { get; set; }

    }


    public class   UpdCategoryDto
    {
        [Key]
        public int CategoryId { get; set; }

        public string CName { get; set; }

        public DateOnly DateCreated { get; set; }
    }


    public class AddCategoryDto
    {
        public string CName { get; set; }

        public DateOnly DateCreated { get; set; }
    }
}
