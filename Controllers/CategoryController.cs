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
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddCategory()
        {
            List<Category> categories = _db.Categories.ToList();
            ViewData["categories"] = categories;
            return View("AddCategory");
        }
        public async Task<IActionResult> Update(int? id)
        {
            Category? category = await _db.Categories.FindAsync(id);
            List<Category> categories = _db.Categories.ToList();
            ViewData["categories"] = categories;

            return View("Update", category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            
            return AddCategory();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory([FromQuery] int id)
        {
            Category? category = await _db.Categories.FindAsync(id);
            return Ok(category);
        }
        [HttpGet]
        public IActionResult GetCategories()
        {
            List<Category> categories = _db.Categories.Include(x => x.Films).ToList();
            categories.ForEach(async x => x.Nesting = await GetNesting(x.Id));
            return Ok(categories);
        }
        [HttpGet]
        public IActionResult GetCategoriesByFilmId(int filmId)
        {
            List<Category> categories = _db.Categories.Where(x => x.Films != null && x.Films.Any(y => y.Id == filmId)).ToList();

            return Ok(categories);
        }
        [HttpPatch, HttpPost]
        public async Task<IActionResult> UpdateCategory(int id, Category categoryModel)
        {
            if (!await IsLoop(id, categoryModel.ParentCategoryId ?? -1))
            {
                Category? category = await _db.Categories.FindAsync(id);
                if (category != null)
                {
                    category.ParentCategoryId = categoryModel.ParentCategoryId;
                    category.Name = categoryModel.Name;
                    _db.Categories.Update(category);
                    await _db.SaveChangesAsync();
                }
            }
            return await Update(id);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category? category = await _db.Categories.FindAsync(id);

            if (category != null)
            {
                var childCategories = _db.Categories.Where(x => x.ParentCategoryId == category.Id);
                await childCategories.ForEachAsync(x => x.ParentCategoryId = null);
                _db.Categories.UpdateRange(childCategories);
                await _db.SaveChangesAsync();

                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
            }
            return Index();
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
