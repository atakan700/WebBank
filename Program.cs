using Microsoft.EntityFrameworkCore;
using WebBank.Models;

var builder = WebApplication.CreateBuilder(args);

// HTTPS yönlendirme işlemi
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 7106;
});

// Veritabanı bağlantısı
builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC servisleri
builder.Services.AddControllersWithViews();

// ✅ Session servisi
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Orta katmanlar (middleware)

// Statik dosyalar önce gelsin
app.UseStaticFiles();

// HTTPS yönlendirme
app.UseHttpsRedirection();

// Routing önce gelsin
app.UseRouting();

// Session middleware'i, UseRouting sonrası olmalı
app.UseSession();

// Authorization middleware'i UseSession ve UseRouting sonrası olmalı
app.UseAuthorization();

// Varsayılan route ayarı
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calisanlar}/{action=GiseGiris}/{id?}");

app.Run();
