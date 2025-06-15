using InterComCore.Entities;
using InterComInfrastructure.Data;
using InterComInfrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InterCom.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly InterComDbContext _db;
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;

        public ContractsController(
            InterComDbContext db,
            UserManager<IdentityUser> userMgr,
            IPdfService pdfService,
            IEmailService emailService)
        {
            _db = db;
            _userMgr = userMgr;
            _pdfService = pdfService;
            _emailService = emailService;
        }

        // GET: User/Contracts
        public async Task<IActionResult> Index()
        {
            var userId = _userMgr.GetUserId(User);
            var contracts = await _db.Contracts
                                     .Include(c => c.Template)      // <-- підвантажуємо назву шаблону
                                     .Where(c => c.UserId == userId)
                                     .OrderByDescending(c => c.CreatedAt)
                                     .ToListAsync();
            return View(contracts);
        }

        // GET: User/Contracts/Create?templateId=...
        [HttpGet]
        public async Task<IActionResult> Create(int templateId)
        {
            var tpl = await _db.Templates.FindAsync(templateId);
            if (tpl == null) return NotFound();

            // всі ключі з БД
            var allDbKeys = await _db.Placeholders
                                     .Select(p => p.Key)
                                     .ToListAsync();

            // ключі з HTML, що є в БД
            var htmlKeys = Regex.Matches(tpl.HtmlContent, "{{(.*?)}}")
                                .Select(m => m.Groups[1].Value)
                                .Distinct();
            var keys = htmlKeys.Intersect(allDbKeys).ToList();

            ViewBag.HtmlTemplate = tpl.HtmlContent;
            ViewBag.PlaceholderKeys = keys;
            ViewBag.TemplateId = tpl.Id;

            return View();
        }

        // POST: User/Contracts/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int templateId, Dictionary<string, string> form)
        {
            var tpl = await _db.Templates.FindAsync(templateId);
            if (tpl == null) return NotFound();

            // повторюємо пошук ключів у HTML і БД
            var allDbKeys = await _db.Placeholders
                                     .Select(p => p.Key)
                                     .ToListAsync();
            var htmlKeys = Regex.Matches(tpl.HtmlContent, "{{(.*?)}}")
                                .Select(m => m.Groups[1].Value)
                                .Distinct();
            var keys = htmlKeys.Intersect(allDbKeys).ToList();

            // формуємо словник значень
            var values = keys.ToDictionary(
                k => k,
                k => form.ContainsKey(k) ? form[k] : string.Empty
            );

            var userId = _userMgr.GetUserId(User);

            var contract = new Contract
            {
                TemplateId = templateId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                PlaceholderValuesJson = JsonSerializer.Serialize(values)
            };

            _db.Contracts.Add(contract);
            await _db.SaveChangesAsync();

            // підставляємо й генеруємо PDF
            var html = tpl.HtmlContent;
            foreach (var kv in values)
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            var pdfBytes = _pdfService.GeneratePdf(html);

            // надсилаємо на ClientEmail
            if (values.TryGetValue("ClientEmail", out var toEmail)
                && !string.IsNullOrWhiteSpace(toEmail))
            {
                await _emailService.SendAsync(
                    toEmail,
                    $"Договір #{contract.Id} від InterCom",
                    "<p>Доброго дня!</p><p>Ваш договір у вкладенні.</p>",
                    pdfBytes,
                    $"Contract_{contract.Id}.pdf"
                );
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: User/Contracts/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _db.Contracts
                                    .Include(c => c.Template)
                                    .FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null) return NotFound();

            var values = contract.GetValues();
            var html = contract.Template.HtmlContent;
            foreach (var kv in values)
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);

            ViewBag.RenderedHtml = html;
            return View(contract);
        }

        // GET: User/Contracts/Download/5
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var contract = await _db.Contracts
                                    .Include(c => c.Template)
                                    .FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null) return NotFound();

            var values = contract.GetValues();
            var html = contract.Template.HtmlContent;
            foreach (var kv in values)
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);

            var pdfBytes = _pdfService.GeneratePdf(html);
            return File(pdfBytes, "application/pdf", $"Contract_{contract.Id}.pdf");
        }
    }
}
