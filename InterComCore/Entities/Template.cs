using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterComCore.Entities
{
    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; }               // Назва шаблона (наприклад «Договір інтернет»)
        public string HtmlContent { get; set; }        // HTML-шаблон з плейсхолдерами

        // Пізніше тут можна додати навігаційні властивості до Placeholder
        // public ICollection<Placeholder> Placeholders { get; set; }
    }
}
