using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InterCom.ViewModels
{
    public class CreateAccountViewModel
    {
        [Required]
        [Display(Name = "Логін")]
        public string Username { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";

        [Required]
        [Display(Name = "Роль")]
        public string Role { get; set; } = "";

        // Список ролей для дропдауну
        public IEnumerable<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();
    }
}
