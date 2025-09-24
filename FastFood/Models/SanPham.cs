using System;
using System.Collections.Generic;

namespace FastFood.Models;

public partial class SanPham
{
    public int MaSp { get; set; }

    public string TenSp { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal Gia { get; set; }

    public string? Anh { get; set; }

    public int? MaDm { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual DanhMuc? MaDmNavigation { get; set; }
}
