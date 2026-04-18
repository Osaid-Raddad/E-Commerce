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
    public interface IProductService
    {
        Task CreateProduct(ProductRequest request);
        Task<List<ProductResponse>> GetAllProductsAsync();

        Task<ProductResponse?> GetProduct(Expression<Func<Product, bool>> filter);

        Task <bool> UpdateProduct(int id, ProductUpdateRequest request);  
        Task<bool> DeleteProduct(int id);
    }
}
