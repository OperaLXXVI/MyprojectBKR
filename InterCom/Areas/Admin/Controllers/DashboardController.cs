using InterComInfrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InterCom.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly InterComDbContext _db;
        public DashboardController(InterComDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            // Вивести всі укладені контракти
            var list = await _db.Contracts
                                .Include(c => c.Template)
                                .ToListAsync();
            return View(list);
        }
    }
}
