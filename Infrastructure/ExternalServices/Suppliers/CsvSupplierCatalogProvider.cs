using System.Globalization;
using System.Text;
using Shopniu_api.Aplication.Suppliers.Ports;
using Shopniu_api.Domain.Entities.SupplierEntity;

namespace Shopniu_api.Infrastructure.ExternalServices.Suppliers;

/// <summary>Provider de catálogo desde un CSV por proveedor
/// (`{DropShipping:CatalogDirectory}/{supplierId}.csv`, sin encabezado o con
/// fila de encabezados). Mientras no haya integración API real permite cargar
/// catálogos externos en disco para el sync. Archivo ausente = catálogo vacío.
/// Columnas: sku,name,costPrice,stock,imageUrl,description,leadTimeDays.</summary>
public class CsvSupplierCatalogProvider : ISupplierCatalogProvider
{
    private readonly IConfiguration _configuration;

    public CsvSupplierCatalogProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<SupplierCatalogItem>> FetchAsync(
        Supplier supplier,
        CancellationToken cancellationToken = default)
    {
        var directory = _configuration.GetValue<string>("DropShipping:CatalogDirectory") ?? "./SupplierCatalogs";
        var filePath = Path.Combine(directory, $"{supplier.Id}.csv");

        if (!File.Exists(filePath))
        {
            return Array.Empty<SupplierCatalogItem>();
        }

        var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
        var items = new List<SupplierCatalogItem>();

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var columns = ParseCsvLine(line);
            if (columns.Count < 7)
            {
                continue;
            }

            if (decimal.TryParse(columns[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var cost)
                && int.TryParse(columns[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out var stock)
                && int.TryParse(columns[6], NumberStyles.Integer, CultureInfo.InvariantCulture, out var leadTime))
            {
                items.Add(new SupplierCatalogItem(
                    Sku: columns[0].Trim(),
                    Name: columns[1].Trim(),
                    CostPrice: cost,
                    Stock: stock,
                    ImageUrl: columns[4].Trim(),
                    Description: columns[5].Trim(),
                    LeadTimeDays: leadTime
                ));
            }
        }

        return items;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(c);
                }
            }
            else if (c == '"')
            {
                inQuotes = true;
            }
            else if (c == ',')
            {
                result.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }

        result.Add(field.ToString());
        return result;
    }
}
