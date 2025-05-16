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
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StudentController(ApplicationDbContext db) => _db = db;

        // Şu anki öğrencinin user Id’si
        private int CurrentStudentId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Student
        // Tüm dersleri listeler, kayıtlıysa butonu kapatır
        public async Task<IActionResult> Index()
        {
            // Öğrencinin kayıt olduğu courseIds
            var enrolledCourseIds = await _db.Enrollments
                .Where(e => e.StudentId == CurrentStudentId)
                .Select(e => e.CourseId)
                .ToListAsync();

            // Tüm dersler ve ilgili öğretmen bilgisi
            var courses = await _db.Courses
                .Include(c => c.Teacher)
                .ToListAsync();

            // View’a modeli: ders + kayıt durumu
            var vm = courses.Select(c => new StudentCourseViewModel
            {
                CourseId = c.Id,
                Title = c.Title,
                TeacherName = c.Teacher.FirstName + " " + c.Teacher.LastName,
                IsEnrolled = enrolledCourseIds.Contains(c.Id)
            }).ToList();

            return View(vm);
        }

        // POST: /Student/Enroll/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            // Zaten kayıtlı mı kontrol et
            var exists = await _db.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.StudentId == CurrentStudentId);
            if (!exists)
            {
                _db.Enrollments.Add(new Enrollment
                {
                    CourseId = courseId,
                    StudentId = CurrentStudentId
                });
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
