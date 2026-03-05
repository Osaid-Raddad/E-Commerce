using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        Task <List<CategoryResponse>> GetAllCategoriesAsync();
        Task <CategoryResponse> CreateCategoryAsync(CategoryRequest categoryRequest);
        Task<CategoryResponse> GetCategory(Expression<Func<Category, bool>> filter);

        Task <bool> DeleteCategoryAsync(int id);
    }
}
