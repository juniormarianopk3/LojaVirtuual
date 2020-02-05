using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LojaVirtual.Models.ViewModels
{
    public class ProductViewModels
    {
        public Product Product { get; set; }
        public ImageProduct ImageProduct { get; set; }
        public Brand Brand { get; set; }
        public Department Department { get; set; }
    }
}
