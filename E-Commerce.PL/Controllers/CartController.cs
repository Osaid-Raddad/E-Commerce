using E_Commerce.BLL.Services.Classes;
using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace E_Commerce.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CartController(ICartService cartService, IStringLocalizer<SharedResources> Localizer) 
        {
            _cartService = cartService;
            _localizer = Localizer;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetCart()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = await _cartService.GetCart(UserId);

            return Ok(new { data = items });
        }


        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> AddToCart(AddToCartRequest request)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.AddToCart(request, UserId);
            if(!result)
            {
                return BadRequest(new
                {
                    message = _localizer["Failed"].Value,
                });
            }
            return Ok(new
            {
                message = _localizer["Success"].Value,
            });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem([FromRoute] int productId)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var removed = await _cartService.RemoveItem(productId, UserId);

            if (!removed) return BadRequest();

            return Ok();
        }

        [HttpPatch("{productId}")]
        public async Task<IActionResult> UpdateQuantity([FromRoute] int productId, [FromBody] UpdateCartRequest request)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updated = await _cartService.UpdateQuantity(productId, request.Count, UserId);

            if (!updated) return BadRequest();

            return Ok();
        }
    }
}
