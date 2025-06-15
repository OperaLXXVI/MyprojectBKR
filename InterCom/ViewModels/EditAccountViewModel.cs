using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterCom.ViewModels
{
    public class EditAccountViewModel
    {
        [Required]
        public string Id { get; set; } = "";

        [Required, Display(Name = "Логін")]
        public string Username { get; set; } = "";

        [Display(Name = "Нова роль")]
        public string Role { get; set; } = "";

        public List<SelectListItem> RoleList { get; set; } = new();

        // Нові поля для заданя/зміни паролю
        [DataType(DataType.Password), Display(Name = "Новий пароль")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password), Display(Name = "Підтвердження паролю")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
        public string? ConfirmPassword { get; set; }
    }
}
