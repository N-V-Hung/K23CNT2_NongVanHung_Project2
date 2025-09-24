using FastFood.Models;
using Microsoft.AspNetCore.Mvc;
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

        // GET: DonHangs
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            IQueryable<DonHang> query = _context.DonHangs
                .Include(d => d.MaKhNavigation);

            // Nếu là khách thì chỉ xem đơn hàng của mình
            if (role == "Customer" && customerId != null)
            {
                query = query.Where(d => d.MaKh == customerId);
            }

            return View(await query.ToListAsync());
        }

        // GET: DonHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);

            if (donHang == null) return NotFound();

            // Nếu khách -> chặn xem đơn của người khác
            var role = HttpContext.Session.GetString("Role");
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (role == "Customer" && donHang.MaKh != customerId)
            {
                return Unauthorized(); // cấm truy cập
            }

            return View(donHang);
        }

        // GET: DonHangs/Edit/5 (chỉ Admin mới được sửa)
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Unauthorized();
            }

            if (id == null) return NotFound();

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null) return NotFound();

            return View(donHang);
        }

        // POST: DonHangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DonHang donHang)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Unauthorized();
            }

            if (id != donHang.MaDh) return NotFound();

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
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(donHang);
        }

        // GET: DonHangs/Delete/5 (Admin mới được xóa)
        public async Task<IActionResult> Delete(int? id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Unauthorized();
            }

            if (id == null) return NotFound();

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

        // POST: DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return Unauthorized();
            }

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null)
            {
                _context.DonHangs.Remove(donHang);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
