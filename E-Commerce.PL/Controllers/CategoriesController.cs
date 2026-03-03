using E_Commerce.DAL.Data;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using E_Commerce.DAL.Repository;
using E_Commerce.PL.Resources;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace E_Commerce.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CategoriesController(ICategoryRepository categoryRepository, IStringLocalizer<SharedResources> Localizer)
        {  
            _categoryRepository = categoryRepository;
            _localizer = Localizer;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var categories = _categoryRepository.GetAll();
            var response = categories.Adapt<List<CategoryResponse>>();
            return Ok(new {
                data= response,
                _localizer["Success"].Value
            });
        }

        [HttpPost("")]
        public IActionResult Create(CategoryRequest categoryRequest)
        {
            var category = categoryRequest.Adapt<Category>();
            _categoryRepository.Create(category);
            return Ok(new { _localizer["Succes"].Value });
        }
    }
}