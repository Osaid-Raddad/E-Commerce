using E_Commerce.DAL.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCart(AddToCartRequest request, string UserId);
    }
}
