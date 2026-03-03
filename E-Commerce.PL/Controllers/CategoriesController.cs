using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.Data;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using E_Commerce.DAL.Repository.Interfaces;
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
        private readonly ICategoryService _categoryService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public CategoriesController(ICategoryService categoryService, IStringLocalizer<SharedResources> Localizer)
        {  
            _categoryService = categoryService;
            _localizer = Localizer;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var response = _categoryService.GetAllCategories();
            return Ok(new {
                data= response,
                _localizer["Success"].Value
            });
        }

        [HttpPost("")]
        public IActionResult Create(CategoryRequest categoryRequest)
        {
            var response = _categoryService.CreateCategory(categoryRequest);
            return Ok(new {
                _localizer["Succes"].Value,
                response
            });
        }
    }
}