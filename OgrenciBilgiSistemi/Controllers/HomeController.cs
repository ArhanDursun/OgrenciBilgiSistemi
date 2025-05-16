using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OgrenciBilgiSistemi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Users", "Admin");
            if (User.IsInRole("Teacher"))
                return RedirectToAction("Index", "Teacher");
            if (User.IsInRole("Student"))
                return RedirectToAction("Index", "Student");

            // Hiçbir role eþleþmezse
            return View();
        }

        public IActionResult Privacy() => View();
    }
}