using E_Commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task <List<Category>> GetAllAsync();
        Task <Category> CreateAsync(Category category);
    }
}
