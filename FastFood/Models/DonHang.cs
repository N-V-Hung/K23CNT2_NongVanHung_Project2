using System;
using System.Collections.Generic;

namespace FastFood.Models;

public partial class DonHang
{
    public int MaDh { get; set; }

    public DateTime? NgayDat { get; set; }

    public string? TrangThai { get; set; }

    public string? PhuongThucTt { get; set; }

    public decimal TongTien { get; set; }

    public int? MaKh { get; set; }

    
    

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual KhachHang? MaKhNavigation { get; set; }
}
