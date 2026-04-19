using E_Commerce.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryTranslation> CategoryTranslations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTranslation> ProductTranslations { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, 
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            builder.Entity<Category>()
                .HasOne(p=>p.CreatedBy)
                .WithMany()
                .HasForeignKey(p=>p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
               .HasOne(p => p.UpdatedBy)
               .WithMany()
               .HasForeignKey(p => p.UpdatedById)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
               .HasOne(p => p.UpdatedBy)
               .WithMany()
               .HasForeignKey(p => p.UpdatedById)
               .OnDelete(DeleteBehavior.Restrict);

        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {

           if(_httpContextAccessor.HttpContext != null)
            {
                var entries = ChangeTracker.Entries<AuditableEntity>();
                var currentUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(x => x.CreatedById).CurrentValue = currentUserId;
                        entry.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                        entry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
                    }
                }
            }


            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


    }
}