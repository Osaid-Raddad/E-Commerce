using E_Commerce.BLL.Services.Classes;
using E_Commerce.BLL.Services.Interfaces;
using E_Commerce.DAL.Repository.Classes;
using E_Commerce.DAL.Repository.Interfaces;
using E_Commerce.DAL.Utils;
using Stripe;

namespace E_Commerce.PL.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services, IConfiguration Configuration)
        {

            Services.AddScoped<ICategoryRepository, CategoryRepository>();

            Services.AddScoped<ICategoryService, CategoryService>();

            Services.AddScoped<IAuthenticationService, AuthenticationService>();

            Services.AddScoped<ISeedData, RoleSeedData>();

            Services.AddTransient<IEmailSender, EmailSender>();

            Services.AddScoped<IFileService, BLL.Services.Classes.FileService>();

            Services.AddScoped<IProductService, BLL.Services.Classes.ProductService>();

            Services.AddScoped<IProductRepository, ProductRepository>();

            Services.AddScoped<ICartRepository, CartRepository>();
            
            Services.AddScoped<ICartService, CartService>();

            Services.AddScoped<ICheckoutService, BLL.Services.Classes.CheckoutService>();


            Services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = Configuration["Stripe:SecretKey"];

            return Services;
        }
    }
}
