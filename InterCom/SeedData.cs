using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using InterComCore.Entities;
using InterComInfrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterCom
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            // Отримуємо RoleManager, UserManager та DbContext
            var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = services.GetRequiredService<UserManager<IdentityUser>>();
            var db = services.GetRequiredService<InterComDbContext>();

            // 1) Створюємо ролі, якщо їх нема
            foreach (var role in new[] { "Admin", "User" })
            {
                if (!await roleMgr.RoleExistsAsync(role))
                    await roleMgr.CreateAsync(new IdentityRole(role));
            }

            // 2) Створюємо локального адміна
            const string adminUser = "admin";
            const string adminPwd = "Admin123!";

            var admin = await userMgr.FindByNameAsync(adminUser);
            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminUser,
                    Email = $"{adminUser}@local",
                    EmailConfirmed = true
                };

                var result = await userMgr.CreateAsync(admin, adminPwd);
                if (!result.Succeeded)
                {
                    throw new Exception("Не вдалося створити адміністратора: " +
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }

                await userMgr.AddToRoleAsync(admin, "Admin");
            }

            // 3) Сідуємо плейсхолдери, якщо їх ще нема в таблиці
            if (!db.Placeholders.Any())
            {
                var placeholders = new List<Placeholder>
                {
                    new() { Key = "ContractNumber", Description = "Номер договору" },
                    new() { Key = "ClientAddress",  Description = "Адреса клієнта" },
                    new() { Key = "ClientName",     Description = "ПІБ клієнта" },
                    new() { Key = "ClientEmail",    Description = "Email клієнта" },
                    new() { Key = "ClientPhone",    Description = "Телефон клієнта" },
                    new() { Key = "PassportData",   Description = "Паспортні дані" },
                    new() { Key = "EquipmentList",  Description = "Список обладнання" },
                    new() { Key = "TotalCost",      Description = "Загальна вартість" },
                    new() { Key = "ContractDate",   Description = "Дата договору" },
                    new() { Key = "TariffName",     Description = "Назва тарифу" },
                    new() { Key = "TariffSpeed",    Description = "Швидкість тарифу" },
                    new() { Key = "TariffPrice",    Description = "Ціна тарифу" },
                };

                db.Placeholders.AddRange(placeholders);
                await db.SaveChangesAsync();
            }
        }
    }
}
