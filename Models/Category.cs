using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestFilms.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string? Name { get; set; }

        public List<Film>? Films { get; } = [];

        [Column("Parent_category_id")]
        [ForeignKey("ParentBucketGroup")]
        public int? ParentCategoryId { get; set; }
        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category>? Children { get; set; }

        [NotMapped]
        public int? Nesting { set; get; }

        public static SelectList GetCategoriesOptions(List<Category> categories)
        {
            return new SelectList(categories, nameof(Category.Id), nameof(Category.Name));
        }
    }
}
