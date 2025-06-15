// Проект InterCom/ViewModels/TemplateViewModel.cs
namespace InterCom.ViewModels
{
    public class TemplateViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string CreatedByUserName { get; set; } = "";
    }
}
