using Microsoft.AspNetCore.Http;
using ProjectGSMAUI.Api.Container;
using ProjectGSMVC.Services;
using ProjectGSMAUI.Api.Services;
using ProjectGSMVC.Areas.Admin.Controllers;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Thêm CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://localhost:7047") // URL của MVC frontend
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
builder.Services.AddSingleton(new Uri("https://localhost:7141/api"));
builder.Services.AddHttpClient<PhimController>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddDistributedMemoryCache(); // Thêm cache cho session
builder.Services.AddSingleton<IVnPayServices, VnPayServices>();
// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout của session
    options.Cookie.HttpOnly = true; // Chỉ cho phép truy cập cookie qua HTTP
    options.Cookie.IsEssential = true; // Đảm bảo cookie luôn được lưu
});
// Add services to the container.


builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
// Áp dụng CORS Policy
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

// Định tuyến
app.MapControllerRoute(
name: "areaRoute",
pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
   name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();