using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

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
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(
//         builder.Configuration.GetConnectionString("DefaultConnection")
//     ));



var provider = builder.Configuration["DatabaseProvider"];

string[] connections =
{
    builder.Configuration.GetConnectionString("SqlServerConnection"),
    builder.Configuration.GetConnectionString("ExpressConnection"),
    builder.Configuration.GetConnectionString("LocalDBConnection")
};

string? workingConnection = null;

foreach (var conn in connections)
{
    try
    {
        using var sqlConnection = new SqlConnection(conn);
        sqlConnection.Open();

        workingConnection = conn;
        System.Console.WriteLine($"Подключено к: {conn}");

        break;
    }
    catch
    {
        System.Console.WriteLine($"Не удалось подключиться: {conn}");
    }
}

if (workingConnection == null)
{
    throw new Exception("Не удалось найти доступный SQL Server");
}

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(workingConnection));











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

    await ASSC.Data.DbInitializer.SeedAdminAsync(services);
}

app.Run();