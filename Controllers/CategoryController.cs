using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestFilms.Models;

namespace TestFilms.Controllers
{
    [Route("[Controller]/[Action]")]
    [Route("[Controller]/[Action]/{id?}")]
    public class CategoryController : Controller
    {
        private readonly Context _db;
        public CategoryController(Context db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var query = _db.Categories.Include(x => x.Films);
            await query.ForEachAsync(x =>
                x.Nesting = _db.Database.SqlQuery<int>($"EXEC [dbo].[GetCategoriesNesting] @Id = {x.Id}")
                .AsEnumerable()
                .FirstOrDefault()
            );
            return View(query.ToList());
        }
        public async Task<IActionResult> Details(int? id)
        {
            var category = await _db.Categories.FindAsync(id);

            List<Category> categories = _db.Categories.ToList();
            ViewData["categories"] = categories;

            return View(category);
        }
        public IActionResult ManageCategories()
        {
            return View();
        }
        public async Task<IActionResult> Update(int? id)
        {
            Category? category = await _db.Categories.FindAsync(id);
            List<Category> categories = _db.Categories.ToList();
            ViewData["categories"] = categories;

            return View("Update", category);
        }

        private async Task<bool> IsLoop(int id, int parentId)
        {
            if (id == parentId)
            {
                return true;
            }
            parentId = await GetParentId(parentId);
            if (parentId == -1)
            {
                return false;
            }
            return await IsLoop(id, parentId);
        }
        private async Task<int> GetNesting(int id)
        {
            int parentId = await GetParentId(id);
            if (parentId != -1)
            {
                return await GetNesting(parentId) + 1;
            }
            return 0;
        }
        private async Task<int> GetParentId(int id)
        {
            Category? category = await _db.Categories.FindAsync(id);
            return category != null && category.ParentCategoryId != null ? category.ParentCategoryId.Value : -1;
        }
    }
}
