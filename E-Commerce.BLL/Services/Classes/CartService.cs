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

        public async Task<bool> ClearCart(string userId)
        {
            var items = await _cartRepository.GetAllAsync(
                filter: c => c.UserId == userId
            );

            if (!items.Any()) return false;

            await _cartRepository.DeleteRangeAsync(items);
            return true;
        }

        public async Task<List<CartResponse>> GetCart(string userId)
        {
            var items = await _cartRepository.GetAllAsync(

                 filter: c => c.UserId == userId,
                 includes: new string[]
                 {
                    nameof(Cart.Product),
                    $"{nameof(Cart.Product)}.{nameof(Product.Translations)}"
                 }

            );

            return items.Adapt<List<CartResponse>>();
        }

        public async Task<bool> RemoveItem(int productId, string userId)
        {
            var item = await _cartRepository.GetOneAsync(
               c => c.ProductId == productId && c.UserId == userId
            );

            if (item is null) return false;

            return await _cartRepository.DeleteAsync(item);
        }

        public async Task<bool> UpdateQuantity(int productId, int count, string userId)
        {
            var item = await _cartRepository.GetOneAsync(
                c => c.ProductId == productId && c.UserId == userId
            );

            if (item is null) return false;

            var product = await _productRepository.GetOneAsync(p => p.Id == productId);

            if (count > product.Quantity) return false;

            item.Count = count;

            return await _cartRepository.UpdateAsync(item);
        }
    }
}
