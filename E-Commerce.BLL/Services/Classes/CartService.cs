using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using E_Commerce.DAL.Repository.Classes;
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
        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository) 
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            
        }

        public async Task<bool> AddToCart(AddToCartRequest request, string UserId)
        {
            var product = await _productRepository.GetOneAsync(p => p.Id == request.ProductId);
            if (product is null) return false;

            var ExistingItem = await _cartRepository.GetOneAsync(
                c => c.ProductId == request.ProductId && c.UserId == UserId
            );

            var currentCount = ExistingItem?.Count ?? 0;
            var newCount = currentCount + request.Count;

            if (newCount > product.Quantity) return false;

            if (ExistingItem != null)
            {
                ExistingItem.Count = newCount;
                await _cartRepository.UpdateAsync(ExistingItem);
            }
            else
            {
                var cartItem = request.Adapt<Cart>();
                cartItem.UserId = UserId;
                await _cartRepository.CreateAsync(cartItem);
            }

            return true;
        }

        public Task<bool> ClearCart(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CartResponse>> GetCart(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveItem(int productId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateQuantity(int productId, int count, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
