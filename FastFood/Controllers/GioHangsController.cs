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

        // ✅ Cập nhật số lượng (POST)
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

            return RedirectToAction("Index");
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

        // ✅ Hiển thị form chọn phương thức thanh toán (GET)
        [HttpGet]
        public IActionResult ThanhToan()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Auth");

            return View();
        }

        // ✅ Xử lý thanh toán (POST)
        [HttpPost]
        public async Task<IActionResult> ThanhToan(string phuongThucThanhToan)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var gioHangItems = await _context.GioHangs
                .Include(g => g.MaSpNavigation)
                .Where(g => g.MaKh == customerId)
                .ToListAsync();

            if (gioHangItems == null || !gioHangItems.Any())
            {
                TempData["Message"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            // Danh sách phương thức thanh toán hợp lệ (phải đúng với CHECK constraint trong database)
            var validPaymentMethods = new List<string> { "GiaDienTu", "TienMat", "TheTinDung" };

            if (string.IsNullOrEmpty(phuongThucThanhToan) || !validPaymentMethods.Contains(phuongThucThanhToan))
            {
                ModelState.AddModelError("", "Phương thức thanh toán không hợp lệ. Vui lòng chọn phương thức hợp lệ.");
                return View();
            }

            // Tính tổng tiền
            decimal tongTien = gioHangItems.Sum(item =>
                (item.SoLuong ?? 0) * item.MaSpNavigation.Gia);

            // Tạo đơn hàng
            var donHang = new DonHang
            {
                MaKh = customerId.Value,
                NgayDat = DateTime.Now,
                TrangThai = "Chờ xác nhận",
                TongTien = tongTien,
                PhuongThucTt = phuongThucThanhToan
            };

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync(); // để lấy mã đơn hàng

            // Tạo chi tiết đơn hàng
            foreach (var item in gioHangItems)
            {
                var ctdh = new ChiTietDonHang
                {
                    MaDh = donHang.MaDh,
                    MaSp = item.MaSp.Value,
                    SoLuong = item.SoLuong,
                    DonGia = item.MaSpNavigation.Gia
                };
                _context.ChiTietDonHangs.Add(ctdh);
            }

            // Xóa giỏ hàng sau khi thanh toán
            _context.GioHangs.RemoveRange(gioHangItems);

            await _context.SaveChangesAsync();

            TempData["Message"] = "Đặt hàng thành công!";
            TempData["TongTien"] = tongTien.ToString("F2");
            TempData["PhuongThucThanhToan"] = phuongThucThanhToan;

            return RedirectToAction("XacNhanDonHang");
        }

        // ✅ Xác nhận đơn hàng
        public IActionResult XacNhanDonHang()
        {
            ViewBag.Message = TempData["Message"] as string;

            var tongTienStr = TempData["TongTien"] as string;
            if (decimal.TryParse(tongTienStr, out var tongTien))
            {
                ViewBag.TongTien = tongTien;
            }
            else
            {
                ViewBag.TongTien = null;
            }

            ViewBag.PhuongThucThanhToan = TempData["PhuongThucThanhToan"] as string;

            return View();
        }
    }
}
