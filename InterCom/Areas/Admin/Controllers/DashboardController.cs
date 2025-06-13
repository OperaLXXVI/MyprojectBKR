using InterComCore.Entities;
using InterComInfrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace InterCom.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly InterComDbContext _db;
        public DashboardController(InterComDbContext db) => _db = db;

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Index()
        {
            // Fetch contracts with related template
            var contracts = await _db.Contracts
                                     .Include(c => c.Template)
                                     .OrderByDescending(c => c.CreatedAt)
                                     .ToListAsync();
            return View(contracts);
        }

        // GET: /Admin/Dashboard/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _db.Contracts
                                    .Include(c => c.Template)
                                    .FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null) return NotFound();

            // Deserialize placeholder values
            var values = contract.GetValues();

            // Render HTML by replacing placeholders
            var html = contract.Template.HtmlContent;
            foreach (var kv in values)
            {
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            ViewBag.RenderedHtml = html;
            return View(contract);
        }

        // POST: /Admin/Dashboard/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Contracts.FindAsync(id);
            if (c != null)
            {
                _db.Contracts.Remove(c);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
