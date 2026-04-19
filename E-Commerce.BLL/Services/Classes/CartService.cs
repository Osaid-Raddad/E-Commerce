using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.Models;
using E_Commerce.DAL.Repository.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Classes
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository) 
        {
            _cartRepository = cartRepository;
        }

        public async Task AddToCart(AddToCartRequest request, string UserId)
        {
            var ExistingItem = await _cartRepository.GetOneAsync(
                   c => c.ProductId == request.ProductId && c.UserId == UserId
            );

            if (ExistingItem != null)
            {
                ExistingItem.Count += request.Count;
                await _cartRepository.UpdateAsync(ExistingItem);
            }
            else
            {
                var cartItem = request.Adapt<Cart>();
                cartItem.UserId = UserId;
                await _cartRepository.CreateAsync(cartItem);
            }
        }
    }
}
