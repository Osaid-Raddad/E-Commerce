using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using E_Commerce.DAL.Repository.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Classes
{
    public class CategoryService : Interfaces.ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public CategoryResponse CreateCategory(CategoryRequest categoryRequest)
        {
            var category = categoryRequest.Adapt<Category>();
            _categoryRepository.Create(category);
            return category.Adapt<CategoryResponse>();
        }

        public List<CategoryResponse> GetAllCategories()
        {
            var categories = _categoryRepository.GetAll();   

            return categories.Adapt<List<CategoryResponse>>();
        }
    }
}
