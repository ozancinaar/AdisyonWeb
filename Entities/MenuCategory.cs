using System.Collections.Generic;

namespace AdisyonWeb.Entities
{
    public class MenuCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}
