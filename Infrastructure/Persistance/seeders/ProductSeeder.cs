// Infrastructure/Persistance/Seeders/ProductSeeder.cs
using Microsoft.EntityFrameworkCore;
using Shopniu_api.Domain.Entities.ProductEntity;

namespace Shopniu_api.Infrastructure.Persistance.Seeders;

public static class ProductSeeder
{
    public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
    {
        // Ids de los usuarios back-office en el servicio de identidad. En una BD
        // de identidad fresca el orden del UserSeeder deja admin=1 y seller=3;
        // se sobrescriben por configuración si en el ambiente difieren.
        var adminId = configuration.GetValue("Database:Seeding:AdminUserId", 1);
        var sellerId = configuration.GetValue("Database:Seeding:SellerUserId", 3);

        // Backfill idempotente: los productos sin ningún dueño (sembrados antes
        // del modelo de propiedad) quedan asignados a admin y a seller.
        var orphanProductIds = await context.Products
            .Where(p => !context.ProductOwners.Any(po => po.ProductId == p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        foreach (var productId in orphanProductIds)
        {
            context.ProductOwners.AddRange(
                new ProductOwner { ProductId = productId, UserId = adminId },
                new ProductOwner { ProductId = productId, UserId = sellerId }
            );
        }

        if (await context.Products.AnyAsync())
        {
            await context.SaveChangesAsync();
            return; // Ya había productos: solo correspondía el backfill de dueños.
        }

        // El creador exacto de todos los productos es admin.
        var products = new List<Product>
        {
            new("Auriculares inalámbricos Pro", 299900m, "https://placehold.co/600x600?text=Auriculares", "Auriculares Bluetooth con cancelación de ruido activa y 30 horas de batería.", 25, adminId),
            new("Teclado mecánico RGB", 199900m, "https://placehold.co/600x600?text=Teclado", "Teclado mecánico con switches rojos y retroiluminación RGB personalizable.", 40, adminId),
            new("Mouse ergonómico", 89900m, "https://placehold.co/600x600?text=Mouse", "Mouse ergonómico con sensor óptico de alta precisión y diseño cómodo.", 60, adminId),
            new("Monitor 27\" 4K UHD", 1299900m, "https://placehold.co/600x600?text=Monitor", "Monitor UHD 4K con panel IPS, 98% DCI-P3 y soporte ajustable en altura.", 12, adminId),
            new("Silla gamer ergonómica", 899900m, "https://placehold.co/600x600?text=Silla", "Silla gaming con soporte lumbar, reposabrazos 4D y tapizado transpirable.", 8, adminId),
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // El creador exacto es admin; ambos back-office pueden gestionarlos.
        var owners = products
            .SelectMany(product => new[]
            {
                new ProductOwner { ProductId = product.Id, UserId = adminId },
                new ProductOwner { ProductId = product.Id, UserId = sellerId },
            })
            .ToList();

        await context.ProductOwners.AddRangeAsync(owners);
        await context.SaveChangesAsync();
    }
}
