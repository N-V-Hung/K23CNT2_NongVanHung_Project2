using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Admin" &&
                HttpContext.Session.GetString("Role") != "Quản lý")
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }



    }
}
