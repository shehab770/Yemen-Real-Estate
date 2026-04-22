using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using webProgramming.Models;
using webProgramming.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "EstateApp.Cookie";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("Server=ROG;Database=estatedb;Trusted_Connection=True;TrustServerCertificate=True;Pooling=true;Min Pool Size=5;Max Pool Size=100;");
});
// Important for Sessions - ADD BEFORE app.Build()
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddSignalR();

builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapHub<ChatHub>("/chathub");
app.UseHttpsRedirection();
app.UseStaticFiles(); // you missed this line for static assets

app.UseRouting();

// Session Middleware - ADD AFTER UseRouting, BEFORE Authorization
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
