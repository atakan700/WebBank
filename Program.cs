using Microsoft.EntityFrameworkCore;
using WebBank.Models;

var builder = WebApplication.CreateBuilder(args);

// HTTPS y�nlendirme i�lemi
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 7106;  // HTTPS y�nlendirme i�in portu belirleyin
});

builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS y�nlendirmesini aktif et
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calisanlar}/{action=Ekle}/{id?}");

app.Run();
