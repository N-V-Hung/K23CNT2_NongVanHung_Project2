using System;
using System.Collections.Generic;

namespace FastFood.Models;

public partial class GioHang
{
    public int MaGh { get; set; }

    public int? MaKh { get; set; }

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual KhachHang? MaKhNavigation { get; set; }
}
