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
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.RoleList = _roleMgr.Roles
                    .Select(r => new SelectListItem(r.Name, r.Name))
                    .ToList();
                return View(vm);
            }

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

            await _userMgr.AddToRoleAsync(user, vm.Role);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Accounts/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userMgr.FindByIdAsync(id);
            if (user == null) return NotFound();

            var userRoles = await _userMgr.GetRolesAsync(user);
            var vm = new EditAccountViewModel
            {
                Id = user.Id,
                Username = user.UserName!,
                Role = userRoles.FirstOrDefault() ?? "",
                RoleList = _roleMgr.Roles
                    .Select(r => new SelectListItem(r.Name, r.Name))
                    .ToList()
            };
            return View(vm);
        }

        // POST: Admin/Accounts/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditAccountViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.RoleList = _roleMgr.Roles
                    .Select(r => new SelectListItem(r.Name, r.Name))
                    .ToList();
                return View(vm);
            }

            var user = await _userMgr.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            // 1) Оновлюємо логін
            user.UserName = vm.Username;
            var updRes = await _userMgr.UpdateAsync(user);
            if (!updRes.Succeeded)
            {
                foreach (var e in updRes.Errors)
                    ModelState.AddModelError("", e.Description);

                vm.RoleList = _roleMgr.Roles
                    .Select(r => new SelectListItem(r.Name, r.Name))
                    .ToList();
                return View(vm);
            }

            // 2) Оновлюємо роль
            var currentRoles = await _userMgr.GetRolesAsync(user);
            await _userMgr.RemoveFromRolesAsync(user, currentRoles);
            await _userMgr.AddToRoleAsync(user, vm.Role);

            // 3) Якщо заданий NewPassword – міняємо пароль
            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                var token = await _userMgr.GeneratePasswordResetTokenAsync(user);
                var pwRes = await _userMgr.ResetPasswordAsync(user, token, vm.NewPassword);
                if (!pwRes.Succeeded)
                {
                    foreach (var e in pwRes.Errors)
                        ModelState.AddModelError("", e.Description);

                    vm.RoleList = _roleMgr.Roles
                        .Select(r => new SelectListItem(r.Name, r.Name))
                        .ToList();
                    return View(vm);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Accounts/Delete/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userMgr.FindByIdAsync(id);
            if (user != null)
            {
                await _userMgr.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
