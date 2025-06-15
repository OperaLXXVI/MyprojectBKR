using System;

namespace InterComCore.Entities
{
    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string HtmlContent { get; set; } = "";

        // добавляем эти два свойства
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
    }
}
