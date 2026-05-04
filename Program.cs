using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using ASSC.Data;
using ASSC.Services;
using ASSC.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserContextService>();

// builder.Services
//     .AddIdentity<ApplicationUser, IdentityRole>()
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<RussianIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        await ASSC.Data.DbInitializer.SeedAdminAsync(services);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[WARNING] Не удалось выполнить инициализацию базы данных. \n ");
        Console.WriteLine("Причина: " + ex.Message);
        Console.WriteLine("Приложение запущено, но некоторые функции могут быть недоступны без SQL Server.");
        Console.ResetColor();
    }
}

app.Run();