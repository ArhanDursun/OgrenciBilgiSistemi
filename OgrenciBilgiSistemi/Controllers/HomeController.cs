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

            // Hi�bir role e�le�mezse
            return View();
        }

        public IActionResult Privacy() => View();
    }
}