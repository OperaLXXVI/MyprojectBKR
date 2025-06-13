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
    public class PlaceholdersController : Controller
    {
        private readonly InterComDbContext _db;
        public PlaceholdersController(InterComDbContext db) => _db = db;

        // GET: Admin/Placeholders
        public async Task<IActionResult> Index()
        {
            var items = await _db.Placeholders.ToListAsync();
            return View(items);
        }

        // GET: Admin/Placeholders/Create
        public IActionResult Create() => View(new Placeholder());

        // POST: Admin/Placeholders/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Placeholder p)
        {
            if (!ModelState.IsValid) return View(p);
            _db.Placeholders.Add(p);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Placeholders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Placeholders.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: Admin/Placeholders/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Placeholder p)
        {
            if (!ModelState.IsValid) return View(p);
            _db.Placeholders.Update(p);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET or POST: Admin/Placeholders/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Placeholders.FindAsync(id);
            if (p != null)
            {
                _db.Placeholders.Remove(p);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
