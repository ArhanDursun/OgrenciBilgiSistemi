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
        public async Task<IActionResult> Index()
        {
            var studentId = CurrentStudentId;

            // Öğrencinin kayıt olduğu ders Id’leri
            var enrolledIds = await _db.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.CourseId)
                .ToListAsync();

            // Tüm dersler + öğretmen bilgisi
            var courses = await _db.Courses
                .Include(c => c.Teacher)
                .ToListAsync();

            // Available (kayıt olunabilecek) dersler
            var available = courses
                .Where(c => !enrolledIds.Contains(c.Id))
                .Select(c => new StudentCourseViewModel
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    TeacherName = $"{c.Teacher.FirstName} {c.Teacher.LastName}",
                    IsEnrolled = false
                })
                .ToList();

            // Enrolled (kayıtlı) dersler
            var enrolled = courses
                .Where(c => enrolledIds.Contains(c.Id))
                .Select(c => new StudentCourseViewModel
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    TeacherName = $"{c.Teacher.FirstName} {c.Teacher.LastName}",
                    IsEnrolled = true
                })
                .ToList();

            // Dashboard ViewModel
            var vm = new StudentDashboardViewModel
            {
                AvailableCourses = available,
                EnrolledCourses = enrolled
            };

            return View(vm);
        }

        // POST: /Student/Enroll
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var studentId = CurrentStudentId;

            // Zaten kayıtlı mı?
            var exists = await _db.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (!exists)
            {
                _db.Enrollments.Add(new Enrollment
                {
                    CourseId = courseId,
                    StudentId = studentId
                });
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Student/Unenroll
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            var studentId = CurrentStudentId;

            var enrollment = await _db.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (enrollment != null)
            {
                _db.Enrollments.Remove(enrollment);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
