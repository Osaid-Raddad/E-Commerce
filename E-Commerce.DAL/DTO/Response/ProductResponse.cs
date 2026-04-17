using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.DTO.Response
{
    public class ProductResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public string MainImage { get; set; }
        public string UserCreated { get; set; }
    }
}
