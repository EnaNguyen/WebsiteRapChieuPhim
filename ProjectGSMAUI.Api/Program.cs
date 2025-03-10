﻿using AutoMapper;
using ProjectGSMAUI.Api.Helper;
using Microsoft.EntityFrameworkCore;
using ProjectGSMAUI.Api.Data; // Changed from ProjectGSMAUI.Api.Container
using ProjectGSMAUI.Api.Container;
using ProjectGSMAUI.Api.Services;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using ProjectGSMAUI.Api.Modal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjectGSMAUI.Api.Container;
using System;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
 throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString)); // Changed to AppDbContext

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPhimService, PhimService>();
builder.Services.AddScoped<IPhongService, PhongService>();
builder.Services.AddScoped<IVeService, VeService>();
builder.Services.AddScoped<IVoucherServices, VoucherServices>();
builder.Services.AddScoped<IGiamGiaServices, TaiKhoanServices>();
builder.Services.AddScoped<ITheLoaiPhimService, TheLoaiPhimService>();
builder.Services.AddScoped<ILichChieuService, LichChieuService>();
builder.Services.AddTransient<IRefreshHandler, RefreshHandler>();
builder.Services.AddTransient<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddScoped<ISanPham, SanPhamService>();
builder.Services.AddScoped<IBillMServices, BillMServices>();
builder.Services.AddScoped<IGeminiServices, GeminiServices>();
builder.Services.AddScoped<ICheckOutServices, CheckOutServices>();
builder.Services.AddScoped<IThongKe, ThongKeService>();
//builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
var _authkey = builder.Configuration.GetValue<string>("JwtSettings:securitykey");
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

});
var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = automapper.CreateMapper();
//builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
//{
//    build.WithOrigins("https://domain1.com", "https://domain2.com").AllowAnyMethod().AllowAnyHeader();
// }));
//builder.Services.AddCors(p => p.AddPolicy("corspolicy1", build =>
//{
//    build.WithOrigins("https://domain3.com").AllowAnyMethod().AllowAnyHeader();
// }));
builder.Services.AddCors();

builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedwindow", option =>
{
    option.Window = TimeSpan.FromSeconds(10);
    option.PermitLimit = 5;
    option.QueueLimit = 0;
    option.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
}).RejectionStatusCode = 401);
builder.Services.AddSingleton(mapper);
builder.Services.AddDistributedMemoryCache(); // Thêm cache cho session
//Gemini
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<GeminiSettings>>().Value);
builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection("Authentication"));
// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout after 30 minutes
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
string logpath = builder.Configuration.GetSection("Logging:LogPath").Value;
var _logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
     .WriteTo.File(logpath)
   .CreateLogger();
builder.Logging.AddSerilog(_logger);

var _jwtsetting = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtsetting);
var app = builder.Build();
app.UseRateLimiter();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseCors("corspolicy");
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
   .SetIsOriginAllowed(origin => true)
    .AllowCredentials());
app.UseHttpsRedirection();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();