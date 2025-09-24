using Microsoft.EntityFrameworkCore;
using FastFood.Models; // namespace chứa QLBanDoAnContext

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm DbContext (chỉnh lại chuỗi kết nối phù hợp trong appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("NvhDbConnect");
builder.Services.AddDbContext<QlbanDoAnContext>(x => x.UseSqlServer(connectionString));


// Bật Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session sống 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Để có thể inject HttpContext vào View (_Layout.cshtml)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Bật Session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
