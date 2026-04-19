using E_Commerce.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.PL.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatatbaseServices(this IServiceCollection Services, IConfiguration Configuration)
        {
            Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            return Services;
        }
    }
}
