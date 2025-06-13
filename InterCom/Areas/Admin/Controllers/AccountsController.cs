using InterCom.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace InterCom.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public AccountsController(
            UserManager<IdentityUser> userMgr,
            RoleManager<IdentityRole> roleMgr)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        // GET: Admin/Accounts
        public IActionResult Index()
        {
            var users = _userMgr.Users.ToList();
            return View(users);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            var vm = new CreateAccountViewModel
            {
                RoleList = _roleMgr.Roles
                    .Select(r => new SelectListItem(r.Name, r.Name))
                    .ToList()
            };
            return View(vm);
        }

        // POST: Admin/Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // якщо не валідно — знову підвантажуємо список ролей
                vm.RoleList = _roleMgr.Roles
                    .Select(r => new SelectListItem(r.Name, r.Name))
                    .ToList();
                return View(vm);
            }

            // створюємо користувача
            var user = new IdentityUser { UserName = vm.Username };
            var res = await _userMgr.CreateAsync(user, vm.Password);
            if (!res.Succeeded)
            {
                foreach (var e in res.Errors)
                    ModelState.AddModelError("", e.Description);
                vm.RoleList = _roleMgr.Roles
                    .Select(r => new SelectListItem(r.Name, r.Name))
                    .ToList();
                return View(vm);
            }

            // призначаємо роль
            await _userMgr.AddToRoleAsync(user, vm.Role);
            return RedirectToAction(nameof(Index));
        }

        // ... Edit/Delete ті ж самі, якщо треба ...
    }
}
