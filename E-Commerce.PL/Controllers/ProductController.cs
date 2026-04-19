using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace E_Commerce.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public ProductController(IProductService productService, IStringLocalizer<SharedResources> Localizer)
        {
            _productService = productService;
            _localizer = Localizer;
        }


        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();

            return Ok(new
            {
                data = products,
                _localizer["Success"].Value
            });
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProduct(p => p.Id == id);

            if (product == null) return NotFound();
            return Ok(new
            {
                data = product
            });
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateRequest request)
        {
            var updated = await _productService.UpdateProduct(id, request);

            if (!updated) return BadRequest();

            return Ok();
        }


        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ProductRequest request)
        {
            await _productService.CreateProduct(request);
            return Ok(new
            {
                Message = "Product created successfully"
            });
        }

        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var updated = await _productService.ToggleStatus(id);

            if (!updated) return BadRequest();

            return Ok();
        }

    }
}
