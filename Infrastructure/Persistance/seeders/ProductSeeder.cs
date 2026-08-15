// Infrastructure/Persistance/Seeders/ProductSeeder.cs
using Microsoft.EntityFrameworkCore;
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Infrastructure.Persistance.Seeders;

public static class ProductSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Products.AnyAsync())
            return; // Database has already been seeded

        var products = new List<Product>
        {
            new("Auriculares inalámbricos Pro", 299900m, "https://placehold.co/600x600?text=Auriculares", "Auriculares Bluetooth con cancelación de ruido activa y 30 horas de batería.", 25),
            new("Teclado mecánico RGB", 199900m, "https://placehold.co/600x600?text=Teclado", "Teclado mecánico con switches rojos y retroiluminación RGB personalizable.", 40),
            new("Mouse ergonómico", 89900m, "https://placehold.co/600x600?text=Mouse", "Mouse ergonómico con sensor óptico de alta precisión y diseño cómodo.", 60),
            new("Monitor 27\" 4K UHD", 1299900m, "https://placehold.co/600x600?text=Monitor", "Monitor UHD 4K con panel IPS, 98% DCI-P3 y soporte ajustable en altura.", 12),
            new("Silla gamer ergonómica", 899900m, "https://placehold.co/600x600?text=Silla", "Silla gaming con soporte lumbar, reposabrazos 4D y tapizado transpirable.", 8),
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
