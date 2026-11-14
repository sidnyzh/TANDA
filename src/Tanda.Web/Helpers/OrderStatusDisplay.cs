using Tanda.Domain.Entities;

namespace Tanda.Web;

/// <summary>
/// Provee etiquetas y estilos de presentación para <see cref="OrderStatus"/>.
/// La traducción vive en Tanda.Web porque es responsabilidad de la capa de presentación,
/// manteniendo Tanda.Domain independiente de decisiones de UI e idioma.
/// </summary>
public static class OrderStatusDisplay
{
    public static string ToDisplayName(this OrderStatus status) =>
        status switch
        {
            OrderStatus.Quoted => "Cotizado",
            OrderStatus.Confirmed => "Confirmado",
            OrderStatus.InProduction => "En producción",
            OrderStatus.Ready => "Listo",
            OrderStatus.Delivered => "Entregado",
            OrderStatus.Cancelled => "Cancelado",
            _ => status.ToString()
        };

    public static string ToBadgeClass(this OrderStatus status) =>
        status switch
        {
            OrderStatus.Quoted => "badge bg-secondary",
            OrderStatus.Confirmed => "badge bg-primary",
            OrderStatus.InProduction => "badge bg-warning text-dark",
            OrderStatus.Ready => "badge bg-info text-dark",
            OrderStatus.Delivered => "badge bg-success",
            OrderStatus.Cancelled => "badge bg-danger",
            _ => "badge bg-light text-dark"
        };
}
