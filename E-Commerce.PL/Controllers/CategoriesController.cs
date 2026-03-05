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
        public async Task<IActionResult> Get()
        {
            var response = await _categoryService.GetAllCategoriesAsync();
            return Ok(new
            {
                data = response,
                _localizer["Success"].Value
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _categoryService.GetCategory(c => c.Id == id));
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _categoryService.GetCategory(
                c => c.Translations.Any(t => t.Name == name)
            );

            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> Create(CategoryRequest categoryRequest)
        {
            var response = await _categoryService.CreateCategoryAsync(categoryRequest);
            return Ok(new
            {
                _localizer["Succes"].Value,
                response
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result)
            {
                return Ok(new { _localizer["Deleted"].Value });
            }
            return NotFound(new { _localizer["NotFound"].Value });

        }
    }
}