using FastFood.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Controllers
{
    public class AuthController : Controller
    {
        private readonly QlbanDoAnContext _context;

        public AuthController(QlbanDoAnContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // ✅ kiểm tra admin trước
            var admin = _context.NhanViens
                .FirstOrDefault(u => u.Email == email && u.MatKhau == password);

            if (admin != null)
            {
                HttpContext.Session.SetString("UserName", admin.HoTen);
                HttpContext.Session.SetString("Role", admin.VaiTro);
                HttpContext.Session.SetInt32("AdminId", admin.MaNv);

                // ✅ chuyển hẳn về Dashboard
                return RedirectToAction("Dashboard", "Admin");
            }

            // ✅ nếu không phải admin thì kiểm tra khách hàng
            var customer = _context.KhachHangs
                .FirstOrDefault(u => u.Email == email && u.MatKhau == password);

            if (customer != null)
            {
                HttpContext.Session.SetString("UserName", customer.HoTen);
                HttpContext.Session.SetString("Role", "Customer");
                HttpContext.Session.SetInt32("CustomerId", customer.MaKh);

                return RedirectToAction("Index", "Home");
            }

            // ❌ nếu sai
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        // GET: /Auth/RegisterCustomer
        [HttpGet]
        public IActionResult RegisterCustomer()
        {
            return View();
        }

        // POST: /Auth/RegisterCustomer
        [HttpPost]
        public IActionResult RegisterCustomer(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                if (_context.KhachHangs.Any(x => x.Email == model.Email))
                {
                    ViewBag.Error = "Email đã tồn tại!";
                    return View(model);
                }

                _context.KhachHangs.Add(model);
                _context.SaveChanges();

                ViewBag.Success = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
