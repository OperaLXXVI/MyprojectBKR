using InterCom;
using InterComInfrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DinkToPdf;
using DinkToPdf.Contracts;
using InterComInfrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. ϳ�������� DbContext �� Infrastructure
builder.Services.AddDbContext<InterComDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. ����������� Identity �� ������
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        // ��� ����� ������ ������� ������ ����
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<InterComDbContext>();

// 2.1. ����������� ���, ��� ���� ��� �� /Account/Login
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

// 3. ��������� ������ ��� PDF �� �����
// 3.1. DinkToPdf ���������
builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IPdfService, PdfService>();
// 3.2. MailKit email-�����
builder.Services.AddScoped<IEmailService, EmailService>();

// 4. ������ MVC
builder.Services.AddControllersWithViews();
// ���� �� ������������� �������� Razor Pages ��� Identity UI � ����� �������������
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

// 6. �������������� �� �����������
app.UseAuthentication();
app.UseAuthorization();

// 7. ������������� � ��������� Area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ���� ������������� Identity UI � �������. ������ ����� ��������.
app.MapRazorPages();

// 8. ������ ��� �� ����������� Admin
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

app.Run();
