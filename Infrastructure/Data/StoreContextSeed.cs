using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                // Instead of using the file or folder path
                // when publish the web app
                // we need to give the location of our executing assembly and get the directory from there

                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!context.ProductBrands.Any())
                {
                    // var brandsData = File.ReadAllText("../Infrastructure/Data/SeedData/brands.json");

                    var brandsData = File.ReadAllText(path + @"/Data/SeedData/brands.json");
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                    foreach (var item in brands)
                    {
                        context.ProductBrands.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.ProductTypes.Any())
                {
                    // var typesData = File.ReadAllText("../Infrastructure/Data/SeedData/types.json");

                    var typesData = File.ReadAllText(path + @"/Data/SeedData/types.json");
                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                    foreach (var item in types)
                    {
                        context.ProductTypes.Add(item);    
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.Products.Any())
                {
                    // var productsData = File.ReadAllText("../Infrastructure/Data/SeedData/products.json");

                    var productsData = File.ReadAllText(path + @"/Data/SeedData/products.json");
                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                    foreach (var item in products)
                    {
                        context.Products.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.DeliveryMethods.Any())
                {
                    var dmData = File.ReadAllText(path + @"/Data/SeedData/delivery.json");
                    var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

                    foreach (var item in methods)
                    {
                        context.DeliveryMethods.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex.Message);
            }
        }
    }
}