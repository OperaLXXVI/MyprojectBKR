using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterComCore.Entities
{
    public class Placeholder
    {
        public int Id { get; set; }
        public string Key { get; set; }                // Наприклад "{{ClientName}}"
        public string Description { get; set; }        // Підказка або опис для UI

        
    }
}
