using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Models
{
    public class Product
    {
        public Product()
        {
            ImageProducts = new HashSet<ImageProduct>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public ICollection<ImageProduct> ImageProducts { get; set; }
        public Brand Brand { get; set; }
        public int BrandId { get; set; }
    }

}
