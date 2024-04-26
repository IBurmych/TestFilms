using Microsoft.AspNetCore.Mvc;
using TestFilms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace TestFilms.Controllers
{
    [Route("[Controller]/[Action]")]
    [Route("[Controller]/[Action]/{id?}")]
    public class FilmsController : Controller
    {
        private readonly Context _db;
        public FilmsController(Context db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Film> films = _db.Films.Include(x => x.Categories).ToList();
            return View("Index", films);
        }
        public IActionResult ManageFilms()
        {
            return View();
        }
        public IActionResult AddFilm()
        {
            List<Category> categories = _db.Categories.ToList();
            ViewData["categories"] = categories;

            return View("AddFilm");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Film? film = await _db.Films.FindAsync(id);
            if (film != null)
            {
                film.CategoriesIds = await _db.Categories
                    .Where(c => c.Films != null && c.Films.Any(f => f.Id == film.Id))
                    .Select(c => c.Id)
                    .ToArrayAsync();
            }

            List<Category> categories = _db.Categories.ToList();
            ViewData["categories"] = categories;

            return View("Update", film);
        }

    }
}
