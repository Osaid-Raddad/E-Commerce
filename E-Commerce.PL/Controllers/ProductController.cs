using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();

            return Ok(new
            {
                data = products
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

        
    }
}
