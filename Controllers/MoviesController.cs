using BoxOffice.Data;
using BoxOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoxOffice.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly MovieContext _context;

        public MoviesController(MovieContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            webHostEnvironment = webHost;
        }
        public IActionResult Index()
        {
            List<Movie> images = _context.Movies.ToList();
            return View(images);
        }
        [HttpGet]
        public IActionResult Create()
        {
            Movie movie = new Movie();
            return View(movie);
        }
        [HttpPost]
        public IActionResult Create(Movie movie)
        {
            string uniqueFileName = UploadedFile(movie);
            movie.MovieImageURL = uniqueFileName;
            _context.Attach(movie);
            _context.Entry(movie).State = EntityState.Added;
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
        private string UploadedFile(Movie movie)
        {
            string? uniqueFileName = null;
            if (movie.MovieImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + movie.MovieImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    movie.MovieImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;

        }
        public IActionResult Edit(int? id) 
        {
            if(id == null)
            {
                return NotFound();
            }
            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        [HttpPost]
        public IActionResult Edit(int id,Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }
            if(movie.MovieImage==null || movie.MovieImage.Length==0)
            {
                string existingImagePath = FetchExistingImagePathFromDatabase(movie.MovieId);
            }
            try
            {
                string uniqueFileName = UploadedFile(movie);
                movie.MovieImageURL = uniqueFileName;
                _context.Attach(movie);
                _context.Entry(movie).State = EntityState.Added;
                _context.Update(movie);
                _context.SaveChanges();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(movie.MovieId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return RedirectToAction("Index");
        }
        private string FetchExistingImagePathFromDatabase(int id)
        {
            var item = _context.Movies.FirstOrDefault(i => i.MovieId == id);
            if(item != null)
            {
                return item.MovieImageURL;
            }
            return string.Empty;
        }
        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
        public IActionResult Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var movie = _context.Movies.
                FirstOrDefault(e => e.MovieId == id);
            if(movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie =  _context.Movies.Find(id);
            if(movie != null)
            {
                _context.Movies.Remove(movie);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Users()
        {
            List<Movie> movies = _context.Movies.ToList();
            return View(movies);
        }
        [ActionName("Details")]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if(movie == null)
            {
                return NotFound();
            }
            ViewData["Title"] = movie.MovieName;
            return View(movie);
        }
    }
}

