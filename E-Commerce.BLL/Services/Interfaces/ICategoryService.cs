using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        List<CategoryResponse> GetAllCategories();
        CategoryResponse CreateCategory(CategoryRequest categoryRequest);

    }
}
