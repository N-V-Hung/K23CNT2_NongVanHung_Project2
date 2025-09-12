using System;
using System.Collections.Generic;

namespace FastFood.Models;

public partial class NhanVien
{
    public int MaNv { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Sdt { get; set; }

    public string? VaiTro { get; set; }
}
