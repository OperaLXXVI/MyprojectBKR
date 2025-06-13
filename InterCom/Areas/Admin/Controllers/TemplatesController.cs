using InterComCore.Entities;
using InterComInfrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InterCom.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TemplatesController : Controller
    {
        private readonly InterComDbContext _db;
        public TemplatesController(InterComDbContext db) => _db = db;

        // GET: Admin/Templates
        public async Task<IActionResult> Index()
            => View(await _db.Templates.ToListAsync());

        // GET: Admin/Templates/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tpl = await _db.Templates.FindAsync(id);
            if (tpl == null) return NotFound();
            return View(tpl);
        }

        // GET: Admin/Templates/Create
        public IActionResult Create()
            => View();

        // POST: Admin/Templates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Template template)
        {
            if (!ModelState.IsValid)
                return View(template);

            _db.Templates.Add(template);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Templates/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tpl = await _db.Templates.FindAsync(id);
            if (tpl == null) return NotFound();
            return View(tpl);
        }

        // POST: Admin/Templates/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Template template)
        {
            if (!ModelState.IsValid)
                return View(template);

            _db.Templates.Update(template);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Templates/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var tpl = await _db.Templates.FindAsync(id);
            if (tpl != null)
            {
                _db.Templates.Remove(tpl);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
