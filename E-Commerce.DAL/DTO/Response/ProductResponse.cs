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
        public string MainImage { get; set; }
        public string UserCreated { get; set; }
    }
}
