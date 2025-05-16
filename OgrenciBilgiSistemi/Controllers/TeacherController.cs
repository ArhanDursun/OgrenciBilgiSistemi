using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TeacherController(ApplicationDbContext db) => _db = db;

        // Yardımcı: şu anki kullanıcının Id’si
        private int CurrentTeacherId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Teacher
        public async Task<IActionResult> Index()
        {
            var courses = await _db.Courses
                                   .Where(c => c.TeacherId == CurrentTeacherId)
                                   .ToListAsync();
            return View(courses);
        }

        // GET: /Teacher/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Teacher/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (!ModelState.IsValid)
                return View(course);

            course.TeacherId = CurrentTeacherId;
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Teacher/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _db.Courses
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == CurrentTeacherId);
            if (course == null) return NotFound();

            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Teacher/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var course = await _db.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == CurrentTeacherId);

            if (course == null) return NotFound();
            return View(course);
        }
    }
}
