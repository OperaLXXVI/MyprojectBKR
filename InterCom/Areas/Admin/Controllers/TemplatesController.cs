using System;
using System.Linq;
using System.Threading.Tasks;
using InterComCore.Entities;
using InterComInfrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        {
            var list = await _db.Templates
                                .AsNoTracking()
                                .ToListAsync();
            return View(list);
        }

        // GET: Admin/Templates/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tpl = await _db.Templates
                               .AsNoTracking()
                               .FirstOrDefaultAsync(t => t.Id == id);
            if (tpl == null) return NotFound();

            return View(tpl);
        }

        // GET: Admin/Templates/Create
        public IActionResult Create() => View(new Template());

        // POST: Admin/Templates/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Template template)
        {
            if (!ModelState.IsValid)
                return View(template);

            template.CreatedAt = DateTime.UtcNow;
            template.CreatedBy = User.Identity?.Name ?? "—";

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
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Template template)
        {
            if (!ModelState.IsValid)
                return View(template);

            _db.Templates.Update(template);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Templates/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
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
