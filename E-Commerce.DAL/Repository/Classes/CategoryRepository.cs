using E_Commerce.DAL.Data;
using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using E_Commerce.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Repository.Classes
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
       public CategoryRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
