using System;
using System.Collections.Generic;

namespace FastFood.Models;

public partial class ChiTietDonHang
{
    public int MaDh { get; set; }

    public int MaSp { get; set; }

    public int? SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual DonHang MaDhNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
