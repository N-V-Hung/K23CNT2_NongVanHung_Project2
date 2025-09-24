using FastFood.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Controllers
{
    public class DonHangsController : Controller
    {
        private readonly QlbanDoAnContext _context;

        public DonHangsController(QlbanDoAnContext context)
        {
            _context = context;
        }

        // 📌 Lấy role & customerId từ session
        private string? GetRole() => HttpContext.Session.GetString("Role");
        private int? GetCustomerId() => HttpContext.Session.GetInt32("CustomerId");

        // GET: DonHangs
        public async Task<IActionResult> Index()
        {
            var role = GetRole();
            var customerId = GetCustomerId();

            IQueryable<DonHang> query = _context.DonHangs
                .Include(d => d.MaKhNavigation);

            if (role == "Customer" && customerId != null)
            {
                query = query.Where(d => d.MaKh == customerId);
            }

            return View(await query.ToListAsync());
        }

        // GET: DonHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);

            if (donHang == null) return RedirectToAction(nameof(Index));

            // Khách không được xem đơn người khác
            var role = GetRole();
            var customerId = GetCustomerId();
            if (role == "Customer" && donHang.MaKh != customerId)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(donHang);
        }

        // GET: DonHangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (GetRole() != "Admin") return Unauthorized();
            if (id == null) return RedirectToAction(nameof(Index));

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null) return RedirectToAction(nameof(Index));

            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "HoTen", donHang.MaKh);
            return View(donHang);
        }

        // POST: DonHangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonHang donHang)
        {
            if (GetRole() != "Admin") return Unauthorized();
            if (id != donHang.MaDh) return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DonHangs.Any(e => e.MaDh == donHang.MaDh))
                        return RedirectToAction(nameof(Index));
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "HoTen", donHang.MaKh);
            return View(donHang);
        }

        // GET: DonHangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (GetRole() != "Admin") return Unauthorized();
            if (id == null) return RedirectToAction(nameof(Index));

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);

            if (donHang == null) return RedirectToAction(nameof(Index));

            return View(donHang);
        }

        // POST: DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (GetRole() != "Admin") return Unauthorized();

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null)
            {
                // Xóa chi tiết đơn hàng liên quan trước khi xóa đơn hàng
                var chiTietDonHangs = _context.ChiTietDonHangs.Where(ct => ct.MaDh == id);
                _context.ChiTietDonHangs.RemoveRange(chiTietDonHangs);

                // Xóa đơn hàng
                _context.DonHangs.Remove(donHang);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
