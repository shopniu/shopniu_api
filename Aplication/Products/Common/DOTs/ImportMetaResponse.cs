namespace Shopniu_api.Aplication.Products.Common.DTOs;

/// <summary>Metadatos del flujo de importación de catálogo externo. Expuestos
/// al back-office (nunca al catálogo público) para poder previsualizar el
/// precio de venta antes de importar.</summary>
public sealed record ImportMetaResponse(decimal MarkupPercent);
