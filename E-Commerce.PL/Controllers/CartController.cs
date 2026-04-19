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

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> AddToCart(AddToCartRequest request)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.AddToCart(request, UserId);

            return Ok(new
            {
                message = _localizer["Success"].Value,
            });
        }
    }
}
