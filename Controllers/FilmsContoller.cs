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
            return View("Index");
        }
        public IActionResult AddFilm()
        {
            List<Category> categories = _db.Categories.ToList();
            ViewData["categories"] = categories;

            return View("AddFilm");
        }
        public async Task<IActionResult> Update(int? id)
        {
            Film? film = await _db.Films.FindAsync(id);
            List<Category> categories = _db.Categories.ToList();
            ViewData["categories"] = categories;

            return View("Update", film);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFilm(Film film)
        {
            await _db.Films.AddAsync(film);
            await _db.SaveChangesAsync();

            if (film.CategoriesIds != null)
            {
                foreach (int categoryId in film.CategoriesIds)
                {
                    await _db.FilmCategory.AddAsync(new FilmCategory { CategoryId = categoryId, FilmId = film.Id });
                }
            }
            await _db.SaveChangesAsync();

            return AddFilm();
        }
        [HttpGet]
        public async Task<IActionResult> GetFilm([FromQuery]int id)
        {
            Film? film = await _db.Films.FindAsync(id);
            return Ok(film);
        }
        [HttpGet]
        public IActionResult GetFilms()
        {
            List<Film> films = _db.Films.Include(x => x.Categories).ToList();
            return Ok(films);
        }
        [HttpGet]
        public IActionResult GetFilmsByCategoryId(int categoryId)
        {
            List<Film> films = _db.Films.Where(x => x.Categories != null && x.Categories.Any(y => y.Id == categoryId)).ToList();

            return Ok(films);
        }
        [HttpPatch, HttpPost]
        public async Task<IActionResult> UpdateFilm(int? id, Film filmModel)
        {
            Film? film = await _db.Films.FindAsync(filmModel.Id);
            if (film != null)
            {
                film.Director = filmModel.Director;
                film.Name = filmModel.Name;
                film.Release = filmModel.Release;

                _db.Films.Update(film);
                await _db.SaveChangesAsync();


                if (film != null)
                {
                    var filmCategories = _db.FilmCategory.Where(x => x.FilmId == film.Id);
                    _db.FilmCategory.RemoveRange(filmCategories);
                    await _db.SaveChangesAsync();
                }
                if (filmModel.CategoriesIds != null)
                {
                    foreach (int categoryId in filmModel.CategoriesIds)
                    {
                        await _db.FilmCategory.AddAsync(new FilmCategory { CategoryId = categoryId, FilmId = filmModel.Id });
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return await Update(id);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteFilm(int id)
        {
            Film? film = await _db.Films.FindAsync(id);
            if (film != null)
            {
                _db.Films.Remove(film);
                await _db.SaveChangesAsync();
            }
            return Index();
        }
    }
}
