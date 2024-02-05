using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace BoxOffice.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }
        [Display(Name ="Movie Name")]
        public string MovieName { get; set; }
        [Display(Name ="Release Year")]
        public int ReleaseYear { get; set; }
        [Display(Name ="Movie Rating")]
        public int Rating { get; set; }
        public string Cast { get; set; }
        [Display(Name = "Image")]
        public string MovieImageURL { get; set; } = "image.jpg";
        [NotMapped]
        
        public IFormFile MovieImage { get; set; } 
    }
}