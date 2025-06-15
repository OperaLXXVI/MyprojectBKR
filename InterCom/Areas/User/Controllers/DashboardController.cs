using InterCom.ViewModels;
using InterComInfrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace InterCom.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly InterComDbContext _db;
        public DashboardController(InterComDbContext db) => _db = db;

        // GET: User/Dashboard
        public async Task<IActionResult> Index()
        {
            var list = await _db.Templates
                .Select(t => new TemplateViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    CreatedAt = t.CreatedAt,
                    CreatedByUserName = _db.Users
                                           .Where(u => u.Id == t.CreatedBy)
                                           .Select(u => u.UserName)
                                           .FirstOrDefault() ?? "—"
                })
                .OrderByDescending(vm => vm.CreatedAt)
                .ToListAsync();

            return View(list);
        }
    }
}
