using System;
using System.Collections.Generic;

namespace FastFood.Models;

public partial class KhachHang
{
    public int MaKh { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Sdt { get; set; }

    public string MatKhau { get; set; } = null!;

    public string? DiaChiMacDinh { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();
}
