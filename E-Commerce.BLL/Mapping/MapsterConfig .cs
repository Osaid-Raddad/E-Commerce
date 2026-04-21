using E_Commerce.DAL.DTO.Request;
using E_Commerce.DAL.DTO.Response;
using E_Commerce.DAL.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Mapping
{
    public static class MapsterConfig
    {
        public static void MapsterConfigRegister()
        {
            TypeAdapterConfig<Category,CategoryResponse>.NewConfig()
            .Map(dest => dest.cat_Id, src => src.Id)
            .Map(dest => dest.UserCreated, src => src.CreatedBy.UserName)
            .Map(dest => dest.Name, src => src.Translations.Where
            (t=>t.Language== CultureInfo.CurrentCulture.Name)
            .Select(t=>t.Name).FirstOrDefault() );

            TypeAdapterConfig<Product, ProductResponse>.NewConfig()
            .Map(dest => dest.UserCreated, src => src.CreatedBy.UserName)
            .Map(dest => dest.Name, src => src.Translations.Where
            (t => t.Language == CultureInfo.CurrentCulture.Name)
            .Select(t => t.Name).FirstOrDefault() )
            .Map(dest => dest.MainImage,source => $"https://localhost:7003/images/{source.MainImage}");

            TypeAdapterConfig<ProductUpdateRequest, Product>.NewConfig()
            .IgnoreNullValues(true);

            TypeAdapterConfig<Cart, CartResponse>.NewConfig()
            .Map(dest => dest.ProductName, source => source.Product.Translations
            .Where(t => t.Language == CultureInfo.CurrentCulture.Name).Select(t => t.Name)
            .FirstOrDefault()).Map(dest => dest.Price, source => source.Product.Price)
            .Map(dest => dest.ProductImage, source => $"https://localhost:7003/images/{source.Product.MainImage}"); 


        }
    }
}
