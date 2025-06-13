using InterComCore.Entities;
using InterComInfrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InterCom.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]  // будь-хто, хто залогінений
    public class DashboardController : Controller
    {
        private readonly InterComDbContext _db;
        public DashboardController(InterComDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            // Список усіх доступних шаблонів
            var templates = await _db.Templates.ToListAsync();
            return View(templates);
        }
    }
}
