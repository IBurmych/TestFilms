using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestFilms.Models;

namespace TestFilms.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FilmApiController : ControllerBase
    {
        private readonly Context _db;
        public FilmApiController(Context db)
        {
            _db = db;
        }
        [HttpPost]
        public async Task<IActionResult> Create(Film film)
        {
            try
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

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                List<Film> films = _db.Films.Include(x => x.Categories).ToList();
                return Ok(films);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPatch, HttpPost]
        public async Task<IActionResult> Update(Film filmModel)
        {
            try
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
                return Ok();
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
                Film? film = await _db.Films.FindAsync(id);
                if (film != null)
                {
                    _db.Films.Remove(film);
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
