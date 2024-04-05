using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestFilms.Models
{
    public class Film
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string? Name { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string? Director { get; set; }
        public DateTime Release { get; set; }
        [NotMapped]
        public int[]? CategoriesIds { get; set; }
        public List<Category>? Categories { get; } = [];

    }
}
