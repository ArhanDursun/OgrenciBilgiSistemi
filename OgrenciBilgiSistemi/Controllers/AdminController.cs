using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OgrenciBilgiSistemi.Data;
using OgrenciBilgiSistemi.Models;
using OgrenciBilgiSistemi.Helpers;
using System.IO;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db) => _db = db;

        // -------------------------
        // 1. KULLANICI CRUD
        // -------------------------

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _db.Users.ToListAsync();
            return View(users);
        }

        // GET: /Admin/CreateUser
        public IActionResult CreateUser()
        {
            ViewBag.Roles = new SelectList(new[] { "Admin", "Teacher", "Student" });
            return View();
        }

        // POST: /Admin/CreateUser
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                // Hangi alanlarda hata var, listeleyip ViewBag’e atalım
                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Count > 0)
                    .Select(kv => new { Field = kv.Key, Errors = kv.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();
                ViewBag.ModelErrors = errors;
                return View(user);
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(new[] { "Admin", "Teacher", "Student" }, user.Role);
                return View(user);
            }

            // Parolayı hash’le
            user.PasswordHash = PasswordHelper.Hash(user.PasswordHash);

            // Fotoğrafı byte[]’e çevir
            using var ms = new MemoryStream();
            await user.PhotoFile.CopyToAsync(ms);
            user.Photo = ms.ToArray();
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

        // GET: /Admin/EditUser/5
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            ViewBag.Roles = new SelectList(new[] { "Admin", "Teacher", "Student" }, user.Role);
            return View(user);
        }

        // POST: /Admin/EditUser/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, User form)
        {
            if (id != form.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(new[] { "Admin", "Teacher", "Student" }, form.Role);
                return View(form);
            }

            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FirstName = form.FirstName;
            user.LastName = form.LastName;
            user.Email = form.Email;
            user.Role = form.Role;

            // Parola güncelleme (isteğe bağlı)
             if (!string.IsNullOrEmpty(form.PasswordHash))
                user.PasswordHash = PasswordHelper.Hash(form.PasswordHash);

            // Yeni fotoğraf yüklendiyse
            if (form.PhotoFile != null && form.PhotoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await form.PhotoFile.CopyToAsync(ms);
                user.Photo = ms.ToArray();
            }

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

        // POST: /Admin/DeleteUser/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

        // -------------------------
        // 2. DERS CRUD & ÖĞRETMEN ATAMA
        // -------------------------

        // GET: /Admin/Courses
        public async Task<IActionResult> Courses()
        {
            var courses = await _db.Courses
                                   .Include(c => c.Teacher)
                                   .ToListAsync();
            return View(courses);
        }

        // GET: /Admin/CreateCourse
        public IActionResult CreateCourse()
        {
            // Sadece Teacher rolündeki kullanıcıları dropdown’a alıyoruz
            var teachers = _db.Users
                              .Where(u => u.Role == "Teacher")
                              .ToList();
            ViewBag.TeacherList = new SelectList(teachers, "Id", "Email");
            return View();
        }

        // POST: /Admin/CreateCourse
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                var teachers = _db.Users.Where(u => u.Role == "Teacher").ToList();
                ViewBag.TeacherList = new SelectList(teachers, "Id", "Email", course.TeacherId);
                return View(course);
            }
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Courses));
        }

        // GET: /Admin/EditCourse/5
        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null) return NotFound();
            var teachers = _db.Users.Where(u => u.Role == "Teacher").ToList();
            ViewBag.TeacherList = new SelectList(teachers, "Id", "Email", course.TeacherId);
            return View(course);
        }

        // POST: /Admin/EditCourse/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(int id, Course form)
        {
            if (id != form.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                var teachers = _db.Users.Where(u => u.Role == "Teacher").ToList();
                ViewBag.TeacherList = new SelectList(teachers, "Id", "Email", form.TeacherId);
                return View(form);
            }

            _db.Courses.Update(form);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Courses));
        }

        // POST: /Admin/DeleteCourse/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null) return NotFound();
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Courses));
        }
    }
}
