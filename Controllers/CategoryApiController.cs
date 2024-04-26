using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestFilms.Models;

namespace TestFilms.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CategoryApiController : ControllerBase
    {
        private readonly Context _db;
        public CategoryApiController(Context db)
        {
            _db = db;
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                await _db.Categories.AddAsync(category);
                await _db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = _db.Categories.Include(x => x.Films);
                await query.ForEachAsync(x =>
                    x.Nesting = _db.Database
                    .SqlQuery<int>($"EXEC [dbo].[GetCategoriesNesting] @Id = {x.Id}")
                    .AsEnumerable()
                    .FirstOrDefault()
                );

                List<Category> categories = query.ToList();

                return Ok(categories);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPatch, HttpPost]
        public async Task<IActionResult> Update(Category categoryModel)
        {
            try
            {
                if (categoryModel.Id == categoryModel.ParentCategoryId)
                {
                    return BadRequest();
                }
                int? isLoop = _db.Database
                    .SqlQuery<int?>($"EXEC [dbo].[IsCategoriesLoop] @id = {categoryModel.Id}, @parentId = {categoryModel.ParentCategoryId ?? -1}")
                    .AsEnumerable()
                    .FirstOrDefault();
                if (isLoop != 1)
                {
                    Category? category = await _db.Categories.FindAsync(categoryModel.Id);
                    if (category != null)
                    {
                        category.ParentCategoryId = categoryModel.ParentCategoryId;
                        category.Name = categoryModel.Name;
                        _db.Categories.Update(category);
                        await _db.SaveChangesAsync();
                    }
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
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
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
