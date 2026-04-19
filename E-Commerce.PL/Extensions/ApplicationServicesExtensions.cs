using E_Commerce.BLL.Services.Classes;
using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.Repository.Classes;
using E_Commerce.DAL.Repository.Interfaces;
using E_Commerce.DAL.Utils;

namespace E_Commerce.PL.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {

            Services.AddScoped<ICategoryRepository, CategoryRepository>();

            Services.AddScoped<ICategoryService, CategoryService>();

            Services.AddScoped<IAuthenticationService, AuthenticationService>();

            Services.AddScoped<ISeedData, RoleSeedData>();

            Services.AddTransient<IEmailSender, EmailSender>();

            Services.AddScoped<IFileService, FileService>();

            Services.AddScoped<IProductService, ProductService>();

            Services.AddScoped<IProductRepository, ProductRepository>();

            Services.AddScoped<ICartRepository, CartRepository>();
            
            Services.AddScoped<ICartService, CartService>();

            return Services;
        }
    }
}
