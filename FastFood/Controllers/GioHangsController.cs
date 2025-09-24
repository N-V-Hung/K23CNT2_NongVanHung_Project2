using FastFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Controllers
{
    public class GioHangsController : Controller
    {
        private readonly QlbanDoAnContext _context;

        public GioHangsController(QlbanDoAnContext context)
        {
            _context = context;
        }

        // ✅ Xem giỏ hàng
        public async Task<IActionResult> Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var gioHang = await _context.GioHangs
                .Include(g => g.MaSpNavigation)
                .Where(g => g.MaKh == customerId)
                .ToListAsync();

            return View(gioHang);
        }

        // ✅ Thêm sản phẩm vào giỏ
        public async Task<IActionResult> ThemVaoGio(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var sp = await _context.SanPhams.FindAsync(id);
            if (sp == null)
            {
                return NotFound();
            }

            var existingItem = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaKh == customerId && g.MaSp == id);

            if (existingItem != null)
            {
                existingItem.SoLuong += 1;
                _context.Update(existingItem);
            }
            else
            {
                var gioHang = new GioHang
                {
                    MaKh = customerId.Value,
                    MaSp = id,
                    SoLuong = 1
                };
                _context.Add(gioHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult CapNhatSoLuong(int id, string actionType)
        {
            var gioHang = _context.GioHangs.FirstOrDefault(g => g.MaGh == id);
            if (gioHang != null)
            {
                if (actionType == "increase")
                    gioHang.SoLuong += 1;
                else if (actionType == "decrease" && gioHang.SoLuong > 1)
                    gioHang.SoLuong -= 1;

                _context.SaveChanges();
            }

            return RedirectToAction("Index"); // quay lại trang giỏ hàng
        }

        // ✅ Xóa sản phẩm khỏi giỏ
        public async Task<IActionResult> XoaKhoiGio(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaGh == id && g.MaKh == customerId);

            if (gioHang != null)
            {
                _context.GioHangs.Remove(gioHang);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
