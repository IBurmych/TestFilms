using System.ComponentModel.DataAnnotations.Schema;

namespace TestFilms.Models
{
    [Table("Film-categories")]
    public class FilmCategory
    {
        public int Id { get; set; }

        [Column("Film_id")]
        public int FilmId { get; set; }
        [Column("Category_id")]
        public int CategoryId { get; set; }
    }
}
