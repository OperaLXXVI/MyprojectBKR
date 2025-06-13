using InterCom;
using InterComInfrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DinkToPdf;
using DinkToPdf.Contracts;
using InterComInfrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Підключаємо DbContext із Infrastructure
builder.Services.AddDbContext<InterComDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Налаштовуємо Identity із ролями
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        // тут можна додати політики паролів тощо
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<InterComDbContext>();

// 2.1. Конфігуруємо кукі, щоб вхід був по /Account/Login
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

// 3. Регіструємо сервіси для PDF та пошти
// 3.1. DinkToPdf конвертер
builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IPdfService, PdfService>();
// 3.2. MailKit email-сервіс
builder.Services.AddScoped<IEmailService, EmailService>();

// 4. Додаємо MVC
builder.Services.AddControllersWithViews();
// Якщо не використовуєте вбудовані Razor Pages для Identity UI — можна закоментувати
builder.Services.AddRazorPages();

var app = builder.Build();

// 5. Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 6. Аутентифікація та авторизація
app.UseAuthentication();
app.UseAuthorization();

// 7. Маршрутизація з підтримкою Area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Якщо використовуєте Identity UI — залиште. Інакше можна видалити.
app.MapRazorPages();

// 8. Сидимо ролі та початкового Admin
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

app.Run();
