using InterComCore.Entities;
using InterComInfrastructure.Data;
using InterComInfrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            var userId = _userMgr.GetUserId(User);
            var contracts = _db.Contracts
                               .Where(c => c.UserId == userId)
                               .ToList();
            return View(contracts);
        }

        // GET: User/Contracts/Create?templateId=...
        [HttpGet]
        public async Task<IActionResult> Create(int templateId)
        {
            var tpl = await _db.Templates.FindAsync(templateId);
            if (tpl == null) return NotFound();

            // Витягуємо всі ключі {{Key}} із HTML
            var keys = Regex
                .Matches(tpl.HtmlContent, "{{(.*?)}}")
                .Select(m => m.Groups[1].Value)
                .Distinct();

            ViewBag.HtmlTemplate = tpl.HtmlContent;
            ViewBag.PlaceholderKeys = keys;
            ViewBag.TemplateId = tpl.Id;

            return View();
        }

        // POST: User/Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int templateId, Dictionary<string, string> form)
        {
            var tpl = await _db.Templates.FindAsync(templateId);
            if (tpl == null) return NotFound();

            // Збираємо ключі з шаблону
            var keys = Regex
                .Matches(tpl.HtmlContent, "{{(.*?)}}")
                .Select(m => m.Groups[1].Value)
                .Distinct();

            // Формуємо словник значень із всіх плейсхолдерів
            var values = keys.ToDictionary(
                k => k,
                k => form.ContainsKey(k) ? form[k] : string.Empty
            );

            var userId = _userMgr.GetUserId(User);

            // Створюємо запис у БД
            var contract = new Contract
            {
                TemplateId = templateId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                PlaceholderValuesJson = JsonSerializer.Serialize(values)
            };

            _db.Contracts.Add(contract);
            await _db.SaveChangesAsync();

            // Підставляємо значення в HTML
            var html = tpl.HtmlContent;
            foreach (var kv in values)
            {
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            // Генеруємо PDF
            var pdfBytes = _pdfService.GeneratePdf(html);

            // Відправляємо на email, взятий із плейсхолдера ClientEmail
            if (values.TryGetValue("ClientEmail", out var toEmail)
                && !string.IsNullOrWhiteSpace(toEmail))
            {
                var subject = $"Договір #{contract.Id} від InterCom";
                var bodyHtml = "<p>Доброго дня!</p><p>Ваш договір у вкладенні.</p>";
                await _emailService.SendAsync(
                    toEmail,
                    subject,
                    bodyHtml,
                    pdfBytes,
                    $"Contract_{contract.Id}.pdf");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: User/Contracts/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _db.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            var tpl = await _db.Templates.FindAsync(contract.TemplateId);
            var values = contract.GetValues();

            var html = tpl.HtmlContent;
            foreach (var kv in values)
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);

            ViewBag.RenderedHtml = html;
            return View(contract);
        }

        // GET: User/Contracts/Download/5
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var contract = await _db.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            var tpl = await _db.Templates.FindAsync(contract.TemplateId);
            var values = contract.GetValues();

            var html = tpl.HtmlContent;
            foreach (var kv in values)
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);

            var pdfBytes = _pdfService.GeneratePdf(html);
            var fileName = $"Contract_{contract.Id}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
